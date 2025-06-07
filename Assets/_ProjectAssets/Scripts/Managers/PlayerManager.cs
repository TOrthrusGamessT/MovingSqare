using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public PlayerLife playerLife;
    public Movement movement;
    public ParticleSystem deathEffect;
    public ParticleSystem trail;

    [SerializeField] private SpriteRenderer _playerBody;



    private void OnEnable()
    {
        GameManager.onGameOver += PlayerDeath;
        Timer.onCounterEnd += WinLvl;
        AdsManager.onReviveADFinish += Revive;
    }


    private void OnDisable()
    {
        GameManager.onGameOver -= PlayerDeath;
        Timer.onCounterEnd -= WinLvl;
        AdsManager.onReviveADFinish -= Revive;
    }

    private void Start()
    {
        trail = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    public void InitPlayer(SkinData skin)
    {
        _playerBody.sprite = skin.sprite;
        trail.GetComponent<Renderer>().material.SetTexture("_BaseMap", skin.trailTexture);

        ApplyEffect(skin);
    }


    private void ApplyEffect(SkinData skin)
    {

        transform.localScale /= skin.sizeBonus;
        movement.speed*= skin.speedBonus;
        playerLife.AddLife(skin.lives);
    }

    private void WinLvl()
    {
        GetComponent<BoxCollider2D>().enabled = false;
    }

    private void PlayerDeath()
    {
        deathEffect.Play();
        _playerBody.enabled = false;
        trail.gameObject.SetActive(false);
    }

    private void Revive()
    {
        _playerBody.enabled = true;
        trail.gameObject.SetActive(true);
        transform.position = Vector3.zero;
    }


    private void OnCollisionEnter2D(Collision2D col)
    {

        if (col.gameObject.CompareTag("PowerUp"))
        {
            col.gameObject.GetComponent<PowerUpBehaviour>().Effect();
        }

    }


    private float CalculatePercentage(float effect)
    {
        float soum = transform.localScale.x * effect;
        return soum / 100;
    }

    private float CalculateSpeed(float defaultSpeed, float speedBonus)
    {
        return (defaultSpeed / 100) * speedBonus;
    }

    /*#if !UNITY_EDITOR
        private void OnBecameInvisible()
        {
            playerLife.Damage(playerLife.Life);
        }

    #endif*/
}
