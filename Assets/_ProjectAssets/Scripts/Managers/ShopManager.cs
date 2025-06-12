using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ShopManager : MonoBehaviour
{

    #region Singleton

    public static ShopManager instance;

    private void Awake()
    {
        instance = FindObjectOfType<ShopManager>();
        if (instance == null)
        {
            instance = this;
        }
    }

    #endregion
    public ShopText shopText;
    public ItemsPool upgradesPool;
    public SkinsData skinsData;
    public GameObject skinContainer;
    public GameObject powerUpContainer;
    public GameObject purchaseAnimation;
    public Transform contentContainer;
    public TextMeshProUGUI money;
    public int selectedSkin;

    [Header("Buttons")]
    public Texture2D unselectedTexture;
    public Texture2D selectedTexture;

    [Header("TopButtons")]
    public GameObject skinsOn;
    public GameObject upgradesOn;

    [Header("Item Main Image")] 
    public Texture2D unBoughtImage;
    private readonly List<ShopItem> shopElements = new ();
    private  List<BgMovement> bkSquares = new();
    private Dictionary<int,bool> skinStatus = new();
    private ElementType elementType;



    void Start()
    {
        bkSquares = FindObjectsOfType<BgMovement>().ToList();
        elementType = ElementType.Skin;
        InitPlayerPrefs();
        InitShopElements(elementType);
    }
    

    private void InitPlayerPrefs()
    {
        foreach (var skinData in skinsData.skins)
        {
            skinStatus.Add(skinData.id, PlayerPrefs.HasKey($"unlockedSkin{skinData.id}"));   
        }

        selectedSkin = PlayerPrefs.HasKey("currentSkin") ? PlayerPrefs.GetInt("currentSkin") : 0;
    }
    
  
    public void ChangeSelectedSkin(int skinID)
    {
        Skin skin = contentContainer.transform.GetChild(selectedSkin).GetComponent<Skin>();
        skin.Unselect(unselectedTexture);
        selectedSkin = skinID;
        SetBkSquares();
    }

    public void SetShop(int type)
    {
        elementType = (ElementType)type;
        skinsOn.SetActive(type == 0);
        upgradesOn.SetActive(type == 1);
        DestroyAllShopElements();
        InitPlayerPrefs();
        InitShopElements(elementType);
   }
    private void InitShopElements(ElementType elementType)
    {
        //header
        if (elementType == ElementType.Skin)
        {
            foreach (var skin in skinsData.skins)
            {
                Instantiate(skinContainer, contentContainer)
                .GetComponent<Skin>().Initialize(skin, skinStatus[skin.id], skin.id == selectedSkin);

            }
        }
        else
        {
            
            foreach (var t in upgradesPool.items)
            {
               
                    shopElements.Add(Instantiate(powerUpContainer, contentContainer)
                        .GetComponent<ShopItem>().Initialize(t, skinStatus[t.id]));
                
            }
            
        }
    }
    
    private void SetBkSquares()
    {
        foreach (var square in bkSquares)
        {
            square.SetSkin();
        }
    }
   
    private void DestroyAllShopElements(){
        shopElements.Clear();
        skinStatus.Clear();
        foreach (Transform child in contentContainer) {
	        GameObject.Destroy(child.gameObject);
        }
    }

    public void SetMoney(int cash)
    {
        if(cash<1000){
            money.text =cash.ToString();
        }else{
            cash = cash/1000;
            money.text = cash +"k";
        }
    }
}
