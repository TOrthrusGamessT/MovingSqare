using UnityEngine;

/// <summary>
/// Simple setup script for music-reactive background in Survive mode
/// This script provides a straightforward way to add music-reactive backgrounds to Survive scenes
/// </summary>
public class SurviveMusicReactive : MonoBehaviour
{
    [Header("Audio Configuration")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool autoFindAudioSource = true;

    [Header("Background Target")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private bool autoFindCamera = true;

    [Header("Beat Detection")]
    [SerializeField] [Range(0.05f, 0.3f)] private float beatSensitivity = 0.08f;
    [SerializeField] private float beatCooldown = 0.1f;
    
    [Header("Colors")]
    [SerializeField] private Color baseColor = new Color(0.02f, 0.02f, 0.04f, 1f);
    [SerializeField] private Color[] beatColors = new Color[]
    {
        new Color(0.2f, 0.03f, 0.03f, 1f),     // Dark Red
        new Color(0.15f, 0.1f, 0.03f, 1f),     // Dark Orange  
        new Color(0.12f, 0.12f, 0.03f, 1f),    // Dark Yellow
        new Color(0.08f, 0.03f, 0.15f, 1f),    // Dark Purple
        new Color(0.03f, 0.08f, 0.12f, 1f)     // Dark Blue
    };
    
    [Header("Visual Settings")]
    [SerializeField] private float colorTransitionSpeed = 8f;

    // Beat detection variables
    private float[] spectrumData = new float[512];
    private float[] previousSpectrumData = new float[512];
    private float lastBeatTime;
    private Color currentTargetColor;
    private int lastColorIndex = 0;

    // State
    private bool isActive = true;
    private bool isSetup = false;

    void Start()
    {
        SetupComponents();
        currentTargetColor = baseColor;
        if (targetCamera != null)
        {
            targetCamera.backgroundColor = baseColor;
        }
    }

    void SetupComponents()
    {
        // Auto-find AudioSource
        if (autoFindAudioSource && audioSource == null)
        {
            audioSource = FindObjectOfType<AudioSource>();
            
            if (audioSource == null)
            {
                // Check if SoundManager exists
                SoundManager soundManager = FindObjectOfType<SoundManager>();
                if (soundManager != null)
                {
                    audioSource = soundManager.GetComponent<AudioSource>();
                }
            }
            
            if (audioSource == null)
            {
                Debug.LogWarning("SurviveMusicReactive: No AudioSource found. Music-reactive background will not work.");
                return;
            }
        }

        // Auto-find Camera
        if (autoFindCamera && targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null)
            {
                targetCamera = FindObjectOfType<Camera>();
            }
            
            if (targetCamera == null)
            {
                Debug.LogWarning("SurviveMusicReactive: No Camera found. Background color changes will not work.");
                return;
            }
        }

        isSetup = audioSource != null && targetCamera != null;
        
        if (isSetup)
        {
            Debug.Log("SurviveMusicReactive: Setup complete! Ready for beat detection.");
        }
    }

    void Update()
    {
        if (!isActive || !isSetup || audioSource == null || targetCamera == null)
            return;

        // Detect beats
        DetectBeat();
        
        // Update camera color
        UpdateCameraColor();
    }

    void DetectBeat()
    {
        if (!audioSource.isPlaying)
            return;

        // Get spectrum data
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

        // Calculate energy in low-mid frequency range (good for beat detection)
        float energy = 0f;
        for (int i = 2; i < 20; i++)
        {
            energy += spectrumData[i];
        }

        // Calculate energy difference
        float previousEnergy = 0f;
        for (int i = 2; i < 20; i++)
        {
            previousEnergy += previousSpectrumData[i];
        }

        float energyDifference = energy - previousEnergy;

        // Check for beat
        if (energyDifference > beatSensitivity && Time.time - lastBeatTime > beatCooldown)
        {
            OnBeatDetected();
            lastBeatTime = Time.time;
        }

        // Store current data for next frame
        System.Array.Copy(spectrumData, previousSpectrumData, spectrumData.Length);
    }

    void OnBeatDetected()
    {
        // Change to next color
        lastColorIndex = (lastColorIndex + 1) % beatColors.Length;
        currentTargetColor = beatColors[lastColorIndex];

        // Schedule return to base color
        CancelInvoke(nameof(ReturnToBaseColor));
        Invoke(nameof(ReturnToBaseColor), 0.3f);
    }

    void ReturnToBaseColor()
    {
        currentTargetColor = baseColor;
    }

    void UpdateCameraColor()
    {
        if (targetCamera == null)
            return;

        // Smoothly transition to target color
        targetCamera.backgroundColor = Color.Lerp(
            targetCamera.backgroundColor, 
            currentTargetColor, 
            colorTransitionSpeed * Time.deltaTime
        );
    }

    // Public methods for external control
    public void SetActive(bool active)
    {
        isActive = active;
        if (!active && targetCamera != null)
        {
            currentTargetColor = baseColor;
        }
    }

    public void SetBeatSensitivity(float sensitivity)
    {
        beatSensitivity = Mathf.Clamp(sensitivity, 0.05f, 0.3f);
    }

    public void SetBaseColor(Color color)
    {
        baseColor = color;
        if (!isActive)
        {
            currentTargetColor = baseColor;
        }
    }

    public void AddBeatColor(Color color)
    {
        Color[] newColors = new Color[beatColors.Length + 1];
        System.Array.Copy(beatColors, newColors, beatColors.Length);
        newColors[beatColors.Length] = color;
        beatColors = newColors;
    }

    // Integration methods for Survive mode
    public void StartSurviveMode()
    {
        SetActive(true);
        
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        
        Debug.Log("Survive mode music-reactive background started!");
    }

    public void StopSurviveMode()
    {
        SetActive(false);
        Debug.Log("Survive mode music-reactive background stopped!");
    }

    // Method to test beat detection (for debugging)
    [ContextMenu("Test Beat")]
    public void TestBeat()
    {
        OnBeatDetected();
    }

    void OnValidate()
    {
        // Ensure we have at least one beat color
        if (beatColors == null || beatColors.Length == 0)
        {
            beatColors = new Color[] { Color.red, Color.yellow, Color.cyan };
        }
    }
}
