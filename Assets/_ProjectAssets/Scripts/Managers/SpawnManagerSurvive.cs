using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;


public class SpawnManagerSurvive : Spawner
{

    public GameStats gameStats;

    public List<GameObject> obstacles;

    private float lvlDuration;

    protected override void Start()
    {
        InitLvlStats();
        InitPowerUps();
    }

    protected override void InitLvlStats()
    {
        timeBetweenSpawnPowerUps = gameStats.spawnPowerUpsTime;
        timeBetweenSpawnMoney = gameStats.spawnMoneyTime;
        timeBetweenSpawnsGeometricFigures = gameStats.spawnEnemyTime;

        onSpawnManagerSetCoins?.Invoke(timeBetweenSpawnMoney);
    }


    protected override void InitPowerUps()
    {
        availablePowerUps = new();

        if (PlayerPrefs.HasKey(PowerUps.heal.ToString()))
        {
            availablePowerUps.Add(powerUps[0]);
        }

        if (PlayerPrefs.HasKey(PowerUps.size.ToString()))
        {
            availablePowerUps.Add(powerUps[1]);
        }

        if (PlayerPrefs.HasKey(PowerUps.speed.ToString()))
        {
            availablePowerUps.Add(powerUps[2]);
        }

    }


    public override void StartSpawning()
    {
        CoinsBehaviour.Lifetime = gameStats.moneyLifeTime;
        StartCoroutine(SpawnMoney());
        StartCoroutine(SpawnGeometricFigures());

        if (availablePowerUps.Count != 0)
        {
            StartCoroutine(SpawnPowerUps());
        }

        Debug.Log("Start Spawning");

    }


    #region SpawnObstacle

    public IEnumerator SpawnObstacle()
    {
        yield return new WaitForSeconds(DificultyManager.instance.obstacleTimeDelay);

        Instantiate(obstacles[Random.Range(0, obstacles.Count)], obstacleSpawnPoint.position, Quaternion.identity);
        StopSpawning();
    }

    #endregion

    #region Spawn fullScreenLines

    public override IEnumerator SpawnLines()
    {
        yield return new WaitForSeconds(DificultyManager.instance.lineTimeDelay);

        for (int i = 0; i < timeBetweenSpawnLasers; i++)
        {
            fullScreenLineObjects.Add(Instantiate(laser, new Vector2(Random.Range(minX, maxX)
              , Random.Range(minY, maxY)), Quaternion.Euler(0, 0, Random.RandomRange(0, 180))).GetComponent<FullScreenLine>());

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1.5f);

        foreach (var line in fullScreenLineObjects)
        {
            line.Activate();
        }

        yield return new WaitForSeconds(linesLife);

        foreach (var line in fullScreenLineObjects)
        {
            line.DestroyLaser();
        }

        fullScreenLineObjects.Clear();

        StartCoroutine(SpawnLines());
    }

    #endregion

    #region Spawn Money

    protected override IEnumerator SpawnMoney()
    {
        yield return new WaitForSeconds(timeBetweenSpawnMoney);
        Instantiate(coinPrefab, new Vector2(Random.Range(minX, maxX)
            , Random.Range(minY, maxY)), Quaternion.identity);

        StartCoroutine(SpawnMoney());
    }

    #endregion

    #region Spawn Enemy

    protected override IEnumerator SpawnGeometricFigures()
    {
        yield return new WaitForSeconds(DificultyManager.instance.enemyTimeDelay);

        EnemyBehaviour objectToSpawn = geometricFigures[Random.Range(0, geometricFigures.Count)];
        Transform spawnPoint = spawningPoints[Random.Range(0, spawningPoints.Count)];

        //Set direction and Target 

        attentionSignBehaviour = Instantiate(enemyAlertSignPrefab, spawnPoint.position, spawnPoint.rotation)
            .GetComponent<AttentionSignBehaviour>();
        SetEnemyDirection(spawnPoint);
        attentionSignBehaviour.target =
            Instantiate(objectToSpawn, positionToSpawn, Quaternion.identity).GetComponent<Transform>();

        StartCoroutine(SpawnGeometricFigures());
    }


    protected override void SetEnemyDirection(Transform spawnedPoint)
    {
        switch (spawnedPoint.name)
        {
            case "E":
                positionToSpawn = new Vector2(5.99f, Random.Range(minE, maxE));
                attentionSignBehaviour.enemyDirection = Constants.Directions.E;
                break;
            case "V":
                positionToSpawn = new Vector2(-4.9f, Random.Range(minV, maxV));
                attentionSignBehaviour.enemyDirection = Constants.Directions.V;
                break;
            case "W":
                positionToSpawn = new Vector2(Random.Range(minW, maxW), 6.14f);
                attentionSignBehaviour.enemyDirection = Constants.Directions.W;
                break;
            case "S":
                positionToSpawn = new Vector2(Random.Range(minS, maxS), -6.72f);
                attentionSignBehaviour.enemyDirection = Constants.Directions.S;
                break;
            default:
                Debug.LogError("Error in Enemy Direction");
                break;
        }
    }

    #endregion

    #region Spawn Power Ups

    protected override IEnumerator SpawnPowerUps()
    {
        yield return new WaitForSeconds(timeBetweenSpawnPowerUps);
        Instantiate(availablePowerUps[Random.Range(0, availablePowerUps.Count - 1)], new Vector2(Random.Range(minX, maxX)
            , Random.Range(minY, maxY)), Quaternion.identity);
        StartCoroutine(SpawnPowerUps());
    }

    #endregion
}