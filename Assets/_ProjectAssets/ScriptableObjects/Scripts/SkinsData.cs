using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkinData 
{
    public int id;
    public Sprite sprite;
    public Texture2D trailTexture;
    public bool unlocked;
    public int price;

    [Range(1, 3)]
    public int lives=1;

    [Range(1f, 2f)]
    public float speedBonus=1f;
    [Range(1f, 2f)]
    public float sizeBonus=1f;

   
}

[CreateAssetMenu(fileName = "SkinsData", menuName = "Custom/SkinsData")]
public class SkinsData : ScriptableObject
{
    public List<SkinData> skins;
}
