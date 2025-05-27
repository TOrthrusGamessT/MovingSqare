using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class AdsManager : MonoBehaviour
{


    #region Selected Id Based On Device

    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    private static string _adUnitId = "ca-app-pub-5781212170655183/3816693134";
#elif UNITY_IPHONE
  private static string _adUnitId = "ca-app-pub-5781212170655183/4374278096";
#else
    private static string _adUnitId = "unused";
#endif


    #endregion

    #region Singleton

    public static AdsManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    public static Action onReviveADFinish;
    public static Action onDoubleMoneyADFinish;


    private static bool value;
    private static RewardedAd rewardedAd;

    private void Start()
    {

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            LoadRewardedAd();
        });

    }


    public static void InitReviveAD()
    {
        if (rewardedAd.CanShowAd())
        {
            value = true;
            RegisterEventHandlers(rewardedAd);
            ShowReviveAD(true);
        }
    }

    public static void InitDoubleCoinAD()
    {
        if (rewardedAd.CanShowAd())
        {
            value = false;
            RegisterEventHandlers(rewardedAd);
            ShowReviveAD(false);
        }
    }

    /// <summary>
    /// Loads the rewarded ad.
    /// </summary>
    private static void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(_adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {

                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                rewardedAd = ad;
            });
    }

    private static void ShowReviveAD(bool value)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            Debug.Log("Before Showing Revive AD");

            rewardedAd.Show((Reward reward) =>
            {
                if (value)
                {
                    Debug.Log("Revive AD Finish");
                    onReviveADFinish?.Invoke();
                }
                else
                {
                    Debug.Log("Double AD Finish");
                    onDoubleMoneyADFinish?.Invoke();
                }
                LoadRewardedAd();
            });
        }
    }

    private static void RegisterReloadHandler(RewardedAd ad)
    {
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            LoadRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.Log("In OnAdFullScreenContentFailed");
            LoadRewardedAd();
        };
    }


    private static void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log("In OnAdPaid");
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {

            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.LogWarning("In OnAdFullScreenContentOpened");
            Debug.Log("Rewarded ad full screen content opened.");

            /*
            if (value)
            {
                Debug.Log("Revive AD Finish");
                onReviveADFinish?.Invoke();
            }
            else
            {
                Debug.Log("Double AD Finish");
                onDoubleMoneyADFinish?.Invoke();
            }
            */

        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.LogWarning("In OnAdFullScreenContentClosed");
            ad.Destroy();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError(error);
        };
    }
}
