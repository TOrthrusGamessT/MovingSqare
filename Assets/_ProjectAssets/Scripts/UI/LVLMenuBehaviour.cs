using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLMenuBehaviour : MonoBehaviour
{
    public RectTransform lvlWrapper;
    public RectTransform content;

    private int maxLvlReached;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("MaxLvlReached"))
        {
            PlayerPrefs.SetInt("MaxLvlReached", 0);
        }
        else
        {
            maxLvlReached = PlayerPrefs.GetInt("MaxLvlReached", 0);
        }

    }


    private void OnEnable()
    {
        for (int i = 0; i <= maxLvlReached; i++)
        {
            LVLButtonBehaviour lvlButtonBehaviour = Instantiate(lvlWrapper,
                content.position, Quaternion.identity, content).GetComponent<LVLButtonBehaviour>();
            lvlButtonBehaviour.Init((i + 1).ToString(), true);
        }

        for (int i = maxLvlReached + 1; i < 20; i++)
        {
            LVLButtonBehaviour lvlButtonBehaviour = Instantiate(lvlWrapper,
                content.position, Quaternion.identity, content).GetComponent<LVLButtonBehaviour>();
            lvlButtonBehaviour.Init((i + 1).ToString(), true);
        }
    }

    private void OnDisable()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }
}
