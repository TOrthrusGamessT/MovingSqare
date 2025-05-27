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

    public void InitPlayer(Item item)
    {
        _playerBody.sprite = item.sprite;
        trail.GetComponent<Renderer>().material.SetTexture("_BaseMap", item.trailTexture);

        ApplyEffect(item);
    }


    private void ApplyEffect(Item item)
    {

        switch (item.effects[0].effect)
        {
            case EffectType.Size:
                {
                    transform.localScale -= CalculatePercentage(item.effects[0].value) * Vector3.one;
                    //float scale =  GetComponent<TrailRenderer>().widthMultiplier - 0.12f*PlayerPrefs.GetInt(item.effects[0].name);
                    //GetComponent<TrailRenderer>().widthMultiplier = scale;
                    break;
                }
            case EffectType.Speed:
                {
                    movement.speed += CalculateSpeed(movement.speed, item.effects[0].value);
                    break;
                }
            case EffectType.Life:
                {
                    playerLife.AddLife((int)item.effects[0].value);
                    break;
                }
            default:
                {
                    Debug.Log("NoEffectApplied");
                    break;
                }
        }
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
