using UnityEngine;

/// <summary>
/// Prefab-ready music reactive background for Survive mode
/// Simply drop this prefab into your Survive scene and it will auto-configure itself
/// </summary>
public class SurviveMusicReactivePrefab : MonoBehaviour
{
    [Header("Auto-Setup Configuration")]
    [Tooltip("Colors used for beat detection in Survive mode")]
    public Color[] survivalBeatColors = new Color[]
    {
        new Color(0.25f, 0.04f, 0.04f, 1f),    // Dark Intense Red
        new Color(0.2f, 0.12f, 0.04f, 1f),     // Dark Orange
        new Color(0.15f, 0.15f, 0.04f, 1f),    // Dark Yellow
        new Color(0.12f, 0.04f, 0.2f, 1f),     // Dark Purple
        new Color(0.04f, 0.1f, 0.18f, 1f),     // Dark Blue
        new Color(0.15f, 0.04f, 0.12f, 1f)     // Dark Magenta
    };

    [Tooltip("Base camera color for Survive mode")]
    public Color survivalBaseColor = new Color(0.02f, 0.02f, 0.04f, 1f);

    [Header("Beat Detection Settings")]
    [Range(0.05f, 0.2f)]
    [Tooltip("How sensitive the beat detection is (lower = more sensitive)")]
    public float beatSensitivity = 0.08f;

    private SurviveMusicReactive musicReactive;

    void Awake()
    {
        SetupMusicReactiveBackground();
    }

    void SetupMusicReactiveBackground()
    {
        // Add the music reactive component if it doesn't exist
        musicReactive = GetComponent<SurviveMusicReactive>();
        if (musicReactive == null)
        {
            musicReactive = gameObject.AddComponent<SurviveMusicReactive>();
        }

        // Configure it with our settings
        ConfigureSettings();

        Debug.Log("Survive Music Reactive Background Prefab setup complete!");
    }

    void ConfigureSettings()
    {
        if (musicReactive == null) return;

        // Set the base color
        musicReactive.SetBaseColor(survivalBaseColor);

        // Set beat sensitivity
        musicReactive.SetBeatSensitivity(beatSensitivity);

        // Add all our survival colors
        foreach (Color color in survivalBeatColors)
        {
            musicReactive.AddBeatColor(color);
        }
    }

    void Start()
    {
        // Start the system
        if (musicReactive != null)
        {
            musicReactive.StartSurviveMode();
        }
    }

    // Called when the prefab settings are changed in the inspector
    void OnValidate()
    {
        if (musicReactive != null)
        {
            ConfigureSettings();
        }
    }

    // Public methods for external control
    public void EnableMusicReactive(bool enable)
    {
        if (musicReactive != null)
        {
            musicReactive.SetActive(enable);
        }
    }

    public void ChangeBeatSensitivity(float sensitivity)
    {
        beatSensitivity = Mathf.Clamp(sensitivity, 0.05f, 0.2f);
        if (musicReactive != null)
        {
            musicReactive.SetBeatSensitivity(beatSensitivity);
        }
    }
}
