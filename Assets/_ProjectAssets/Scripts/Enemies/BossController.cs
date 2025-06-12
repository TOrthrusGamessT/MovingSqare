using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public List<ParticleSystem> lasers;
    public Transform boomerangSpawnPoint;
    public GameObject boomerangPrefab;
    public Transform leftBarrierSpawnPPoint;
    public Transform rightBarrierSpawnPPoint;
    public Constants.BarrierSet[] barierSet;
    public ParticleSystem[] destroyEffect;
    public float timeBetweenSpawnBarrier;
    public RectTransform rewardText;

    public GameObject littleBarrier;
    public GameObject mediumBarrier;
    public GameObject bigBarrier;

    public GameObject head;
    public GameObject[] guns;
    public GameObject shield;
    [HideInInspector] public bool inAnimation;
    public SpawnManagerLvls spawnManagerLvl;
    public bool nextStage;


    private Animator _animator;
    private GameObject barier;
    private Rigidbody2D _rb;
    private int life = 2;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public void ActivateLasers()
    {
        foreach (var laser in lasers)
        {
            laser.Play();
        }
    }

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

    public void SecondAttackBegin()
    {
        UniTask.Void(async () =>
        {
            bool finishAnimation = false;

            LeanTween.scaleX(shield, 4.68f, 2.5f).setEaseInQuad().setOnComplete(() =>
            {
                spawnManagerLvl.StartSecondAttack();
                finishAnimation = true;
            });

            await UniTask.WaitUntil(() => finishAnimation);

            await UniTask.Delay(TimeSpan.FromSeconds(35f));

            DeactivateShield();
        });
    }

    [ContextMenu("ThirdAttack")]
    public void ThirdAttackBegin()
    {
        UniTask.Void(async () =>
        {
            bool finishAnimation = false;

            LeanTween.scaleX(shield, 4.68f, 2.5f).setEaseInQuad().setOnComplete(() =>
                {
                    LeanTween.moveY(shield, -3.54f, 2f).setEaseInQuad().setOnComplete(() =>
                    {
                        finishAnimation = true;
                        LeanTween.moveLocalY(shield, 1.83f, 0.5f).setEaseInQuad();
                    });
                });

            await UniTask.WaitUntil(() => finishAnimation);

            foreach (var t in barierSet)
            {
                switch (t.barrierPosition)
                {
                    case Constants.BarrierPosition.Left:
                        {
                            DefineBarrierType(t.barrierType);
                            GameObject newBarrier =
                                Instantiate(barier, leftBarrierSpawnPPoint.position, Quaternion.identity);
                            newBarrier.GetComponent<BarierBehaviour>().Appear(Constants.BarrierPosition.Left);
                            break;
                        }

                    case Constants.BarrierPosition.Right:
                        {
                            DefineBarrierType(t.barrierType);
                            GameObject newBarrier =
                                Instantiate(barier, rightBarrierSpawnPPoint.position, Quaternion.identity);
                            newBarrier.GetComponent<BarierBehaviour>().Appear(Constants.BarrierPosition.Right);
                            break;
                        }
                }

                await UniTask.Delay((int)timeBetweenSpawnBarrier * 1000);
            }

            UniTask.Delay(TimeSpan.FromSeconds(2.5f));
            DeactivateShield();
            _animator.SetTrigger("FinishThirdAttack");
        });
    }

    private void DefineBarrierType(Constants.BarrierType barrierType)
    {
        switch (barrierType)
        {
            case Constants.BarrierType.LittleBarrier:
                {
                    barier = littleBarrier;
                    break;
                }
            case Constants.BarrierType.MediumBarrier:
                {
                    barier = mediumBarrier;
                    break;
                }

            case Constants.BarrierType.BigBarrier:
                {
                    barier = bigBarrier;
                    break;
                }
        }
    }

    private void DeactivateShield()
    {
        LeanTween.scaleX(shield, 0, 2.5f).setEaseInQuad();
    }

    private void TakeDamage()
    {
        if (life == 0)
        {
            _animator.SetTrigger("Death");
        }
        else
        {
            _animator.SetBool("NextStage", true);
            nextStage = true;
            life--;
        }

    }

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

    public void SetBossPosition()
    {
        LeanTween.moveLocalY(gameObject, 2.951164f, 4f).setEaseInQuad().setOnComplete(() => _animator.SetTrigger("StartBossBehaviour"));
    }

    private void OnParticleCollision(GameObject other)
    {
        TakeDamage();
    }
}