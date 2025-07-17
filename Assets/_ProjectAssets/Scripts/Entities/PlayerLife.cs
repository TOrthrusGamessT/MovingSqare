using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour
{
    public static Action onPlayerDie;
    public static Action<int> onPlayerGotLife;

    [SerializeField] private SpriteRenderer _playerBody;

    private float invincibleDuration = 3f; // Duration of invincibility effect in seconds
    [SerializeField] private Image fillImage; // Reference to the fill image for invincibility effect
    private BoxCollider2D _boxCollider2D;



    private int life = 1;

    public int Life
    {
        get => life;
    }


    void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    void OnEnable()
    {
        AdsManager.onReviveADFinish += PlayerWatchedReviveAd;
    }

    void OnDisable()
    {
        AdsManager.onReviveADFinish -= PlayerWatchedReviveAd;
    }

    private void PlayerWatchedReviveAd()
    {
        AddLife(1);
        Invincible();
    }

    public void AddLife(int amount)
    {
        if (life + amount > 3)
        {
            life = 3;
            onPlayerGotLife?.Invoke(life);
        }
        else
        {
            life += 1;
            onPlayerGotLife?.Invoke(life);
        }
    }

    public void Damage(int damage)
    {
        if (life - damage < 0)
            return;
        BackgroundParticlesManager.instance.TakeDamageAnimation();
        Handheld.Vibrate();
        life -= damage;
        Invincible();
        EffectManager.DamageEffect();
        UIManagerGameRoom.instance.DecreaseLife();
        CameraShaking.Shake();
        if (life <= 0)
        {
            FireBase.LogCustomEvent("player_died", new System.Collections.Generic.Dictionary<string, object>
            {
                { "total_seconds_survived", Timer.ElapsedSeconds }
            });
            onPlayerDie?.Invoke();
            GameManager.instance.GameOver();
        }

    }

    private void Invincible()
    {
        // Cancel any existing invincibility animations
        _playerBody.DOKill();
        fillImage.DOKill();
        CancelInvoke(nameof(EnableCollider));
        
        _boxCollider2D.enabled = false;

        // Reset initial states
        fillImage.fillAmount = 1f;
        _playerBody.color = new Color(_playerBody.color.r, _playerBody.color.g, _playerBody.color.b, 1f);

        // Create a synchronized sequence
        Sequence invincibilitySequence = DOTween.Sequence();
        
        // Add both animations to the same sequence for perfect synchronization
        float fadeInterval = 0.25f;
        int loopCount = Mathf.FloorToInt(invincibleDuration / (fadeInterval * 2));
        
        invincibilitySequence.Join(fillImage.DOFillAmount(0f, invincibleDuration).SetEase(Ease.Linear));
        invincibilitySequence.Join(_playerBody.DOFade(0.5f, fadeInterval)
            .SetLoops(loopCount, LoopType.Yoyo)
            .SetEase(Ease.InOutCubic));
        
        // Ensure both animations complete at the same time
        invincibilitySequence.OnComplete(() => {
            _playerBody.color = new Color(_playerBody.color.r, _playerBody.color.g, _playerBody.color.b, 1f);
            _boxCollider2D.enabled = true;
        });

        // Backup timer to ensure collider is re-enabled even if animation fails
        Invoke(nameof(EnableCollider), invincibleDuration + 0.1f);
    }

    private void EnableCollider()
    {
        if (_boxCollider2D != null)
        {
            _boxCollider2D.enabled = true;
            _playerBody.color = new Color(_playerBody.color.r, _playerBody.color.g, _playerBody.color.b, 1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {

        if (col.gameObject.CompareTag("Projectile"))
        {
            SoundManager.instance.PlaySoundEffect(Constants.Sounds.PlayerGetHit);
            Damage(1);
            CameraShaking.Shake();
        }

        if (col.gameObject.CompareTag("Obstacle"))
        {
            Damage(1);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        SoundManager.instance.PlaySoundEffect(Constants.Sounds.PlayerGetHit);
        Damage(1);
        CameraShaking.Shake();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {

        if (col.gameObject.CompareTag("DeadZone"))
        {
            Damage(1);
        }

        if (col.gameObject.CompareTag("Enemy"))
        {
            Damage(1);
            SoundManager.instance.PlaySoundEffect(Constants.Sounds.PlayerGetHit);
            CameraShaking.Shake();
        }

    }

    void OnDestroy()
    {
        DOTween.KillAll();
    }

}
