using System;
using TMPro;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static int _money;

    public static int MoneyCollected
    {
        get => _money;
        set => _money += value;
    }


    private void OnEnable()
    {
        GameManager.onGameOver += SaveHighScore;
        CoinsBehaviour.onCoinCollected += UpdateMoney;
    }

    private void OnDisable()
    {
        GameManager.onGameOver -= SaveHighScore;
        CoinsBehaviour.onCoinCollected -= UpdateMoney;
    }

    void Start()
    {
        _money = 0;
    }

    private void UpdateMoney(int amount)
    {
        _money += amount;
    }


    private void SaveHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore");
        int score = _money;

        if (score > highScore)
            PlayerPrefs.SetInt("HighScore", score);
    }

    public void SaveMoney()
    {
        int amount = PlayerPrefs.GetInt("Money");
        amount += _money;
        PlayerPrefs.SetInt("Money", amount);
        PlayerPrefs.Save();
    }

}
