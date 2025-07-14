using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public List<ParticleSystem> lasers;
    public Transform boomerangSpawnPoint;
    public GameObject boomerangPrefab;
    public Transform leftBarrierSpawnPPoint;
    public Transform rightBarrierSpawnPPoint;

    public ParticleSystem[] destroyEffect;
    public RectTransform rewardText;

    public GameObject head;
    public GameObject[] guns;
    public GameObject shield;
    public SpawnManagerLvls spawnManagerLvl;


    private Animator _animator;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }


    
#region Shield
    public async UniTask ActivateShield()
    {
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

#region Boomerang
    public void SpawnBoomerang()
    {
        Boomerang boomerang = Instantiate(boomerangPrefab, boomerangSpawnPoint.position, Quaternion.identity)
            .GetComponent<Boomerang>();
        boomerang.Instantiate(GameObject.FindWithTag("Player").transform);

        HeadAnimation();
    }

     private void HeadAnimation()
    {
        LeanTween.moveLocalY(head.gameObject, -0.11f, 4.5f).setEaseInCubic().setOnComplete(() =>
        {
            LeanTween.moveLocalY(head.gameObject, -1.111098f, 0.5f).setEaseInCubic();
        });

        foreach (var gun in guns)
        {
            LeanTween.moveLocalY(gun.gameObject, 0.93f, 4.5f).setEaseInCubic().setOnComplete(() =>
            {
                LeanTween.moveLocalY(gun.gameObject, 0.14f, 0.5f).setEaseInCubic();
            });
        }
    }
#endregion
#region TakeDamage
    private void OnParticleCollision(GameObject other)
    {
        TakeDamage();
    }

        private void TakeDamage()
    {
        //TODO: As scoate din timp vri-o 10 sec poate

    }
    #endregion
   
    public void Death()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        _rb.gravityScale = 1;
        _rb.linearDamping = 16;
        foreach (var particleSystem in destroyEffect)
        {
            particleSystem.Play();
        }

        LeanTween.rotate(gameObject, new Vector3(0, 0, 180), 30f).setEaseInQuad();
        LeanTween.scale(rewardText, new Vector3(1, 1, 1), 1f).setEaseInQuad().setOnComplete(() =>
        {
            DataManager.MoneyCollected = 500;
            UIManagerGameRoom.instance.FinishLvlState();
        });

    }
}