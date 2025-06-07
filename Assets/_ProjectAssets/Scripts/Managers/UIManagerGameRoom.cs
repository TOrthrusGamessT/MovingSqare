using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManagerGameRoom : MonoBehaviour
{

    #region Singleton

    public static UIManagerGameRoom instance;

    private void Awake()
    {
        instance = FindObjectOfType<UIManagerGameRoom>();
        if (instance == null)
        {
            instance = this;
        }
    }

    #endregion

    [SerializeField] private TextMeshProUGUI highScore;
    [SerializeField] private TextMeshProUGUI moneyCollected;
    [SerializeField] private CanvasGroup mainUI;
    [SerializeField] private GameObject movingZone;
    [SerializeField] private GameObject moneyParent;
    [SerializeField] private GameObject revive;
    [SerializeField] private GameObject next;
    [SerializeField] private GameObject doubleCoin;
    [SerializeField] private List<GameObject> playerLives;
    [SerializeField] private GameObject livesParent;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject maineMenu;
    [SerializeField] private TextMeshProUGUI highScorePauseMenu;
    [SerializeField] private TextMeshProUGUI moneyCollectedPauseMenu;

    private int index = 0;
    private bool canWatchDoubleCoinAD = true;
    private bool canWatchReviveAD = true;


    private void OnEnable()
    {
        AdsManager.onReviveADFinish += () =>
        {
            Debug.LogWarning("Yooo entered in ReviveAdFinish");
            canWatchReviveAD = false;
            ResetMainMenu();
        };
        AdsManager.onDoubleMoneyADFinish += () =>
        {
            RemoveDoubleCoinButton();
            canWatchDoubleCoinAD = false;
        };

        PlayerLife.onPlayerGotLife += IncreaseLife;

        Timer.onCounterEnd += FinishLvlState;
    }

    private void OnDisable()
    {
        AdsManager.onReviveADFinish -= () =>
        {
            Debug.LogWarning("Yooo entered in ReviveAdFinish");
            canWatchReviveAD = false;
            ResetMainMenu();
        };
        AdsManager.onDoubleMoneyADFinish -= () =>
        {
            RemoveDoubleCoinButton();
            canWatchDoubleCoinAD = false;
        };

        PlayerLife.onPlayerGotLife -= IncreaseLife;
        Timer.onCounterEnd -= FinishLvlState;
    }


    private void Start()
    {
        SetBkSize();
    }

    private void SetBkSize()
    {
        var sr = movingZone.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        movingZone.transform.localScale = new Vector3(1, 1, 1);

        var width = sr.sprite.bounds.size.x;
        var height = sr.sprite.bounds.size.y;

        var worldScreenHeight = Camera.main.orthographicSize * 2f;
        var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        movingZone.transform.localScale = new Vector2((float)worldScreenWidth / width,
            (float)worldScreenHeight / height);

    }


    public void LoseState()
    {
        maineMenu.SetActive(true);
        UpdateScoreUI();

        mainUI.gameObject.SetActive(true);
        revive.SetActive(canWatchReviveAD);
        next.SetActive(false);
        doubleCoin.SetActive(canWatchDoubleCoinAD);

        FadeInEffect();
    }

    public void FinishLvlState()
    {
        maineMenu.SetActive(true);
        UpdateScoreUI();

        mainUI.gameObject.SetActive(true);
        revive.SetActive(false);
        next.SetActive(true);
        doubleCoin.SetActive(canWatchDoubleCoinAD);

        FadeInEffect();
    }

    public void AdState()
    {
        maineMenu.SetActive(true);
        UpdateScoreUI();
        AdListener();

        revive.SetActive(canWatchReviveAD);
        doubleCoin.SetActive(canWatchDoubleCoinAD);
        mainUI.gameObject.SetActive(true);

        FadeInEffect();
    }


    private void RemoveDoubleCoinButton()
    {
        doubleCoin.SetActive(false);
        DataManager.MoneyCollected = DataManager.MoneyCollected * 2;
        UpdateScoreUI();
    }

    private void FadeInEffect()
    {
        LeanTween.value(0, 1, 1f).setOnUpdate(value =>
        {
            mainUI.alpha = value;
        }).setEaseInQuad().setDelay(0.5f);
    }

    private void FadeOutEffect()
    {
        LeanTween.value(1, 0, 1f).setOnUpdate(value =>
        {
            mainUI.alpha = value;

        }).setEaseInQuad().setOnComplete(() =>
        {
            mainUI.gameObject.SetActive(false);
        });
    }

    private void UpdateScoreUI()
    {
        highScore.text = $"High Score\n{PlayerPrefs.GetInt("HighScore")}";
        moneyCollected.text = $"Score\n{DataManager.MoneyCollected}";
    }




    private void AdListener()
    {
        revive.GetComponent<Button>().onClick.AddListener(AdsManager.InitReviveAD);
        doubleCoin.GetComponent<Button>().onClick.AddListener(AdsManager.InitDoubleCoinAD);
    }

    private void ResetMainMenu()
    {
        revive.GetComponent<Button>().onClick.RemoveAllListeners();
        LeanTween.value(1, 0, 1f).setOnUpdate(value => mainUI.alpha = value).setEaseInQuad()
            .setOnComplete(() => mainUI.gameObject.SetActive(false));
    }


    void IncreaseLife(int life)
    {
        while (index < life - 1)
        {
            index++;
            playerLives[index].SetActive(true);
        }

        LeanTween.value(1, 0.5f, 0.7f).setOnUpdate(value =>
        {
            livesParent.transform.localScale = Vector3.one * value;
        }).setEasePunch();

    }

    public void ShowPauseMenu()
    {
        UpdateScoreUIPauseMenu();
        mainUI.gameObject.SetActive(true);
        mainUI.alpha = 1;
        pauseMenu.SetActive(true);
    }

    public void HidePauseMenu()
    {
        mainUI.alpha = 0;
        mainUI.gameObject.SetActive(false);
        pauseMenu.SetActive(false);
    }

    public void DecreaseLife()
    {
        Debug.LogError("Decreasing life");
        playerLives[index].SetActive(false);
        index--;
    }

    private void UpdateScoreUIPauseMenu()
    {
        highScorePauseMenu.text = $"High Score\n{PlayerPrefs.GetInt("HighScore")}";
        moneyCollectedPauseMenu.text = $"Score\n{DataManager.MoneyCollected}";
    }

}
