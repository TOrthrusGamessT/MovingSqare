using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUps
{
    heal,
    speed,
    size
}


public abstract class Spawner : MonoBehaviour
{
    //TODO find a better way
    public static Action<float> onSpawnManagerSetCoins;

    [Header("Obstacles")]
    public List<EnemyBehaviour> geometricFigures;
    public GameObject enemyAlertSignPrefab;
    public GameObject laser;
    public List<Transform> spawningPoints;

    [Header("Helpers")]
    public List<GameObject> powerUps;
    public GameObject coinPrefab;

    [Header("Parameters")]
    public float timeBetweenSpawnsGeometricFigures;
    public float timeBetweenSpawnPowerUps;
    public float timeBetweenSpawnMoney;
    public float timeBetweenSpawnLasers;
    public float linesLife;
    public Transform obstacleSpawnPoint;

    private List<Color> brightColorPalette = new List<Color>
    {
        Color.HSVToRGB(0f, 0.4f, 1f),        // More saturated Red
        Color.HSVToRGB(240f/360f, 0.4f, 1f), // More saturated Blue
        Color.HSVToRGB(60f/360f, 0.4f, 1f),  // More saturated Yellow
        Color.HSVToRGB(300f/360f, 0.4f, 1f), // More saturated Magenta
        Color.HSVToRGB(30f/360f, 0.4f, 1f),  // More saturated Orange`
        Color.HSVToRGB(270f/360f, 0.4f, 1f), // More saturated Purple
        Color.HSVToRGB(330f/360f, 0.4f, 1f), // More saturated Pink
        Color.HSVToRGB(45f/360f, 0.4f, 1f),  // More saturated  Gold
        Color.HSVToRGB(200f/360f, 0.4f, 1f)  // More saturated Sky Blue
    };

    [Header("Boundaries")]
    [Header("V Values")]
    public float maxV = 4.41f;
    public float minV = -2.93f;
    [Header("W Values")]
    public float maxW = 4.24f;
    public float minW = -3.23f;
    [Header("E Values")]
    public float maxE = 4.38f;
    public float minE = -3.07f;
    [Header("S Values")]
    public float maxS = 2.62f;
    public float minS = -2.27f;

    [Header("Money Spawner Zone")]
    public float maxX = 1.54f;
    public float minX = -1.59f;
    public float maxY = 4.35f;
    public float minY = -2.04f;

    protected Vector2 positionToSpawn;
    protected AttentionSignBehaviour attentionSignBehaviour;
    protected List<GameObject> availablePowerUps;
    protected List<FullScreenLine> fullScreenLineObjects = new();

    protected virtual void OnEnable()
    {
        BossGameplay.OnBossAppear += StopSpawning;
        BossGameplay.OnBossDisappear += StartSpawning;
        GameManager.onGameOver += StopSpawning;
        AdsManager.onReviveADFinish += StartSpawning;
    }

    protected virtual void OnDisable()
    {
        BossGameplay.OnBossAppear -= StopSpawning;
        BossGameplay.OnBossDisappear -= StartSpawning;
        GameManager.onGameOver -= StopSpawning;
        AdsManager.onReviveADFinish -= StartSpawning;
    }

    protected abstract void Start();

    public abstract void StartSpawning();

    protected void StopSpawning()
    {
        try
        {
            StopAllCoroutines();
            foreach (var fullScreenLine in fullScreenLineObjects)
            {
                fullScreenLine.DestroyLaser();
            }
            fullScreenLineObjects.Clear();
        }
        catch (Exception e)
        {
            Debug.LogWarning("plm carpeala" + this.GetInstanceID());
        }

    }

    protected abstract void InitLvlStats();

    protected abstract void InitPowerUps();


    public abstract IEnumerator SpawnLines();

    protected abstract IEnumerator SpawnMoney();

    protected abstract IEnumerator SpawnGeometricFigures();

    protected abstract void SetEnemyDirection(Transform spawnedPoint);


    protected abstract IEnumerator SpawnPowerUps();
    protected void ApplyColor(EnemyBehaviour enemy)
    {
        if (enemy != null && brightColorPalette.Count > 0)
        {
            Color randomColor = brightColorPalette[UnityEngine.Random.Range(0, brightColorPalette.Count)];
            enemy.SetColor(randomColor);
        }
    }

}
