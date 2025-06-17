using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BgSquare : MonoBehaviour
{

    public Image myImg;

    public void Initialize(Sprite sprite)
    {
        myImg.sprite = sprite;
        myImg.color = new Color(1, 1, 1, 0.5f);
    }

}
