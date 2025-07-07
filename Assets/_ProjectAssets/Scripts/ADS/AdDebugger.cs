using UnityEngine;
using GoogleMobileAds.Api;

public class AdDebugger : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== AD DEBUGGER START ===");
        
        // Check internet connection
        Debug.Log($"Internet Reachability: {Application.internetReachability}");
        
        // Check if we're on Android
        Debug.Log($"Platform: {Application.platform}");
        
        // Test AdMob connection
        AdsManager.TestAdMobConnection();
        
        // Wait a bit then check ad status
        Invoke(nameof(CheckAdStatus), 5f);
    }
    
    void CheckAdStatus()
    {
        Debug.Log("=== CHECKING AD STATUS ===");
        bool isReady = AdsManager.IsAdReady();
        Debug.Log($"Ad ready after 5 seconds: {isReady}");
        
        // Try to test initialization
        Debug.Log("Testing ad initialization...");
        AdsManager.InitReviveAD();
    }
    
    void Update()
    {
        // Press Space to test ad loading
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("=== MANUAL AD TEST ===");
            AdsManager.TestAdMobConnection();
            AdsManager.InitReviveAD();
        }
    }
}
