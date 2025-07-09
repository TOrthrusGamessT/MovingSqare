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
        Debug.Log("Initializing Google Mobile Ads SDK...");
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            Debug.Log("Google Mobile Ads SDK initialized successfully");
            foreach (var adapterStatus in initStatus.getAdapterStatusMap())
            {
                Debug.Log($"Adapter: {adapterStatus.Key}, Status: {adapterStatus.Value.InitializationState}, Description: {adapterStatus.Value.Description}");
            }
            LoadRewardedAd();
        });
    }


    public static void InitReviveAD()
    {
        Debug.Log("InitReviveAD called");
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            Debug.Log("Rewarded ad is ready for revive");
            value = true;
            RegisterEventHandlers(rewardedAd);
            ShowReviveAD(true);
        }
        else
        {
            Debug.LogWarning($"Rewarded ad not ready for revive. Ad null: {rewardedAd == null}, CanShow: {rewardedAd?.CanShowAd()}");
            LoadRewardedAd();
        }
    }

    public static void InitDoubleCoinAD()
    {
        Debug.Log("InitDoubleCoinAD called");
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            Debug.Log("Rewarded ad is ready for double coin");
            value = false;
            RegisterEventHandlers(rewardedAd);
            ShowReviveAD(false);
        }
        else
        {
            Debug.LogWarning($"Rewarded ad not ready for double coin. Ad null: {rewardedAd == null}, CanShow: {rewardedAd?.CanShowAd()}");
            LoadRewardedAd();
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

        Debug.Log($"[AdsManager] Loading rewarded ad with ID: {_adUnitId}");

        // Create our request to load the ad.
        var adRequest = new AdRequest();

        // Send the request to load the ad.
        RewardedAd.Load(_adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError($"[AdsManager] Rewarded ad failed to load: {error}");
                    if (error != null)
                    {
                        int code = error.GetCode();
                        Debug.LogError($"[AdsManager] Error Code: {code}, Message: {error.GetMessage()}");
                        Debug.LogError($"[AdsManager] Domain: {error.GetDomain()}, Cause: {error.GetCause()}");

                        if (code == 3)
                        {
                            // Error 3 = NO FILL
                            Debug.LogWarning("[AdsManager] No ad inventory available (error code 3). Waiting longer before retrying.");

                            // Optional: Notify UI that ads are unavailable
                            // OnAdUnavailable?.Invoke();

                            if (instance != null)
                            {
                                instance.StartCoroutine(RetryLoadAd(30f)); // Wait 30 seconds
                            }

                            return;
                        }
                    }

                    // For other errors, use normal retry delay
                    if (instance != null)
                    {
                        instance.StartCoroutine(RetryLoadAd());
                    }
                    return;
                }

                Debug.Log($"[AdsManager] Rewarded ad loaded successfully. ResponseInfo: {ad.GetResponseInfo()}");

                rewardedAd = ad;
                RegisterReloadHandler(ad);
            });
    }

    private static IEnumerator RetryLoadAd(float delaySeconds = 5f)
    {
        Debug.Log($"[AdsManager] Retrying ad load in {delaySeconds} seconds...");
        yield return new WaitForSeconds(delaySeconds);
        LoadRewardedAd();
    }

    private static IEnumerator RetryLoadAd()
    {
        Debug.Log("Retrying ad load in 5 seconds...");
        yield return new WaitForSeconds(5f);
        LoadRewardedAd();
    }

    private static void ShowReviveAD(bool value)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            Debug.Log("Before Showing Revive AD");

            FireBase.LogCustomEvent("revive_ad_shown", new System.Collections.Generic.Dictionary<string, object>
            {
                { "ad_type", value ? "revive" : "double_coin" }
            });
            rewardedAd.Show((Reward reward) =>
            {
                MainThreadDispatcher.Enqueue(() =>
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

                    FireBase.LogCustomEvent("revive_ad_finished", new System.Collections.Generic.Dictionary<string, object>
                    {
                        { "ad_type", value ? "revive" : "double_coin" }
                    });
                    LoadRewardedAd();
                });
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
            LoadRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Ad failed to open: " + error);
            LoadRewardedAd();
        };
    }

    // Test methods to verify AdMob functionality
    public static void TestAdMobConnection()
    {
        Debug.Log("Testing AdMob connection...");

        // Test ad request
        var testRequest = new AdRequest();
        Debug.Log($"Test AdRequest created successfully");

        // Check if we can get device info
        Debug.Log($"Device info available: {SystemInfo.deviceModel}");
        Debug.Log($"Internet reachability: {Application.internetReachability}");
    }

    public static bool IsAdReady()
    {
        bool isReady = rewardedAd != null && rewardedAd.CanShowAd();
        Debug.Log($"Ad ready status: {isReady}");
        return isReady;
    }
}
