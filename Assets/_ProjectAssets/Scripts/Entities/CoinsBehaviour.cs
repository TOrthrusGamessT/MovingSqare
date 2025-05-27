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

    private void Start()
    {
        Invoke(nameof(Destroy), life);
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
    }
}
