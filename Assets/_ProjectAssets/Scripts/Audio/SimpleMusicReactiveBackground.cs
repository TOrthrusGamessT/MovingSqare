using UnityEngine;

/// <summary>
/// Simplified music-reactive background color script that easily integrates with existing systems.
/// This version focuses on simplicity and ease of use with minimal configuration required.
/// </summary>
public class SimpleMusicReactiveBackground : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool autoFindAudioSource = true;
    
    [Header("Target (Choose One)")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private SpriteRenderer targetSpriteRenderer;
    [SerializeField] private BackGroundImageController backgroundController; // Integration with existing system
    
    [Header("Colors")]
    [SerializeField] private Color baseColor = new Color(0.1f, 0.1f, 0.1f, 1f);
    [SerializeField] private Color beatColor1 = Color.red;
    [SerializeField] private Color beatColor2 = Color.blue;
    [SerializeField] private Color beatColor3 = Color.green;
    
    [Header("Settings")]
    [Range(0.01f, 1f)]
    [SerializeField] private float sensitivity = 0.1f;
    [Range(0.1f, 5f)]
    [SerializeField] private float colorSpeed = 2f;
    [Range(0.1f, 2f)]
    [SerializeField] private float returnSpeed = 0.5f;
    
    // Private variables
    private float[] audioSamples = new float[64];
    private Color currentColor;
    private Color targetColor;
    private float audioEnergy;
    private float lastBeatTime;
    private int colorIndex;
    private Color[] beatColors;
    
    private void Start()
    {
        Initialize();
    }
    
    private void Initialize()
    {
        // Auto-find AudioSource if needed
        if (autoFindAudioSource && audioSource == null)
        {
            audioSource = SoundManager.instance?.GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = FindObjectOfType<AudioSource>();
        }
        
        // Auto-find Camera if no targets specified
        if (targetCamera == null && targetSpriteRenderer == null && backgroundController == null)
        {
            targetCamera = Camera.main;
        }
        
        // Setup colors
        beatColors = new Color[] { beatColor1, beatColor2, beatColor3 };
        currentColor = baseColor;
        targetColor = baseColor;
        
        // Set initial color
        SetBackgroundColor(baseColor);
        
        // Validation
        if (audioSource == null)
        {
            Debug.LogWarning("SimpleMusicReactiveBackground: No AudioSource found!");
            enabled = false;
        }
    }
    
    private void Update()
    {
        if (audioSource == null || !audioSource.isPlaying)
        {
            // Return to base color when no music
            targetColor = baseColor;
        }
        else
        {
            AnalyzeAudioAndUpdateColor();
        }
        
        // Smooth color transition
        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * colorSpeed);
        SetBackgroundColor(currentColor);
    }
    
    private void AnalyzeAudioAndUpdateColor()
    {
        // Get audio data
        audioSource.GetOutputData(audioSamples, 0);
        
        // Calculate energy
        float sum = 0f;
        for (int i = 0; i < audioSamples.Length; i++)
        {
            sum += audioSamples[i] * audioSamples[i];
        }
        audioEnergy = sum / audioSamples.Length;
        
        // Beat detection - simple threshold method
        if (audioEnergy > sensitivity && Time.time - lastBeatTime > 0.2f)
        {
            OnBeat();
            lastBeatTime = Time.time;
        }
        else
        {
            // Gradually return to base color
            targetColor = Color.Lerp(targetColor, baseColor, Time.deltaTime * returnSpeed);
        }
    }
    
    private void OnBeat()
    {
        // Cycle through beat colors
        colorIndex = (colorIndex + 1) % beatColors.Length;
        
        // Set target color with intensity based on audio energy
        float intensity = Mathf.Clamp01(audioEnergy * 10f);
        targetColor = Color.Lerp(baseColor, beatColors[colorIndex], intensity);
    }
    
    private void SetBackgroundColor(Color color)
    {
        // Apply color to target component
        if (targetCamera != null)
        {
            targetCamera.backgroundColor = color;
        }
        else if (targetSpriteRenderer != null)
        {
            targetSpriteRenderer.color = color;
        }
        else if (backgroundController != null)
        {
            // If using custom background controller, modify its sprite renderer
            var spriteRenderer = backgroundController.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }
        }
    }
    
    // Public methods for runtime control
    public void SetSensitivity(float newSensitivity)
    {
        sensitivity = Mathf.Clamp01(newSensitivity);
    }
    
    public void SetBaseColor(Color color)
    {
        baseColor = color;
    }
    
    public void SetBeatColors(Color color1, Color color2, Color color3)
    {
        beatColor1 = color1;
        beatColor2 = color2;
        beatColor3 = color3;
        beatColors = new Color[] { color1, color2, color3 };
    }
}
