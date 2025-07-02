using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    #region Singleton

    public static GameManager instance;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        instance = FindObjectOfType<GameManager>();

        if (instance == null)
        {
            instance = this;
        }
    }

    #endregion

    public static Action onGameOver;

    [SerializeField] private UIManagerGameRoom uiManager;
    [SerializeField] private Spawner spawnManagerSurvive;
    [SerializeField] private ItemsPool upgrades;
    [SerializeField] private SkinsData skins;
    [SerializeField] private int difficultySpeed;
    [SerializeField] private float extraMoneyIncrease;

    [Header("Music-Reactive Background")]
    [SerializeField] private bool enableMusicReactiveBackground = true;
    [SerializeField] private SurviveMusicReactive musicReactiveBackground;

    [HideInInspector]
    public Vector2 PlayerPosition => player.position;
    private Transform player;
    private bool alreadyOver;
    private static bool askedAd;
    private float moneyMultiplayer = 2;
    private int index;

    public Transform Player => player;


    private void OnEnable()
    {
        BossGameplay.OnBossAppear += StopCoroutine;
        BossGameplay.OnBossDisappear += StartCoroutine;
        onGameOver += StopCoroutine;
        Timer.onCounterEnd += StopCoroutine;

        AdsManager.onReviveADFinish += ResetAlreadyOver;
        AdsManager.onReviveADFinish += StartCoroutine;
        AdsManager.onReviveADFinish += SetViewAdTrue;
    }


    private void OnDisable()
    {
        BossGameplay.OnBossAppear -= StopCoroutine;
        BossGameplay.OnBossDisappear -= StartCoroutine;
        onGameOver -= StopCoroutine;
        Timer.onCounterEnd -= StopCoroutine;

        AdsManager.onReviveADFinish -= StartCoroutine;
        AdsManager.onReviveADFinish -= ResetAlreadyOver;
        AdsManager.onReviveADFinish -= SetViewAdTrue;
    }

    private void Start()
    {
        Application.targetFrameRate = Screen.currentResolution.refreshRate;

        player.GetComponent<PlayerManager>().InitPlayer(skins.skins[PlayerPrefs.GetInt("currentSkin")]);

        StartCoroutine(InitGame());

    }

    IEnumerator InitGame()
    {
        CoinsBehaviour.amount = (int)moneyMultiplayer;

        // Initialize music-reactive background
        if (enableMusicReactiveBackground)
        {
            InitializeMusicReactiveBackground();
        }

        yield return new WaitForSeconds(2f);
        spawnManagerSurvive.StartSpawning();

        StartCoroutine(IncreaseMoneyValue());
    }

    private void InitializeMusicReactiveBackground()
    {
        // Find or create music-reactive background
        if (musicReactiveBackground == null)
        {
            musicReactiveBackground = FindFirstObjectByType<SurviveMusicReactive>();
        }

        if (musicReactiveBackground == null)
        {
            // Create a new GameObject with the music-reactive background
            GameObject musicReactiveGO = new GameObject("Survive Music Reactive Background");
            musicReactiveBackground = musicReactiveGO.AddComponent<SurviveMusicReactive>();
            Debug.Log("Created SurviveMusicReactive background for Survive mode");
        }

        // Start the music-reactive background
        musicReactiveBackground.StartSurviveMode();
    }


    public void GameOver()
    {
        if (!alreadyOver)
        {
            // Stop music-reactive background
            if (musicReactiveBackground != null)
            {
                musicReactiveBackground.StopSurviveMode();
            }

            onGameOver?.Invoke();
            if (!askedAd)
            {
                uiManager.AdState();
            }
            else
            {
                ResetAd();
                uiManager.LoseState();
            }
            alreadyOver = !alreadyOver;
        }
    }

    public void ResetAlreadyOver()
    {
        alreadyOver = !alreadyOver;
    }

    private void SetViewAdTrue()
    {
        askedAd = true;
    }

    public void ResetAd()
    {
        if (askedAd)
            askedAd = false;
    }

    private void StopCoroutine()
    {
        StopAllCoroutines();
    }

    private void StartCoroutine()
    {
        StartCoroutine(IncreaseMoneyValue());
        
        // Restart music-reactive background if enabled
        if (enableMusicReactiveBackground && musicReactiveBackground != null)
        {
            musicReactiveBackground.StartSurviveMode();
        }
    }

    private IEnumerator IncreaseMoneyValue()
    {
        yield return new WaitForSeconds(extraMoneyIncrease);

        moneyMultiplayer += Mathf.Ceil(moneyMultiplayer / 10);
        CoinsBehaviour.amount = (int)moneyMultiplayer;

        StartCoroutine(IncreaseMoneyValue());
    }

    public void Pause()
    {
        Time.timeScale = 0;
    }

    public void Resume()
    {
        Time.timeScale = 1;
    }


}
