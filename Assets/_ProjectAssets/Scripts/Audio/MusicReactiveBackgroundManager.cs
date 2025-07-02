using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Music-reactive background manager that integrates with the existing game systems.
/// This script can be easily added to any scene and will automatically find and react to music.
/// Works with Camera backgrounds, UI Images, and SpriteRenderers.
/// </summary>
public class MusicReactiveBackgroundManager : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool autoSetup = true;
    [SerializeField] private bool useMainCamera = true;
    [SerializeField] private bool findBackgroundSprite = true;
    [SerializeField] private bool findUIBackground = true;
    
    [Header("Manual Setup (if not using auto)")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private Camera backgroundCamera;
    [SerializeField] private SpriteRenderer backgroundSprite;
    [SerializeField] private Image backgroundImage; // For UI backgrounds
    
    [Header("Beat Detection")]
    [Range(0.01f, 0.5f)]
    [SerializeField] private float beatSensitivity = 0.01f;
    [Range(0.1f, 1f)]
    [SerializeField] private float beatCooldown = 0.6f;
    [Range(1f, 10f)]
    [SerializeField] private float energyMultiplier = 8f;
    
    [Header("Color Configuration")]
    [SerializeField] private Color defaultColor = new Color(0.02f, 0.02f, 0.04f, 1f);
    [SerializeField] private Color[] musicColors = {
        new Color(0.15f, 0.02f, 0.02f, 1f),    // Dark Red
        new Color(0.02f, 0.05f, 0.15f, 1f),    // Dark Blue
        new Color(0.02f, 0.12f, 0.04f, 1f),    // Dark Green
        new Color(0.12f, 0.08f, 0.02f, 1f),    // Dark Orange/Brown
        new Color(0.1f, 0.02f, 0.1f, 1f),      // Dark Purple
        new Color(0.02f, 0.08f, 0.08f, 1f)     // Dark Teal
    };
    
    [Header("Animation")]
    [Range(0.5f, 8f)]
    [SerializeField] private float colorTransitionSpeed = 3f;
    [Range(0.2f, 2f)]
    [SerializeField] private float returnToDefaultSpeed = 0.8f;
    [Range(0f, 1f)]
    [SerializeField] private float colorIntensity = 0.7f;
    
    [Header("Advanced")]
    [SerializeField] private bool enableOnlyInGameplay = false;
    [SerializeField] private bool respectMusicSettings = true;
    [SerializeField] private bool debugMode = false;
    
    // Private variables
    private float[] audioSamples;
    private float currentEnergy;
    private float lastBeatTime;
    private Color currentTargetColor;
    private Color currentDisplayColor;
    private int currentColorIndex;
    private bool isActive;
    private float energyAverage;
    private float[] energyHistory;
    private int energyHistoryIndex;
    
    private void Start()
    {
        InitializeSystem();
    }
    
    private void InitializeSystem()
    {
        audioSamples = new float[64];
        energyHistory = new float[20]; // Short history for beat detection
        currentTargetColor = defaultColor;
        currentDisplayColor = defaultColor;
        
        if (autoSetup)
        {
            AutoSetupComponents();
        }
        
        // Set initial background color
        ApplyBackgroundColor(defaultColor);
        
        // Validate setup
        ValidateSetup();
    }
    
    private void AutoSetupComponents()
    {
        // Find AudioSource (prioritize SoundManager, then any AudioSource)
        if (musicSource == null)
        {
            if (SoundManager.instance != null)
            {
                musicSource = SoundManager.instance.GetComponent<AudioSource>();
            }
            
            if (musicSource == null)
            {
                var menuManager = FindObjectOfType<MenuManager>();
                if (menuManager != null)
                {
                    musicSource = menuManager.GetComponent<AudioSource>();
                }
            }
            
            if (musicSource == null)
            {
                musicSource = FindObjectOfType<AudioSource>();
            }
        }
        
        // Find Camera
        if (useMainCamera && backgroundCamera == null)
        {
            backgroundCamera = Camera.main;
            if (backgroundCamera == null)
            {
                backgroundCamera = FindObjectOfType<Camera>();
            }
        }
        
        // Find background sprite
        if (findBackgroundSprite && backgroundSprite == null)
        {
            var bgController = FindObjectOfType<BackGroundImageController>();
            if (bgController != null)
            {
                backgroundSprite = bgController.GetComponent<SpriteRenderer>();
            }
            
            if (backgroundSprite == null)
            {
                // Find any SpriteRenderer that might be a background
                var spriteRenderers = FindObjectsOfType<SpriteRenderer>();
                foreach (var sr in spriteRenderers)
                {
                    if (sr.gameObject.name.ToLower().Contains("background") || 
                        sr.gameObject.name.ToLower().Contains("bg") ||
                        sr.sortingOrder < 0)
                    {
                        backgroundSprite = sr;
                        break;
                    }
                }
            }
        }
        
        // Find UI background
        if (findUIBackground && backgroundImage == null)
        {
            var images = FindObjectsOfType<Image>();
            foreach (var img in images)
            {
                if (img.gameObject.name.ToLower().Contains("background") || 
                    img.gameObject.name.ToLower().Contains("bg"))
                {
                    backgroundImage = img;
                    break;
                }
            }
        }
    }
    
    private void ValidateSetup()
    {
        if (musicSource == null)
        {
            Debug.LogWarning("MusicReactiveBackgroundManager: No AudioSource found! Music-reactive background will not work.");
            enabled = false;
            return;
        }
        
        if (backgroundCamera == null && backgroundSprite == null && backgroundImage == null)
        {
            Debug.LogWarning("MusicReactiveBackgroundManager: No background target found! Please assign a Camera, SpriteRenderer, or Image.");
            enabled = false;
            return;
        }
        
        isActive = true;
        
        if (debugMode)
        {
            Debug.Log($"MusicReactiveBackgroundManager initialized with: Camera={backgroundCamera != null}, Sprite={backgroundSprite != null}, Image={backgroundImage != null}");
        }
    }
    
    private void Update()
    {
        if (!isActive) return;
        
        // Check if we should be active based on settings
        if (respectMusicSettings)
        {
            bool musicEnabled = true;
            
            if (SoundManager.instance != null)
            {
                musicEnabled = SoundManager.instance.musicOn;
            }
            else if (FindObjectOfType<MenuManager>() != null)
            {
                musicEnabled = FindObjectOfType<MenuManager>().musicOn;
            }
            
            if (!musicEnabled)
            {
                currentTargetColor = defaultColor;
                UpdateBackgroundColor();
                return;
            }
        }
        
        // Check if music is playing
        if (musicSource == null || !musicSource.isPlaying)
        {
            currentTargetColor = defaultColor;
            UpdateBackgroundColor();
            return;
        }
        
        AnalyzeMusicAndUpdateColors();
    }
    
    private void AnalyzeMusicAndUpdateColors()
    {
        // Get audio data
        musicSource.GetOutputData(audioSamples, 0);
        
        // Calculate current energy
        float energySum = 0f;
        for (int i = 0; i < audioSamples.Length; i++)
        {
            energySum += audioSamples[i] * audioSamples[i];
        }
        currentEnergy = energySum / audioSamples.Length;
        
        // Update energy history for beat detection
        UpdateEnergyHistory();
        
        // Detect beats
        if (DetectBeat())
        {
            OnBeatDetected();
        }
        else
        {
            // Return to default color immediately when no beat
            currentTargetColor = defaultColor;
        }
        
        UpdateBackgroundColor();
    }
    
    private void UpdateEnergyHistory()
    {
        energyHistory[energyHistoryIndex] = currentEnergy;
        energyHistoryIndex = (energyHistoryIndex + 1) % energyHistory.Length;
        
        // Calculate average energy
        float sum = 0f;
        for (int i = 0; i < energyHistory.Length; i++)
        {
            sum += energyHistory[i];
        }
        energyAverage = sum / energyHistory.Length;
    }
    
    private bool DetectBeat()
    {
        // Simple beat detection: current energy is significantly higher than average
        float threshold = energyAverage + (beatSensitivity * energyMultiplier);
        bool isBeat = currentEnergy > threshold && Time.time - lastBeatTime > beatCooldown;
        
        if (debugMode && isBeat)
        {
            Debug.Log($"Beat detected! Energy: {currentEnergy:F4}, Threshold: {threshold:F4}");
        }
        
        return isBeat;
    }
    
    private void OnBeatDetected()
    {
        lastBeatTime = Time.time;
        
        // Cycle through colors
        currentColorIndex = (currentColorIndex + 1) % musicColors.Length;
        Color beatColor = musicColors[currentColorIndex];
        
        // Apply intensity based on energy level
        float intensity = Mathf.Clamp01((currentEnergy * energyMultiplier) * colorIntensity);
        currentTargetColor = Color.Lerp(defaultColor, beatColor, intensity);
        
    }
    
    private void UpdateBackgroundColor()
    {
        // Apply color immediately without lerping
        currentDisplayColor = currentTargetColor;
        ApplyBackgroundColor(currentDisplayColor);
    }
    
    private void ApplyBackgroundColor(Color color)
    {
        // Apply to camera background
        if (backgroundCamera != null)
        {
            backgroundCamera.backgroundColor = color;
        }
        
        // Apply to sprite renderer
        if (backgroundSprite != null)
        {
            backgroundSprite.color = color;
        }
        
        // Apply to UI image
        if (backgroundImage != null)
        {
            backgroundImage.color = color;
        }
    }
    
    // Public methods for external control
    public void SetActive(bool active)
    {
        isActive = active;
        if (!active)
        {
            ApplyBackgroundColor(defaultColor);
        }
    }
    
    public void SetBeatSensitivity(float sensitivity)
    {
        beatSensitivity = Mathf.Clamp01(sensitivity);
    }
    
    public void SetDefaultColor(Color color)
    {
        defaultColor = color;
    }
    
    public void AddMusicColor(Color color)
    {
        var newColors = new Color[musicColors.Length + 1];
        System.Array.Copy(musicColors, newColors, musicColors.Length);
        newColors[musicColors.Length] = color;
        musicColors = newColors;
    }
    
    public void SetColorIntensity(float intensity)
    {
        colorIntensity = Mathf.Clamp01(intensity);
    }
    
    // Integration events
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            ApplyBackgroundColor(defaultColor);
        }
    }
    
    private void OnDisable()
    {
        ApplyBackgroundColor(defaultColor);
    }
}
