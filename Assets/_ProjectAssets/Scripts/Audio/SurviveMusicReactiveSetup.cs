using UnityEngine;

[System.Serializable]
public class SurviveMusicReactiveSetup : MonoBehaviour
{
    [Header("Music-Reactive Background Setup for Survive Mode")]
    [Tooltip("The Music Reactive Background Manager that will handle beat detection")]
    public MusicReactiveBackgroundManager musicReactiveManager;
    
    [Header("Audio Setup")]
    [Tooltip("AudioSource for music playback - will auto-find if not assigned")]
    public AudioSource musicAudioSource;
    
    [Header("Background Target")]
    [Tooltip("Camera for background color changes - will auto-find if not assigned")]
    public Camera targetCamera;
    
    [Header("Survive Mode Configuration")]
    [Tooltip("Beat sensitivity for intense survival gameplay")]
    [Range(0.05f, 0.3f)]
    public float surviveBeatSensitivity = 0.08f;
    
    [Tooltip("Colors for survival mode beats")]
    public Color[] survivalColors = new Color[]
    {
        new Color(0.2f, 0.03f, 0.03f, 1f),     // Dark Red
        new Color(0.18f, 0.1f, 0.03f, 1f),     // Dark Orange
        new Color(0.15f, 0.15f, 0.03f, 1f),    // Dark Yellow
        new Color(0.1f, 0.03f, 0.18f, 1f),     // Dark Purple
        new Color(0.03f, 0.08f, 0.15f, 1f)     // Dark Cyan
    };
    
    [Tooltip("Base camera color for Survive mode")]
    public Color survivalBaseColor = new Color(0.02f, 0.02f, 0.04f, 1f);

    void Start()
    {
        SetupMusicReactiveBackground();
    }

    void SetupMusicReactiveBackground()
    {
        // Find or create the MusicReactiveBackgroundManager
        if (musicReactiveManager == null)
        {
            GameObject musicReactiveGO = new GameObject("Music Reactive Background");
            musicReactiveManager = musicReactiveGO.AddComponent<MusicReactiveBackgroundManager>();
        }

        // Auto-find AudioSource if not assigned
        if (musicAudioSource == null)
        {
            musicAudioSource = FindObjectOfType<AudioSource>();
            if (musicAudioSource == null)
            {
                // Create an AudioSource if none exists
                GameObject audioGO = new GameObject("Survive Music AudioSource");
                musicAudioSource = audioGO.AddComponent<AudioSource>();
                musicAudioSource.loop = true;
                musicAudioSource.playOnAwake = true;
                
                // Try to load a default music clip from Resources
                AudioClip defaultMusic = Resources.Load<AudioClip>("Audio/SurviveMusic");
                if (defaultMusic != null)
                {
                    musicAudioSource.clip = defaultMusic;
                    musicAudioSource.Play();
                }
                
                Debug.Log("Created AudioSource for Survive mode. Assign a music clip in the inspector.");
            }
        }

        // Auto-find Camera if not assigned
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null)
            {
                targetCamera = FindObjectOfType<Camera>();
            }
            
            if (targetCamera == null)
            {
                // Create a camera if none exists
                GameObject cameraGO = new GameObject("Survive Camera");
                targetCamera = cameraGO.AddComponent<Camera>();
                targetCamera.tag = "MainCamera";
                targetCamera.transform.position = new Vector3(0, 0, -10);
                
                // Setup for 2D
                targetCamera.orthographic = true;
                targetCamera.orthographicSize = 5;
                targetCamera.backgroundColor = survivalBaseColor;
                
                Debug.Log("Created Camera for Survive mode.");
            }
        }

        // Configure the music reactive manager
        ConfigureMusicReactiveManager();
    }

    void ConfigureMusicReactiveManager()
    {
        if (musicReactiveManager == null) return;

        // Set audio source
        var audioField = typeof(MusicReactiveBackgroundManager).GetField("audioSource", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (audioField != null)
        {
            audioField.SetValue(musicReactiveManager, musicAudioSource);
        }

        // Set target camera
        var cameraField = typeof(MusicReactiveBackgroundManager).GetField("targetCamera", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (cameraField != null)
        {
            cameraField.SetValue(musicReactiveManager, targetCamera);
        }

        // Configure colors and sensitivity
        var colorsField = typeof(MusicReactiveBackgroundManager).GetField("musicColors", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (colorsField != null)
        {
            colorsField.SetValue(musicReactiveManager, survivalColors);
        }

        var sensitivityField = typeof(MusicReactiveBackgroundManager).GetField("beatSensitivity", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (sensitivityField != null)
        {
            sensitivityField.SetValue(musicReactiveManager, surviveBeatSensitivity);
        }

        var baseColorField = typeof(MusicReactiveBackgroundManager).GetField("defaultColor", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (baseColorField != null)
        {
            baseColorField.SetValue(musicReactiveManager, survivalBaseColor);
        }

        // Set camera background to base color
        if (targetCamera != null)
        {
            targetCamera.backgroundColor = survivalBaseColor;
        }

        Debug.Log("Music-Reactive Background configured for Survive mode!");
    }

    // Public methods for runtime control
    public void SetBeatSensitivity(float sensitivity)
    {
        surviveBeatSensitivity = Mathf.Clamp(sensitivity, 0.05f, 0.3f);
        if (musicReactiveManager != null)
        {
            musicReactiveManager.SetBeatSensitivity(surviveBeatSensitivity);
        }
    }

    public void AddSurvivalColor(Color color)
    {
        if (musicReactiveManager != null)
        {
            musicReactiveManager.AddMusicColor(color);
        }
    }

    public void SetActive(bool active)
    {
        if (musicReactiveManager != null)
        {
            musicReactiveManager.SetActive(active);
        }
    }

    // Method to call when starting survival mode
    public void StartSurvivalMode()
    {
        if (musicAudioSource != null && !musicAudioSource.isPlaying)
        {
            musicAudioSource.Play();
        }
        
        SetActive(true);
        
        Debug.Log("Survival mode music-reactive background started!");
    }

    // Method to call when ending survival mode
    public void StopSurvivalMode()
    {
        SetActive(false);
        
        if (targetCamera != null)
        {
            targetCamera.backgroundColor = survivalBaseColor;
        }
        
        Debug.Log("Survival mode music-reactive background stopped!");
    }
}
