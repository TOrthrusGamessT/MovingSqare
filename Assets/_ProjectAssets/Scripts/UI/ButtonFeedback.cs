using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Mobile-optimized button feedback for responsive UI interactions.
/// Designed for touch interfaces with satisfying visual feedback.
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Touch Animation")]
    [Tooltip("Scale factor when button is touched (0.9 = 90% of original size)")]
    [Range(0.7f, 0.95f)]
    public float touchScale = 0.9f;
    
    [Tooltip("Duration of press animation")]
    [Range(0.05f, 0.2f)]
    public float pressDuration = 0.1f;
    
    [Tooltip("Duration of release animation")]
    [Range(0.05f, 0.3f)]
    public float releaseDuration = 0.15f;
    
    [Header("Feedback Effects")]
    [Tooltip("Add satisfying bounce effect on release")]
    public bool useBounceEffect = true;
    
    [Tooltip("Intensity of bounce effect")]
    [Range(0.05f, 0.2f)]
    public float bounceIntensity = 0.1f;
    
    [Tooltip("Tint button color when pressed")]
    public bool useColorFeedback = true;
    
    [Tooltip("Color tint when button is pressed")]
    public Color pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    
    [Header("Mobile Optimization")]
    [Tooltip("Use lighter animations for better mobile performance")]
    public bool mobileLightMode = true;
    
    [Tooltip("Disable feedback when button is not interactable")]
    public bool respectInteractableState = true;
    
    // Private variables
    private Button button;
    private Image buttonImage;
    private Color originalColor;
    private Vector3 originalScale;
    private bool isPressed = false;
    private int currentTweenId = -1;
    
    void Start()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        originalScale = transform.localScale;
        
        if (buttonImage != null)
        {
            originalColor = buttonImage.color;
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (respectInteractableState && !button.interactable) return;
        
        isPressed = true;
        
        // Cancel any existing animations
        if (currentTweenId != -1)
        {
            LeanTween.cancel(currentTweenId);
        }
        
        // Press animation
        currentTweenId = LeanTween.scale(gameObject, originalScale * touchScale, pressDuration)
            .setEase(LeanTweenType.easeOutQuad).id;
        
        // Color feedback for mobile
        if (useColorFeedback && buttonImage != null)
        {
            LeanTween.value(gameObject, buttonImage.color, pressedColor, pressDuration)
                .setOnUpdate((Color c) => { if (buttonImage != null) buttonImage.color = c; });
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (respectInteractableState && !button.interactable) return;
        
        isPressed = false;
        
        // Cancel any existing animations
        if (currentTweenId != -1)
        {
            LeanTween.cancel(currentTweenId);
        }
        
        if (useBounceEffect && !mobileLightMode)
        {
            // Full bounce effect for better devices
            currentTweenId = LeanTween.scale(gameObject, originalScale * (1f + bounceIntensity), releaseDuration * 0.4f)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnComplete(() =>
                {
                    LeanTween.scale(gameObject, originalScale, releaseDuration * 0.6f)
                        .setEase(LeanTweenType.easeOutBounce);
                }).id;
        }
        else
        {
            // Simple scale back for mobile performance
            currentTweenId = LeanTween.scale(gameObject, originalScale, releaseDuration)
                .setEase(LeanTweenType.easeOutBack).id;
        }
        
        // Restore color
        if (useColorFeedback && buttonImage != null)
        {
            LeanTween.value(gameObject, buttonImage.color, originalColor, releaseDuration)
                .setOnUpdate((Color c) => { if (buttonImage != null) buttonImage.color = c; });
        }
    }
    
    void OnDisable()
    {
        // Clean up when disabled
        if (currentTweenId != -1)
        {
            LeanTween.cancel(currentTweenId);
        }
        
        // Reset to original state
        if (transform != null)
            transform.localScale = originalScale;
        
        if (buttonImage != null)
            buttonImage.color = originalColor;
        
        isPressed = false;
    }
    
    /// <summary>
    /// Trigger press animation programmatically (useful for tutorials)
    /// </summary>
    public void SimulatePress()
    {
        if (respectInteractableState && !button.interactable) return;
        
        // Cancel existing animations
        if (currentTweenId != -1)
        {
            LeanTween.cancel(currentTweenId);
        }
        
        // Quick press and release simulation
        LeanTween.scale(gameObject, originalScale * touchScale, pressDuration * 0.5f)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
            {
                LeanTween.scale(gameObject, originalScale, releaseDuration)
                    .setEase(LeanTweenType.easeOutBack);
            });
    }
    
    /// <summary>
    /// Set mobile optimization mode
    /// </summary>
    public void SetMobileMode(bool enabled)
    {
        mobileLightMode = enabled;
        
        if (enabled)
        {
            // Optimize settings for mobile
            pressDuration = 0.08f;
            releaseDuration = 0.12f;
            bounceIntensity = 0.05f;
        }
        else
        {
            // Full quality settings
            pressDuration = 0.1f;
            releaseDuration = 0.15f;
            bounceIntensity = 0.1f;
        }
    }
}
