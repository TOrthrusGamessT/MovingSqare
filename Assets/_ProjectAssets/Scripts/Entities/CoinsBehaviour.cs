using System;
using UnityEngine;

public class CoinsBehaviour : MonoBehaviour
{

    public static Action<bool> onCoinDestroy;
    public static Action<int> onCoinCollected;
    public static int amount;
    private static int life;

    [SerializeField] private GameObject destroyVFX;
    [SerializeField] private GameObject vfx;
    [SerializeField] private ColectMoneyEffect textEffect;

    public static int Lifetime
    {
        get => life;
        set => life = value;

    }

    private static int _combo = 0;

    // Movement properties
    private Vector2 velocity;
    private bool shouldMove;
    private Rigidbody2D rb;

    private void Start()
    {
        Invoke(nameof(Destroy), life);

        // Create and apply physics material for bouncy behavior
        PhysicsMaterial2D bouncyMaterial = new PhysicsMaterial2D("CoinBouncy");
        bouncyMaterial.friction = 0.1f;    // Low friction
        bouncyMaterial.bounciness = 0.8f;  // High bounciness
        GetComponent<CircleCollider2D>().sharedMaterial = bouncyMaterial;

        GetComponent<Rigidbody2D>().AddForce(new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * 100f);

    }

    private bool ShouldCoinMove()
    {
        // Check if level > 10 (currentLvlIndex is 0-based, so > 9 means level > 10)
        if (LVLIndexer.currentLvlIndex > 9)
        {
            return true;
        }

        // Check if in survival mode and timer > 60 seconds from the survival start
        if (Timer.IsSurviveMode && Timer.ElapsedSeconds > 10)
        {
            return true;
        }

        return false;
    }


    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            _combo++;
            ColectMoneyEffect colectMoneyEffect = Instantiate(textEffect, transform.position + Vector3.forward * -9.125f, Quaternion.identity);
            colectMoneyEffect.ShowText(_combo);
            Instantiate(vfx, transform.position, Quaternion.identity);
            BackgroundParticlesManager.instance.TakeCoinAnimation();
            SoundManager.instance.PlaySoundEffect(Constants.Sounds.PickCoin);
            onCoinDestroy?.Invoke(true);
            onCoinCollected?.Invoke(amount);
            Destroy(gameObject);
        }
        else if (shouldMove && col.gameObject.layer == LayerMask.NameToLayer("MovingZone"))
        {

        }
    }
    

    private void Destroy()
    {
        Instantiate(destroyVFX, transform.position, Quaternion.identity);
        onCoinDestroy?.Invoke(false);
        ColectMoneyEffect colectMoneyEffect = Instantiate(textEffect, transform.position + Vector3.forward * -9.125f, Quaternion.identity);
        colectMoneyEffect.ShowText(0);
        _combo = 0;
        Destroy(gameObject);
    }
    
}
