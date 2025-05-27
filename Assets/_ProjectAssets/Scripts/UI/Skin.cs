using System.Xml.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;

public class Skin : ShopItem
{
    private Sprite itemSprite;
    [SerializeField]
    private RawImage skinImageShow;
    [SerializeField]
    private RawImage buttonSprite;
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private Color selectedColor, unselectedColor;
    [SerializeField]
    private GameObject selectedVFX;
    [SerializeField]
    private RawImage margin;

    private String description;
    private int price;
    private int id;
    private ElementType type;

    public override ShopItem Initialize(Item shopItem, bool status, ShopText shopText)
    {
        elementType = ElementType.Skin;
        effects = shopItem.effects;
        type = shopItem.type;
        id = shopItem.id;
        price = type == ElementType.Skin ? effects[0].price : effects[PlayerPrefs.GetInt(effects[0].name)].price;

        itemSprite = shopItem.sprite;

        SetDesctiption(shopText);
        SetStatus(status);
        return this;
    }

    public override ShopItem Initialize(Item shopItem, bool status) { return null; }


    public void SetStatus(bool status)
    {
        AddListener(status);
        if (status)
        {
            //Unselect Case
            buttonSprite.texture = ShopManager.instance.unselectedTexture;
            skinImageShow.texture = itemSprite.texture;
            text.color = new Color32(255, 255, 255, 255);
            text.text = description;
        }
        else
        {
            //Need to be bought case
            skinImageShow.texture = ShopManager.instance.unBoughtImage;
            text.color = new Color32(255, 255, 255, 255);
            text.text = price.ToString();
        }
    }

    private void SetDesctiption(ShopText shopText)
    {
        foreach (EffectTypeString current in shopText.effectTypeString)
        {
            if (current.type == effects[0].effect)
            {
                description = effects[0].value.ToString() + current.text;
            }
        }
    }


    public override void Buy()
    {
        int currentMoney = PlayerPrefs.GetInt("Money");
        if (currentMoney >= price)
        {
            SetDesctiption(ShopManager.instance.shopText);
            currentMoney -= price;
            PlayerPrefs.SetInt("Money", currentMoney);
            ShopManager.instance.SetMoney(currentMoney);
            ShopManager.instance.purchaseAnimation.SetActive(true);
            ShopManager.instance.purchaseAnimation.GetComponent<PurchaseAnimation>().SetSkin(itemSprite);
            StartCoroutine(AnimatePurchase());
            PlayerPrefs.SetInt("unlockedSkin" + id, 1);
            ClearListener();
            AddListener(true);
            text.text = description;
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


    IEnumerator AnimatePurchase()
    {
        // StartCoroutine(ColorAnimation());
        LeanTween.scale(skinImageShow.gameObject, new Vector3(0, 0, 0), 1f);
        yield return new WaitForSeconds(1f);
        skinImageShow.texture = itemSprite.texture;
        text.color = new Color32(255, 255, 255, 255);
        LeanTween.scale(skinImageShow.gameObject, new Vector3(1, 1, 1), 0.5f).setEase(LeanTweenType.easeOutBack);
    }

    public void Unselect(Texture2D unselectedTexture)
    {
        margin.color = unselectedColor;
        text.color = unselectedColor;
        selectedVFX.SetActive(false);
        buttonSprite.texture = unselectedTexture;
    }

    public override void Select()
    {
        margin.color = selectedColor;
        text.color = selectedColor;
        selectedVFX.SetActive(true);
        ShopManager.instance.selectedSkin = id;
        buttonSprite.texture = ShopManager.instance.selectTexture;
        PlayerPrefs.SetInt("currentSkin", id);
    }

    private void AddListener(bool status)
    {
        if (status)
        {
            buttonSprite.gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                ShopManager.instance.ChangeSelectedSkin(id);
                Select();
            });
        }
        else
        {
            buttonSprite.gameObject.GetComponent<Button>().onClick.AddListener(Buy);
        }
    }


    private void ClearListener()
    {
        buttonSprite.GetComponent<Button>().onClick.RemoveAllListeners();
    }
}
