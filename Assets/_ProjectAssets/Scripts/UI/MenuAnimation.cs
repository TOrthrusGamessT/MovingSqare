using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuAnimation : MonoBehaviour
{
    public TextMeshProUGUI money;
    public GameObject  shopButton, lvlButton;
    public SceneLoader SceneLoader;
    public GameObject lvlMenu, shopMenu;

    private Coroutine shopButtonAnimationCoroutine;

    private void Start()
    {
        InitPlayerPrefs();
        CheckAndAnimateShopButton();
    }

    //TODO: I dont like it here, needs to be replaced
    private void InitPlayerPrefs()
    {
        money.text = PlayerPrefs.HasKey("Money") ? PlayerPrefs.GetInt("Money").ToString() : "0";
        if (!PlayerPrefs.HasKey("currentSkin"))
        {
            PlayerPrefs.SetInt("currentSkin", 0);
        }
    }

    private void CheckAndAnimateShopButton()
    {
        int currentMoney = PlayerPrefs.GetInt("Money", 0);
        bool tutorialPassed = PlayerPrefs.GetInt("TutorialPassed", 0) == 1;

        if (currentMoney > 0 && !tutorialPassed)
        {
            StartShopButtonAnimation();
        }
    }

    private void StartShopButtonAnimation()
    {
        if (shopButtonAnimationCoroutine != null)
        {
            StopCoroutine(shopButtonAnimationCoroutine);
        }
        shopButtonAnimationCoroutine = StartCoroutine(AnimateShopButton());
    }

    private void StopShopButtonAnimation()
    {
        if (shopButtonAnimationCoroutine != null)
        {
            StopCoroutine(shopButtonAnimationCoroutine);
            shopButtonAnimationCoroutine = null;
            shopButton.transform.localScale = Vector3.one; // Reset scale
        }
    }

    private IEnumerator AnimateShopButton()
    {
        Vector3 originalScale = shopButton.transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;

        while (true)
        {
            // Scale up
            float elapsedTime = 0f;
            float duration = 0.5f;
            
            while (elapsedTime < duration)
            {
                shopButton.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Scale down
            elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                shopButton.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(0.5f); // Pause between animations
        }
    }

    //used on Level button
    public void ShowLvlMenu()
    {
        UniTask.Void(async () =>
        {
            SceneLoader.RandomTransition();

            await UniTask.Delay(TimeSpan.FromSeconds(1f));

            lvlMenu.SetActive(true);
        });

        FireBase.LogCustomEvent("lvl_menu_opened", new System.Collections.Generic.Dictionary<string, object>
        {

        });
    }
    // Used on Level menu close button
    public void HideLvlMenu()
    {
        UniTask.Void(async () =>
        {
            SceneLoader.RandomTransition();

            await UniTask.Delay(TimeSpan.FromSeconds(1f));

            lvlMenu.SetActive(false);
        });
        FireBase.LogCustomEvent("lvl_menu_closed", new System.Collections.Generic.Dictionary<string, object>
        {});
    }

    // Used on Shop button
    public void GoShop()
    {
        // Mark tutorial as passed when shop is opened
        if (PlayerPrefs.GetInt("TutorialPassed", 0) == 0)
        {
            PlayerPrefs.SetInt("TutorialPassed", 1);
            PlayerPrefs.Save();
            StopShopButtonAnimation();
        }

        SceneLoader.RandomTransition();
        //StartCoroutine(AnimateTransition(1, 30,0));
        StartCoroutine(ShowHideShop(true, 1f));

        FireBase.LogCustomEvent("shop_opened", new System.Collections.Generic.Dictionary<string, object>
        {
            // Add any parameters you want to log with this event
        });
    }

    public void LeaveShop()
    {
        SceneLoader.RandomTransition();
        // StartCoroutine(AnimateTransition(30, 1,0.1f));
        StartCoroutine(ShowHideShop(false, 1f));
        FireBase.LogCustomEvent("shop_closed", new System.Collections.Generic.Dictionary<string, object>
        {
            // Add any parameters you want to log with this event
        });
    }
    IEnumerator ShowHideShop(bool state, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        shopMenu.SetActive(state);
    }
}
