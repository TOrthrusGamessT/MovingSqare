using System.Collections;
using UnityEngine;

/// <summary>
/// Music-reactive background color script that analyzes the currently playing music
/// and changes the background color based on detected beats or audio energy.
/// Can work with Camera background color or SpriteRenderer background objects.
/// </summary>
public class MusicReactiveBackground : MonoBehaviour
{
    [Header("Audio Analysis Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private int sampleDataLength = 64;
    [SerializeField] private float beatThreshold = 0.15f;
    [SerializeField] private float energyDecayRate = 2.0f;
    [SerializeField] private float beatCooldown = 0.3f;
    
    [Header("Color Settings")]
    [SerializeField] private bool useCameraBackground = true;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private SpriteRenderer backgroundSpriteRenderer;
    [SerializeField] private Color baseColor = Color.black;
    [SerializeField] private Color[] beatColors = { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.cyan };
    [SerializeField] private float colorTransitionSpeed = 5.0f;
    [SerializeField] private float colorIntensityMultiplier = 2.0f;
    [SerializeField] private float colorReturnSpeed = 1.0f;
    
    [Header("Beat Detection Method")]
    [SerializeField] private BeatDetectionMethod detectionMethod = BeatDetectionMethod.EnergyThreshold;
    [SerializeField] private bool useFrequencyBands = true;
    [SerializeField] private float[] frequencyBandMultipliers = { 1.2f, 1.0f, 0.8f, 0.6f }; // Bass, Low-Mid, High-Mid, Treble
    
    [Header("Visual Effects")]
    [SerializeField] private bool enablePulseEffect = true;
    [SerializeField] private float pulseStrength = 0.3f;
    [SerializeField] private bool enableColorCycling = false;
    [SerializeField] private float colorCycleSpeed = 1.0f;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    
    public enum BeatDetectionMethod
    {
        EnergyThreshold,
        FrequencyAnalysis,
        AmplitudeSpikes,
        SpectrumPeaks
    }
    
    // Private variables
    private float[] audioSamples;
    private float[] spectrumData;
    private float[] frequencyBands;
    private float currentAudioEnergy;
    private float previousAudioEnergy;
    private float averageEnergy;
    private float instantEnergy;
    private float beatTimer;
    private Color currentColor;
    private Color targetColor;
    private int currentBeatColorIndex;
    private float colorCycleTimer;
    private bool beatDetected;
    
    // Energy calculation variables
    private const int energyHistorySize = 43; // ~1 second at 60fps for beat detection
    private float[] energyHistory;
    private int energyHistoryIndex;
    private float energyVariance;
    
    private void Start()
    {
        InitializeAudioAnalysis();
        InitializeColorSystem();
        
        // Auto-find components if not assigned
        if (audioSource == null)
            audioSource = FindObjectOfType<AudioSource>();
        
        if (useCameraBackground && targetCamera == null)
            targetCamera = Camera.main;
        
        if (!useCameraBackground && backgroundSpriteRenderer == null)
            backgroundSpriteRenderer = FindObjectOfType<SpriteRenderer>();
        
        // Validate setup
        if (audioSource == null)
        {
            Debug.LogWarning("MusicReactiveBackground: No AudioSource found! Please assign one.");
            enabled = false;
            return;
        }
        
        if (useCameraBackground && targetCamera == null)
        {
            Debug.LogWarning("MusicReactiveBackground: No Camera found for background color changes!");
            enabled = false;
            return;
        }
        
        if (!useCameraBackground && backgroundSpriteRenderer == null)
        {
            Debug.LogWarning("MusicReactiveBackground: No SpriteRenderer found for background color changes!");
            enabled = false;
            return;
        }
    }
    
    private void InitializeAudioAnalysis()
    {
        audioSamples = new float[sampleDataLength];
        spectrumData = new float[sampleDataLength];
        frequencyBands = new float[4];
        energyHistory = new float[energyHistorySize];
        energyHistoryIndex = 0;
    }
    
    private void InitializeColorSystem()
    {
        currentColor = baseColor;
        targetColor = baseColor;
        currentBeatColorIndex = 0;
        colorCycleTimer = 0f;
        
        // Set initial background color
        SetBackgroundColor(baseColor);
    }
    
    private void Update()
    {
        if (audioSource == null || !audioSource.isPlaying)
        {
            // Return to base color when no music is playing
            targetColor = baseColor;
            UpdateBackgroundColor();
            return;
        }
        
        AnalyzeAudio();
        DetectBeats();
        UpdateColors();
        UpdateBackgroundColor();
        
        if (showDebugInfo)
            DisplayDebugInfo();
    }
    
    private void AnalyzeAudio()
    {
        // Get audio data
        audioSource.GetOutputData(audioSamples, 0);
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);
        
        // Calculate current energy
        float sum = 0f;
        for (int i = 0; i < audioSamples.Length; i++)
        {
            sum += audioSamples[i] * audioSamples[i];
        }
        currentAudioEnergy = sum / audioSamples.Length;
        
        // Calculate spectrum energy
        float spectrumSum = 0f;
        for (int i = 0; i < spectrumData.Length; i++)
        {
            spectrumSum += spectrumData[i];
        }
        
        // Update energy history for beat detection
        UpdateEnergyHistory();
        
        // Calculate frequency bands
        if (useFrequencyBands)
            CalculateFrequencyBands();
        
        previousAudioEnergy = currentAudioEnergy;
    }
    
    private void UpdateEnergyHistory()
    {
        energyHistory[energyHistoryIndex] = currentAudioEnergy;
        energyHistoryIndex = (energyHistoryIndex + 1) % energyHistorySize;
        
        // Calculate average energy
        float sum = 0f;
        float varianceSum = 0f;
        for (int i = 0; i < energyHistorySize; i++)
        {
            sum += energyHistory[i];
        }
        averageEnergy = sum / energyHistorySize;
        
        // Calculate variance for beat detection
        for (int i = 0; i < energyHistorySize; i++)
        {
            float diff = energyHistory[i] - averageEnergy;
            varianceSum += diff * diff;
        }
        energyVariance = varianceSum / energyHistorySize;
    }
    
    private void CalculateFrequencyBands()
    {
        // Divide spectrum into frequency bands
        // Band 0: 20-60Hz (Bass)
        // Band 1: 60-250Hz (Low-Mid)
        // Band 2: 250-4000Hz (High-Mid)
        // Band 3: 4000-20000Hz (Treble)
        
        int[] bandRanges = { 2, 8, 32, sampleDataLength };
        int startIndex = 0;
        
        for (int band = 0; band < 4; band++)
        {
            float sum = 0f;
            int endIndex = Mathf.Min(bandRanges[band], spectrumData.Length);
            
            for (int i = startIndex; i < endIndex; i++)
            {
                sum += spectrumData[i];
            }
            
            frequencyBands[band] = sum * frequencyBandMultipliers[band];
            startIndex = endIndex;
        }
    }
    
    private void DetectBeats()
    {
        beatDetected = false;
        beatTimer -= Time.deltaTime;
        
        if (beatTimer > 0) return; // Beat cooldown
        
        switch (detectionMethod)
        {
            case BeatDetectionMethod.EnergyThreshold:
                DetectBeatByEnergyThreshold();
                break;
            case BeatDetectionMethod.FrequencyAnalysis:
                DetectBeatByFrequencyAnalysis();
                break;
            case BeatDetectionMethod.AmplitudeSpikes:
                DetectBeatByAmplitudeSpikes();
                break;
            case BeatDetectionMethod.SpectrumPeaks:
                DetectBeatBySpectrumPeaks();
                break;
        }
        
        if (beatDetected)
        {
            OnBeatDetected();
            beatTimer = beatCooldown;
        }
    }
    
    private void DetectBeatByEnergyThreshold()
    {
        // Simple energy threshold method
        instantEnergy = currentAudioEnergy;
        float energyThreshold = averageEnergy + (energyVariance * beatThreshold);
        
        if (instantEnergy > energyThreshold && instantEnergy > previousAudioEnergy)
        {
            beatDetected = true;
        }
    }
    
    private void DetectBeatByFrequencyAnalysis()
    {
        // Focus on bass frequencies for beat detection
        if (frequencyBands[0] > beatThreshold && frequencyBands[0] > frequencyBands[1])
        {
            beatDetected = true;
        }
    }
    
    private void DetectBeatByAmplitudeSpikes()
    {
        // Detect sudden amplitude increases
        float energyIncrease = currentAudioEnergy - previousAudioEnergy;
        if (energyIncrease > beatThreshold && currentAudioEnergy > averageEnergy * 1.5f)
        {
            beatDetected = true;
        }
    }
    
    private void DetectBeatBySpectrumPeaks()
    {
        // Find peaks in spectrum data
        float maxSpectrum = 0f;
        for (int i = 1; i < spectrumData.Length - 1; i++)
        {
            if (spectrumData[i] > spectrumData[i - 1] && spectrumData[i] > spectrumData[i + 1])
            {
                maxSpectrum = Mathf.Max(maxSpectrum, spectrumData[i]);
            }
        }
        
        if (maxSpectrum > beatThreshold)
        {
            beatDetected = true;
        }
    }
    
    private void OnBeatDetected()
    {
        // Change to a beat color
        if (beatColors.Length > 0)
        {
            currentBeatColorIndex = (currentBeatColorIndex + 1) % beatColors.Length;
            Color beatColor = beatColors[currentBeatColorIndex];
            
            // Apply intensity based on energy
            float intensity = Mathf.Clamp01(currentAudioEnergy * colorIntensityMultiplier);
            beatColor = Color.Lerp(baseColor, beatColor, intensity);
            
            targetColor = beatColor;
        }
        
        // Trigger camera shake if available
        var cameraShake = FindObjectOfType<CameraShaking>();
        if (cameraShake != null)
        {
            float shakeIntensity = Mathf.Clamp01(currentAudioEnergy * 2f);
            // You might need to modify CameraShaking to accept intensity parameter
            // cameraShake.StartShaking(shakeIntensity);
        }
    }
    
    private void UpdateColors()
    {
        // Handle color cycling
        if (enableColorCycling && !beatDetected)
        {
            colorCycleTimer += Time.deltaTime * colorCycleSpeed;
            float hue = (colorCycleTimer % 1f);
            Color cycleColor = Color.HSVToRGB(hue, 0.5f, 0.8f);
            targetColor = Color.Lerp(baseColor, cycleColor, 0.3f);
        }
        else if (!beatDetected)
        {
            // Return to base color gradually
            targetColor = Color.Lerp(targetColor, baseColor, Time.deltaTime * colorReturnSpeed);
        }
        
        // Handle pulse effect
        if (enablePulseEffect)
        {
            float pulseIntensity = currentAudioEnergy * pulseStrength;
            Color pulseColor = targetColor + Color.white * pulseIntensity;
            targetColor = Color.Lerp(targetColor, pulseColor, Time.deltaTime * colorTransitionSpeed);
        }
    }
    
    private void UpdateBackgroundColor()
    {
        // Smoothly transition to target color
        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * colorTransitionSpeed);
        SetBackgroundColor(currentColor);
    }
    
