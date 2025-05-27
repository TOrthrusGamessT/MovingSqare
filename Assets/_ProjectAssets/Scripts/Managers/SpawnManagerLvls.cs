using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManagerLvls : Spawner
{

   //TODO this should be in Game Manager
   public LVLIndexer LvlIndexer;
   public GameObject boss;

   [Header("Lvl Settings")]
   public Vector2 tetrisRange;
   public Transform tetrisSpawnPoint;

   [Header("Tetris Prefabs")]
   public GameObject[] tetrisPrefabs;
   public GameObject tetrisDestroyEffect;

   private GameObject _mazePrefab;
   private float _geometryFiguresSpeed;
   private float _tetrisEnemiesSpeed;
   private float _timeBetweenSpawnMaze;
   private bool _lvlOver;
   private float _timeBetweenSpawnTetris;
   private float _timeBetweenSpawnTetrisEnemies;
   private int _tetrisPeacesOnTime;
   private LvlSettings _lvlSettings;

   private List<EnemyBehaviour> _geometryToSpawn = new();

   protected override void OnEnable()
   {
      base.OnEnable();
      Timer.onCounterEnd += () =>
      {
         _lvlOver = true;
         StopSpawning();
      };
   }

   protected override void OnDisable()
   {
      base.OnDisable();
      Timer.onCounterEnd -= () =>
      {
         _lvlOver = true;
         StopSpawning();
      };
   }


   protected override void Start()
   {
      SetLvlValues();
   }

   public void SetLvlValues()
   {
      _lvlSettings = LvlIndexer.GetCurrentLvlSettings();
      InitLvlStats();
      InitPowerUps();
   }

   protected override void InitLvlStats()
   {
      timeBetweenSpawnsGeometricFigures = _lvlSettings.timeBetweenSpawnGeometricFigures;
      timeBetweenSpawnPowerUps = _lvlSettings.timeBetweenSpawnPowerUps;
      timeBetweenSpawnMoney = _lvlSettings.timeBetweenSpawnMoney;
      timeBetweenSpawnLasers = _lvlSettings.timeBetweenSpawnLasers;
      linesLife = _lvlSettings.linesLife;
      _timeBetweenSpawnMaze = _lvlSettings.timeBetweenSpawnMaze;
      _mazePrefab = _lvlSettings.obstaclePrefab;
      _geometryFiguresSpeed = _lvlSettings.geometricFiguresSpeed;
      _tetrisEnemiesSpeed = _lvlSettings.tetrisEnemiesSpeed;
      _timeBetweenSpawnTetrisEnemies = _lvlSettings.timeBetweenSpawnTetrisEnemies;
      Timer.Duration = _lvlSettings.lvlDuration;

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
      if (_lvlOver)
         return;

      if (_lvlSettings.lvl10Boss)
      {
         boss.SetActive(true);
         return;
      }

      if (_lvlSettings.geometryFigures)
      {
         _geometryToSpawn = geometricFigures;
      }
      else
      {
         if (_lvlSettings.square)
         {
            _geometryToSpawn.Add(geometricFigures.Find(x => x.figure == Constants.GeometryFigure.Square));
         }

         if (_lvlSettings.circle)
         {
            _geometryToSpawn.Add(geometricFigures.Find(x => x.figure == Constants.GeometryFigure.Circle));
         }

         if (_lvlSettings.hexagon)
         {
            _geometryToSpawn.Add(geometricFigures.Find(x => x.figure == Constants.GeometryFigure.Hexagon));
         }
         StartCoroutine(SpawnGeometricFigures());
      }


      if (_lvlSettings.lasers)
      {
         StartCoroutine(SpawnLines());
      }

      if (_lvlSettings.maze)
      {
         _mazePrefab = _lvlSettings.obstaclePrefab;
         StartCoroutine(SpawnMaze());
      }

      if (_lvlSettings.tetris)
      {
         StartCoroutine(SpawnTetrisPiece());
      }

      if (_lvlSettings.tetrisBoss)
      {
         StartCoroutine(SpawnTetrisEnemies());
      }

      CoinsBehaviour.Lifetime = _lvlSettings.moneyLife;

      StartCoroutine(SpawnMoney());

      if (availablePowerUps.Count != 0)
      {
         StartCoroutine(SpawnPowerUps());
      }


      Timer.instance.StartCounter();
   }


   #region Spawn Maze

   IEnumerator SpawnMaze()
   {
      yield return new WaitForSeconds(_timeBetweenSpawnMaze);

      Instantiate(_mazePrefab, obstacleSpawnPoint.position, Quaternion.identity);
      ObstacleBehaviour.speed = _lvlSettings.obstacleSpeed;
      StopSpawning();

   }

   #endregion

   #region Spawn fullScreenLines

   public override IEnumerator SpawnLines()
   {
      yield return new WaitForSeconds(timeBetweenSpawnLasers);

      for (int i = 0; i < _lvlSettings.linesCount; i++)
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

   #region Spawn Geometric Figures

   //rename this method with the name of the object you are spawning
   protected override IEnumerator SpawnGeometricFigures()
   {
      yield return new WaitForSeconds(timeBetweenSpawnsGeometricFigures);

      EnemyBehaviour objectToSpawn = _geometryToSpawn[Random.Range(0, _geometryToSpawn.Count)];
      objectToSpawn.UpdateSpeedBasedOnFigure(_geometryFiguresSpeed);
      Transform spawnPoint = spawningPoints[Random.Range(0, spawningPoints.Count - 1)];

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



   #region Spawn Tetris Enemies

   private IEnumerator SpawnTetrisEnemies()
   {
      yield return new WaitForSeconds(_timeBetweenSpawnTetrisEnemies);

      GameObject objectToSpawn = tetrisPrefabs[Random.Range(0, tetrisPrefabs.Length)];
      Transform spawnPoint = spawningPoints[Random.Range(0, spawningPoints.Count - 1)];

      //Set direction and Target 

      attentionSignBehaviour = Instantiate(enemyAlertSignPrefab, spawnPoint.position, spawnPoint.rotation)
         .GetComponent<AttentionSignBehaviour>();
      SetEnemyDirection(spawnPoint);
      attentionSignBehaviour.target =
         Instantiate(objectToSpawn, positionToSpawn, Quaternion.identity).GetComponent<Transform>();
      GameObject newTetrisObject = attentionSignBehaviour.target.gameObject;
      SetTetrisObject(newTetrisObject, objectToSpawn);
      newTetrisObject.GetComponent<EnemyBehaviour>().UpdateSpeedBasedOnFigure(_tetrisEnemiesSpeed);
      StartCoroutine(SpawnTetrisEnemies());
   }


   private void SetTetrisObject(GameObject newTetrisObject, GameObject tetrisPrefab)
   {

      newTetrisObject.AddComponent<Rigidbody2D>();
      newTetrisObject.GetComponent<Rigidbody2D>().gravityScale = 0;
      newTetrisObject.tag = "Enemy";
      newTetrisObject.layer = 3;

      switch (tetrisPrefab.name)
      {
         case "Cube":
            {
               newTetrisObject.AddComponent<Square>();
               newTetrisObject.GetComponent<BoxCollider2D>().isTrigger = false;
               break;
            }
         case "L_Down":
            {
               newTetrisObject.AddComponent<Hexagon>();
               newTetrisObject.GetComponent<EdgeCollider2D>().isTrigger = false;
               break;
            }

         case "L_Up":
            {
               newTetrisObject.AddComponent<Hexagon>();
               newTetrisObject.GetComponent<EdgeCollider2D>().isTrigger = false;
               break;
            }

         case "Line_UP":
            {
               newTetrisObject.AddComponent<Circle>();
               newTetrisObject.GetComponent<BoxCollider2D>().isTrigger = false;
               break;
            }

         default:
            {
               Debug.LogError("Tetris Object Undefined", this);
               break;
            }
      }


      newTetrisObject.GetComponent<EnemyBehaviour>().deadEffect = tetrisDestroyEffect;
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

   #region Spawn Tetris One Peace

   IEnumerator SpawnTetrisPiece()
   {
      yield return new WaitForSeconds(_timeBetweenSpawnMaze);

      ObstacleBehaviour.speed = _lvlSettings.obstacleSpeed;

      for (int tetrisIndex = 0; tetrisIndex < _lvlSettings.tetrisCount; tetrisIndex++)
      {
         GameObject newTetrisObject = Instantiate(_lvlSettings.obstaclePrefab,
              new Vector2(Random.Range(tetrisRange.x, tetrisRange.y), tetrisSpawnPoint.position.y)
              , Quaternion.identity);
         newTetrisObject.AddComponent<ObstacleBehaviour>();
         yield return new WaitForSeconds(1f);
      }

      yield return new WaitForSeconds(4f);

      StartCoroutine(SpawnTetrisPiece());
   }

   #endregion

   #region SpawnBossAttacks

   public void StartSecondAttack()
   {

      UniTask.Void(async () =>
      {
         int roundTime = 10;
         int geometricFigureIndex = 0;
         while (geometricFigureIndex < geometricFigures.Count)
         {
            EnemyBehaviour objectToSpawn = geometricFigures[geometricFigureIndex];
            objectToSpawn.UpdateSpeedBasedOnFigure(_geometryFiguresSpeed);
            objectToSpawn.SetTransformRight();

            Transform spawnPoint = spawningPoints[3];

            attentionSignBehaviour = Instantiate(enemyAlertSignPrefab, spawnPoint.position, spawnPoint.rotation)
               .GetComponent<AttentionSignBehaviour>();
            SetEnemyDirection(spawnPoint);
            attentionSignBehaviour.target =
               Instantiate(objectToSpawn, positionToSpawn, Quaternion.identity).GetComponent<Transform>();

            await UniTask.Delay(TimeSpan.FromSeconds(1));
            roundTime--;
            if (roundTime == 0)
            {
               roundTime = 10;
               geometricFigureIndex++;
            }
         }
      });
   }

   #endregion

}
