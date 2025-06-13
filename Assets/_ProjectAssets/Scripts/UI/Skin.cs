using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Skin : MonoBehaviour
{
    [SerializeField]
    private RawImage skinImageShow;
    [SerializeField]
    private RawImage buttonSprite;
    [SerializeField]
    private TextMeshProUGUI priceText;
    [SerializeField]
    private Color selectedColor, unselectedColor;
    [SerializeField]
    private GameObject selectedVFX;
    [SerializeField]
    private RawImage bg;
    [SerializeField]
    private Transform _upgradesContainer;
    [SerializeField]
    private Slider speedSlider;
    [SerializeField]
    private Slider sizeSlider;
    [SerializeField]
    private GameObject[] lifeIcons;
    [SerializeField]
    private Texture2D selectedTexture; // Add this
    [SerializeField]
    private Texture2D unselectedTexture; // Add this

    private SkinData skinData;
    private string description;

    public void Initialize(SkinData data, bool bought, bool isSelected = false)
    {
        skinData = data;
        SetDescription();
        SetStatus(bought);
        if(isSelected)Select();
    }

    public void SetStatus(bool bought)
    {
        AddListener(bought);

        SetDescription();
        if (bought)
        {
            buttonSprite.texture = ShopManager.instance.unselectedTexture;
            skinImageShow.texture = skinData.sprite.texture;
            priceText.color = Color.white;
            priceText.text = "";
            _upgradesContainer.gameObject.SetActive(true);
           
        }
        else
        {
            skinImageShow.texture = ShopManager.instance.unBoughtImage;
            priceText.color = Color.white;
            priceText.text = skinData.price.ToString();
            _upgradesContainer.gameObject.SetActive(false);
        }
    }

    private void SetDescription()
    {
        // If you want to use bonuses in the description, you can format them here
         // Update sliders with skin bonuses
        if (speedSlider != null)
        {
            speedSlider.value = skinData.speedBonus;
            speedSlider.interactable = false; // Make read-only
        }
        if (sizeSlider != null)
        {
            sizeSlider.value = skinData.sizeBonus;
            sizeSlider.interactable = false; // Make read-only
        }

        // Show correct number of life icons
        UpdateLifeIcons();
    }

    private void UpdateLifeIcons()
    {
        if (lifeIcons == null) return;
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            lifeIcons[i].SetActive(i < skinData.lives);
        }
    }

    public void Buy()
    {
        int currentMoney = PlayerPrefs.GetInt("Money");
        if (currentMoney >= skinData.price)
        {
            SetDescription();
            currentMoney -= skinData.price;
            PlayerPrefs.SetInt("Money", currentMoney);
            ShopManager.instance.SetMoney(currentMoney);
            ShopManager.instance.purchaseAnimation.SetActive(true);
            ShopManager.instance.purchaseAnimation.GetComponent<PurchaseAnimation>().SetSkin(skinData.sprite);
            ShopManager.instance.StartCoroutine(AnimatePurchase());
            PlayerPrefs.SetInt("unlockedSkin" + skinData.id, 1);
            ClearListener();
            AddListener(true);
            _upgradesContainer.gameObject.SetActive(true);
            SetDescription();
            priceText.text = description;
        }
        else
        {
            NotEnoughMoney();
        }
    }

    public void NotEnoughMoney()
    {
        Color red = Color.red;
        Color white = Color.white;

        bg.DOColor(red, 0.5f).OnComplete(() => bg.DOColor(white, 0.5f));
        skinImageShow.DOColor(red, 0.5f).OnComplete(() => skinImageShow.DOColor(white, 0.5f));
    }

    IEnumerator AnimatePurchase()
    {
        LeanTween.scale(skinImageShow.gameObject, Vector3.zero, 1f);
        yield return new WaitForSeconds(1f);
        skinImageShow.texture = skinData.sprite.texture;
        priceText.color = Color.white;
        LeanTween.scale(skinImageShow.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack);
    }

    public void Unselect(Texture2D unselectedTex = null)
    {
        Texture2D texToUse = unselectedTex != null ? unselectedTex : unselectedTexture;
        bg.texture = texToUse;
        priceText.color = unselectedColor;
        selectedVFX.SetActive(false);
        buttonSprite.texture = texToUse;
    }

    public void Select()
    {
        PlayerPrefs.SetInt("currentSkin", skinData.id);
        if (selectedTexture != null)
        {
            bg.texture = selectedTexture;
        }
        selectedVFX.SetActive(true);
    }

    private void AddListener(bool bought)
    {
        var btn = buttonSprite.gameObject.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        if (bought)
        {
            btn.onClick.AddListener(() =>
            {
                Select();
                ShopManager.instance.ChangeSelectedSkin(skinData.id);
            });
        }
        else
        {
            btn.onClick.AddListener(Buy);
        }
    }

    private void ClearListener()
    {
        buttonSprite.GetComponent<Button>().onClick.RemoveAllListeners();
    }

}
