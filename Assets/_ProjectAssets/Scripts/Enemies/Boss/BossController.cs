using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossController : MonoBehaviour
{
    public List<ParticleSystem> lasers;
    public GameObject boomerangPrefab;
    public Transform leftBarrierSpawnPPoint;
    public Transform rightBarrierSpawnPPoint;

    public ParticleSystem[] destroyEffect;
    public RectTransform rewardText;
    public GameObject shield;
    public SpawnManagerLvls spawnManagerLvl;


    private Animator _animator;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        Timer.onCounterEnd += () =>
        {
            _animator.SetTrigger("Destroy");
        };
    }


    
#region Shield
    public async UniTask ActivateShield()
    {
        shield.GetComponent<SpriteRenderer>().color = Constants.brightColorPalette[Random.Range(0, Constants.brightColorPalette.Count)];
        LeanTween.scaleX(shield, 4.68f, 2.5f).setEaseInQuad().setOnComplete(() =>
                {
                    LeanTween.moveY(shield, -3.54f, 2f).setEaseInQuad().setOnComplete(() =>
                    {
                        LeanTween.moveLocalY(shield, 1.83f, 0.5f).setEaseInQuad();
                    });
                });
        //5f = 2.5+2+0.5
        await UniTask.Delay(TimeSpan.FromSeconds(5f));
        return;
    }
    public async UniTask DeactivateShield()
    {
        LeanTween.scaleX(shield, 0, 2.5f).setEaseInQuad();
        await UniTask.Delay(TimeSpan.FromSeconds(2.5f));
        return;
    }
#endregion
}