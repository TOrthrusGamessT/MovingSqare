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
    public ItemsPool itemsPool;
    public GameObject skinContainer;
    public GameObject powerUpContainer;
    public GameObject purchaseAnimation;
    public Transform contentContainer;
    public TextMeshProUGUI money;
    public int selectedSkin;

    [Header("Buttons")]
    public Texture2D unselectedTexture;
    public Texture2D selectTexture;

    [Header("Item Main Image")] 
    public Texture2D unBoughtImage;
    
    [SerializeField]
    private TMP_Text shopType;
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
        foreach (var skin in itemsPool.items)
        {
            if (skin.type == ElementType.Skin)
            {
                skinStatus.Add(skin.id, PlayerPrefs.HasKey($"unlockedSkin{skin.id}"));
            }
            
        }

        selectedSkin = PlayerPrefs.HasKey("currentSkin") ? PlayerPrefs.GetInt("currentSkin") : 0;
    }
    
  
    public void ChangeSelectedSkin(int skinID)
    {
        Skin skin = (Skin)shopElements[selectedSkin];
        skin.Unselect(unselectedTexture);
        skin = null;
        selectedSkin = skinID;
        SetBkSquares();
    }
   
    private void InitShopElements(ElementType elementType)
    {
        //header
        shopType.text = elementType.ToString();

        //create items
        foreach (var t in itemsPool.items)
        {
            if (t.type==elementType && elementType == ElementType.Skin)
            {
                shopElements.Add(Instantiate(skinContainer, contentContainer)
                .GetComponent<ShopItem>().Initialize(t,skinStatus[t.id], shopText));
            }
            if(t.type==elementType && elementType == ElementType.PowerUp)
            {
                shopElements.Add(Instantiate(powerUpContainer, contentContainer)
                    .GetComponent<ShopItem>().Initialize(t,skinStatus[t.id]));
            }
        }
        //choose selectedSkin
        if(elementType == ElementType.Skin)
        {
            shopElements[selectedSkin].GetComponent<Skin>().SetStatus(true);
            shopElements[selectedSkin].Select();
        }
    }
    
    private void SetBkSquares()
    {
        foreach (var square in bkSquares)
        {
            square.SetSkin();
        }
    }
    public void NextShopPage(){
        elementType++;
        if(System.Enum.GetValues(typeof(ElementType)).Length-1 < elementType.GetHashCode()){
            elementType =0;
        }
        DestroyAllShopElements();
        InitPlayerPrefs();
        InitShopElements(elementType);
    }
    public void PrevShopPage(){
        elementType--;
        if(elementType.GetHashCode()<0){
            elementType += System.Enum.GetValues(typeof(ElementType)).Length;
        }
        DestroyAllShopElements();
        InitPlayerPrefs();
        InitShopElements(elementType);
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