    private void SetBackgroundColor(Color color)
    {
        if (useCameraBackground && targetCamera != null)
        {
            targetCamera.backgroundColor = color;
        }
        else if (!useCameraBackground && backgroundSpriteRenderer != null)
        {
            backgroundSpriteRenderer.color = color;
        }
    }
    
    private void DisplayDebugInfo()
    {
        Debug.Log($"Audio Energy: {currentAudioEnergy:F4}, Average: {averageEnergy:F4}, Beat: {beatDetected}");
        
        if (useFrequencyBands)
        {
            Debug.Log($"Frequency Bands - Bass: {frequencyBands[0]:F3}, Low-Mid: {frequencyBands[1]:F3}, High-Mid: {frequencyBands[2]:F3}, Treble: {frequencyBands[3]:F3}");
        }
    }
    
    // Public methods for external control
    public void SetBeatThreshold(float threshold)
    {
        beatThreshold = Mathf.Clamp01(threshold);
    }
    
    public void SetColorIntensity(float intensity)
    {
        colorIntensityMultiplier = Mathf.Max(0f, intensity);
    }
    
    public void SetBaseColor(Color color)
    {
        baseColor = color;
    }
    
    public void AddBeatColor(Color color)
    {
        Color[] newBeatColors = new Color[beatColors.Length + 1];
        System.Array.Copy(beatColors, newBeatColors, beatColors.Length);
        newBeatColors[beatColors.Length] = color;
        beatColors = newBeatColors;
    }
    
    public void ToggleColorCycling()
    {
        enableColorCycling = !enableColorCycling;
    }
    
    public void TogglePulseEffect()
    {
        enablePulseEffect = !enablePulseEffect;
    }
    
    // Integration with existing systems
    private void OnEnable()
    {
        // Subscribe to game events if needed
        if (SoundManager.instance != null)
        {
            // Could integrate with SoundManager events
        }
    }
    
    private void OnDisable()
    {
        // Unsubscribe from events
        // Reset background to base color
        if (targetCamera != null)
            targetCamera.backgroundColor = baseColor;
        if (backgroundSpriteRenderer != null)
            backgroundSpriteRenderer.color = Color.white;
    }
}
