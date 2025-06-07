using System;
using DG.Tweening;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    public static Action onPlayerDie;
    public static Action<int> onPlayerGotLife;

    [SerializeField] private SpriteRenderer _playerBody;
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
        if (life <= 0)
        {
            onPlayerDie?.Invoke();
            GameManager.instance.GameOver();
        }

    }

    private void Invincible()
    {
        _boxCollider2D.enabled = false;

        _playerBody.DOFade(0.5f, 0.5f)
            .SetLoops(10, LoopType.Yoyo)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() => _boxCollider2D.enabled = true);
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
