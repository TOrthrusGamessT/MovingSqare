using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShopItem : MonoBehaviour
{
    public ElementType elementType;
    public Effects[] effects;
    public int upgradeStage = 0;

    public abstract ShopItem Initialize(Item shopItem, bool status);
    public abstract ShopItem Initialize(Item shopItem, bool status, ShopText shopText);
    public abstract void Select();

    public abstract void Buy();

    public abstract void NotEnoughMoney();



}
