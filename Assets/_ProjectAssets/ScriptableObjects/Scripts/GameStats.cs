using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GameStats", menuName = "Custom/GameStats")]
public class GameStats : ScriptableObject
{
   public Vector2 enemySizeRange;
   public Vector2 enemySpeedRange;

   public float spawnPowerUpsTime;
   public float spawnMoneyTime;
   public float spawnEnemyTime;

   public int moneyLifeTime;
}
