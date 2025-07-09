using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuAnimation : MonoBehaviour
{
    public TextMeshProUGUI[] textFade;
    public TextMeshProUGUI money;
    public GameObject main, shop, topBar, startButton, gameModeMenu;
    public SceneLoader SceneLoader;
    public GameObject lvlMenu;

    private void Start()
    {
        InitPlayerPrefs();
        InitAnimations();
    }


    private void InitPlayerPrefs()
    {
        money.text = PlayerPrefs.HasKey("Money") ? PlayerPrefs.GetInt("Money").ToString() : "0";
        if (!PlayerPrefs.HasKey("currentSkin"))
        {
            PlayerPrefs.SetInt("currentSkin", 0);
        }
    }

    private void InitAnimations()
    {

        for (int i = 0; i < textFade.Length; i++)
        {
            textFade[i].color = new Color32(255, 255, 255, 1);
            int index = i;
            LeanTween.value(0, 1, 1f).setOnUpdate(value =>
            {
                Color c = textFade[index].color;
                c.a = value;
                textFade[index].color = c;
            });
        }

    }


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

    public void HideLvlMenu()
    {
        UniTask.Void(async () =>
        {
            SceneLoader.RandomTransition();

            await UniTask.Delay(TimeSpan.FromSeconds(1f));

            lvlMenu.SetActive(false);
        });
        FireBase.LogCustomEvent("lvl_menu_closed", new System.Collections.Generic.Dictionary<string, object>
        {

        });


    }

    public void GoShop()
    {
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
    IEnumerator AnimateTransition(float from, float to, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (waitTime > 0)
        {
            shop.SetActive(false);
        }
        LeanTween.value(from, to, 0.5f).setOnUpdate(value =>
        {
            Vector3 temp = new Vector3(value, value, 1);
            main.transform.localScale = temp;
            //topBar.transform.localScale = temp;
        });

    }
    IEnumerator ShowHideShop(bool state, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        shop.SetActive(state);
    }

    public void ChoseGameMode()
    {
        LeanTween.rotate(startButton, new Vector3(0, 0, 360), 2f);
        StartCoroutine(ShowGameModeMenu());

    }
    public IEnumerator ShowGameModeMenu()
    {
        yield return new WaitForSeconds(0.1f);
        gameModeMenu.GetComponent<HorizontalLayoutGroup>().spacing = -650f;
        gameModeMenu.SetActive(true);
        startButton.SetActive(false);
        foreach (Transform child in gameModeMenu.transform)
        {
            //LeanTween.rotate(child.gameObject, new Vector3(0, 0, 360), 2f);
            LeanTween.rotateLocal(child.gameObject, new Vector3(0, 0, 360), 0.7f);
        }
        LeanTween.value(-650f, 20f, 0.7f).setOnUpdate(value =>
        {
            gameModeMenu.GetComponent<HorizontalLayoutGroup>().spacing = value;
        });

        FireBase.LogCustomEvent("game_mode_menu_opened", new System.Collections.Generic.Dictionary<string, object>
        {
            // Add any parameters you want to log with this event
        });
    }
}
