using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PowerUp : ShopItem
{
    [SerializeField] private RawImage skinImageShow;
    [SerializeField] private RawImage margin;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button button;
    [SerializeField] private ParticleSystem buyVFX;


    private ElementType type;

    public override ShopItem Initialize(Item shopItem, bool status)
    {
        elementType = ElementType.PowerUp;
        effects = shopItem.effects;
        type = shopItem.type;

        upgradeStage = PlayerPrefs.HasKey(effects[0].name) ? PlayerPrefs.GetInt(effects[0].name) : 0;

        skinImageShow.texture = shopItem.sprite.texture;

        SetStatus();

        return this;
    }

    public override ShopItem Initialize(Item shopItem, bool status, ShopText shopText) { return null; }
    private void SetStatus()
    {
        if (upgradeStage < 3)
        {
            text.text = effects[upgradeStage].price.ToString();
            AddListener();
        }
        else
        {
            text.text = "MAX";
        }
    }

    public override void Select()
    {
        if (upgradeStage >= 3)
        {
            ClearListener();
        }
    }

    public override void Buy()
    {
        int currentMoney = PlayerPrefs.GetInt("Money");
        if (currentMoney >= effects[upgradeStage].price)
        {
            buyVFX.Play();
            currentMoney -= Int32.Parse(text.text);
            PlayerPrefs.SetInt("Money", currentMoney);
            ShopManager.instance.SetMoney(currentMoney);

            upgradeStage++;

            text.text = upgradeStage == 3 ? "MAX" : effects[upgradeStage].price.ToString();

            PlayerPrefs.SetInt(effects[0].name, upgradeStage);
            Select();
        }
        else
        {
            NotEnoughMoney();
        }
    }

    public override void NotEnoughMoney()
    {
        Color originalMarginColor = margin.color;
        Color originalSkinColor = skinImageShow.color;
        Color red = Color.red;
        Color white = Color.white;


        margin.DOColor(red, 0.5f).OnComplete(() =>
        {

            margin.DOColor(white, 0.5f);
        });

        skinImageShow.DOColor(red, 0.5f).OnComplete(() =>
        {
            skinImageShow.DOColor(white, 0.5f);
        });
    }


    private void AddListener()
    {
        if (upgradeStage < 3)
        {
            button.onClick.AddListener(Buy);
        }
    }

    private void ClearListener()
    {
        button.onClick.RemoveAllListeners();
    }
}
