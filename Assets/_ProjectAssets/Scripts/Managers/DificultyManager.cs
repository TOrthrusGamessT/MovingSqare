using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DificultyManager : MonoBehaviour
{
    [SerializeField, Range(0, 30)]
    private int currentLvl=1;
    private float lvlUpTimeDelay=10;
    public float lineTimeDelay=100, enemyTimeDelay=100, obstacleTimeDelay=100;
    public SpawnManagerSurvive spawnManagerSurvive;
    
    #region Singleton

    public static DificultyManager instance;

    private void Awake()
    {
        
        enemyTimeDelay=3;
        instance = FindObjectOfType<DificultyManager>();

        if (instance == null)
        {
            instance = this;
        }
        UpdateStats();
        StartCoroutine(LvlUp());
        Debug.Log("IM alive");
    }

    #endregion
    
    IEnumerator LvlUp(){
        yield return new WaitForSeconds(lvlUpTimeDelay); 
        Debug.Log("LvlUp");
        currentLvl++;
        UpdateStats();
        StartCoroutine(LvlUp());
    }

    void LvlReset(){
        currentLvl=0;
    }

    float GetCurrentLvl(){
        return currentLvl;
    }
    void UpdateStats(){
        if(currentLvl<10 || (currentLvl>20 && currentLvl%2==0)){
            UpdateEnemyDelay();
        }else{
            UpdateLineDelay();
        }
        if (currentLvl>30){
            UpdateObstacleDelay();
        }
    }
    void UpdateEnemyDelay(){
        enemyTimeDelay = enemyTimeDelay*0.85f;
    }
    void UpdateLineDelay(){
        if(lineTimeDelay==100){
            lineTimeDelay =5;
            spawnManagerSurvive.timeBetweenSpawnLasers=2;
            StartCoroutine(spawnManagerSurvive.SpawnLines());
        }
        if(currentLvl%2==0){
            spawnManagerSurvive.timeBetweenSpawnLasers++;
        }
        lineTimeDelay = lineTimeDelay*0.85f;
    }
    void UpdateObstacleDelay(){
        if(obstacleTimeDelay==100){
            obstacleTimeDelay = 30;
            StartCoroutine(spawnManagerSurvive.SpawnObstacle());
        }

    }
}
