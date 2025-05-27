using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class BossWaves
{
    public int bossHP;
    public int durationOfWave;
}

public class BossGameplay : MonoBehaviour
{
    public static event Action OnBossAppear;
    public static event Action OnBossDisappear;

    public DataManager dataManager;
    public List<GameObject> fireWorks;
    public GameObject winSign;
    public SpawnManagerSurvive spawnManagerSurvive;
    public GameManager gameManager;
    public UIManagerGameRoom uiManagerGameRoom;
    public BossBehaivour bossPrefab;
    public GameObject leaksPrefab;
    public GameObject portal;
    public BossWaves[] bossWaves;

    private int index;
    public bool finishedWave;

    private void Start()
    {
        StartCoroutine(NextWave());
    }

    IEnumerator NextWave()
    {
        bossPrefab.SetBossStatus(bossWaves[index].bossHP);

        yield return new WaitForSeconds(bossWaves[index].durationOfWave);

        bossPrefab.gameObject.SetActive(true);

        OnBossAppear?.Invoke();
        bossPrefab.InitBoss();

        yield return new WaitUntil(() => finishedWave);
        index++;

        if (index >= bossWaves.Length)
        {
            DataManager.MoneyCollected = 200;
            dataManager.SaveMoney();
            InitiateSpoiler();
        }
        else
        {
            finishedWave = false;
            OnBossDisappear?.Invoke();
            StartCoroutine(NextWave());
        }
    }

    [ContextMenu("Test Wining State")]
    private void InitiateSpoiler()
    {
        portal.SetActive(true);

        foreach (var fireWork in fireWorks)
        {
            fireWork.SetActive(true);
        }

        LeanTween.scale(winSign, Vector3.one, 1f).setEaseInBounce().setOnComplete(() =>
        {
            LeanTween.scale(winSign, Vector3.zero, 2f).setEaseInBounce().setOnComplete(() =>
            {
                LeanTween.scaleX(portal, 0.33f, 2f).setEaseInCubic().setOnComplete(() =>
                {
                    LeanTween.moveX(leaksPrefab, 5, 3f).setEaseInCubic();
                });

                LeanTween.scaleX(portal.transform.GetChild(0).gameObject, 0.5f, 2f).setEaseInCubic();
            }).setDelay(6f);
        });


    }
}
