using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseAnimation : MonoBehaviour
{
    [SerializeField]
    private float animationTime;
    
    [SerializeField]
    private float backgroundFadeTime = 0.3f;

    [SerializeField] private RawImage skin;
    [SerializeField] private Image background;
    
    private void OnEnable()
    {
        StartCoroutine(PlayFullAnimation());
    }

    public IEnumerator PlayFullAnimation()
    {
        // Start with background at 0 opacity
        Color bgColor = background.color;
        bgColor.a = 0f;
        background.color = bgColor;
        
        // Fade background in from 0 to 1
        LeanTween.alpha(background.rectTransform, 1f, backgroundFadeTime);
        yield return new WaitForSeconds(backgroundFadeTime);
        
        // Wait for main animation duration
        yield return new WaitForSeconds(animationTime);
        
        // Fade background out from 1 to 0
        LeanTween.alpha(background.rectTransform, 0f, backgroundFadeTime);
        yield return new WaitForSeconds(backgroundFadeTime);
        
        gameObject.SetActive(false);
    }

    public IEnumerator StopAnimation()
    {
        yield return new WaitForSeconds(animationTime);
        gameObject.SetActive(false);
    }

    public void SetSkin(Sprite newSprite)
    {
        skin.texture = newSprite.texture;
    }
}
