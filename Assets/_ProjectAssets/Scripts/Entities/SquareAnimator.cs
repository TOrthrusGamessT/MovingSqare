using System;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class SquareAnimator : MonoBehaviour
{
    [Header("Dodge Animation Settings")]
    [SerializeField] private float stretchAmount = 0.4f; // How much to stretch when dodging
    [SerializeField] private float scaleSpeed = 25f; // Very fast scale changes for responsiveness
    [SerializeField] private float breathingAmount = 0.05f; // Minimal breathing when idle
    [SerializeField] private float breathingSpeed = 3f; // Faster, more alert breathing
    [SerializeField] private float minMovementForStretch = 0.005f; // Very sensitive to movement
    
    [Header("Quick Response Effects")]
    [SerializeField] private float dodgeSnapAmount = 0.25f; // Instant compression when starting to move
    [SerializeField] private float directionChangeBonus = 0.3f; // Extra stretch on direction changes
    [SerializeField] private float recoverySpeed = 15f; // How fast to return to normal after dodge
    [SerializeField] private AnimationCurve dodgeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Snappy dodge curve
    
    [Header("Alert State")]
    [SerializeField] private float alertnessBuildup = 8f; // Fast alertness buildup for dodging
    [SerializeField] private float maxAlertness = 2f; // High alertness multiplier
    [SerializeField] private float alertnessDecay = 4f; // Medium decay to stay ready
    [SerializeField] private float anticipationSpeed = 20f; // Very fast anticipation for quick dodges
    
    [Header("Enhanced Effects")]
    [SerializeField] private float highSpeedThreshold = 0.15f; // Speed threshold for high-speed effects
    [SerializeField] private float stopBounceIntensity = 0.4f; // How much to bounce when stopping at high speed
    [SerializeField] private float maxRotationAngle = 15f; // Maximum rotation angle for left-right movement
    [SerializeField] private float rotationSpeed = 8f; // How fast to rotate
    [SerializeField] private float rotationDecay = 6f; // How fast rotation returns to normal

    // Animation state variables
    private Vector3 originalScale;
    private Vector3 targetScale;
    private Vector2 lastPosition;
    private Vector2 movementDirection;
    private float movementMagnitude;
    private bool animationInitialized = false;
    
    // Advanced animation variables
    private float bounceTimer = 0f;
    private bool wasMovingLastFrame = false;
    private float anticipationTimer = 0f;
    private float alertnessLevel = 0f; // Changed from excitementLevel for dodge context
    private float velocitySmoothed = 0f;
    private Vector2 lastVelocity = Vector2.zero;
    private float timeMoving = 0f;
    private float timeIdle = 0f;
    private float lastDirectionChangeTime = 0f;
    private Vector2 lastMoveDirection = Vector2.zero;
    
    // Enhanced effects variables
    private float currentRotation = 0f;
    private float targetRotation = 0f;
    private float lastStopSpeed = 0f; // Track speed when stopping for bounce effect
    
    // Movement reference
    private Movement movementScript;
    private bool isAlive = true;

    // Events for other systems to listen to
    public static event Action<float> OnExcitementChanged; // For sound/particle systems
    public static event Action OnMovementStarted;
    public static event Action OnMovementStopped;
    public static event Action<Vector2> OnDirectionChanged; // For rotation effects

    private void Awake()
    {
        movementScript = GetComponent<Movement>();
    }

    private void OnEnable()
    {
        PlayerLife.onPlayerDie += SetAliveFalse;
        AdsManager.onReviveADFinish += SetAliveTrue;
    }

    private void OnDisable()
    {
        PlayerLife.onPlayerDie -= SetAliveFalse;
        AdsManager.onReviveADFinish -= SetAliveTrue;
    }

    private void Update()
    {
        if (animationInitialized && isAlive)
        {
            UpdateAnimationState();
            UpdateScale();
        }
    }

    private void UpdateAnimationState()
    {
        // Get movement data from Movement script
        bool isMoving = movementScript.IsMoving;
        Vector2 currentPosition = transform.position;
        
        if (isMoving)
        {
            // Calculate movement data
            movementDirection = (currentPosition - lastPosition);
            movementMagnitude = movementDirection.magnitude;
            
            if (movementMagnitude > 0)
            {
                movementDirection = movementDirection.normalized;
            }
        }
        else
        {
            movementMagnitude = 0;
        }
        
        lastPosition = currentPosition;
        
        // Update personality system
        UpdatePersonalitySystem(isMoving);
        
        // Handle bounce effects
        HandleBounceEffects(isMoving);
    }
    
    private void UpdateScale()
    {
        bool isMoving = movementScript.IsMoving;
        
        if (isMoving && movementMagnitude > minMovementForStretch)
        {
            // Fast, responsive dodge animation
            float curveValue = dodgeCurve.Evaluate(Mathf.Clamp01(movementMagnitude * 10f)); // More sensitive
            float alertnessMultiplier = 1f + (alertnessLevel * 0.4f); // Stronger effect
            
            // Calculate stretch based on movement direction with dodge snap
            float horizontalStretch = 1f + (Mathf.Abs(movementDirection.x) * stretchAmount * curveValue * alertnessMultiplier);
            float verticalStretch = 1f + (Mathf.Abs(movementDirection.y) * stretchAmount * curveValue * alertnessMultiplier);
            
            // Enhanced compression for dodge feel - more dramatic
            float compressionFactor = 0.4f + (velocitySmoothed * 0.2f);
            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.y))
            {
                // Horizontal dodge - compress vertically more
                verticalStretch = Mathf.Max(0.5f, 1f - (stretchAmount * compressionFactor));
            }
            else if (Mathf.Abs(movementDirection.y) > Mathf.Abs(movementDirection.x))
            {
                // Vertical dodge - compress horizontally more
                horizontalStretch = Mathf.Max(0.5f, 1f - (stretchAmount * compressionFactor));
            }
            
            // Instant snap effect when starting to move (dodge anticipation)
            if (anticipationTimer > 0)
            {
                float snapScale = 1f - (dodgeSnapAmount * anticipationTimer);
                horizontalStretch *= snapScale;
                verticalStretch *= snapScale;
                anticipationTimer -= Time.deltaTime * anticipationSpeed;
            }
            
            // Direction change bonus for quick dodges
            if (Time.time - lastDirectionChangeTime < 0.3f)
            {
                float directionBonus = 1f + directionChangeBonus * (1f - (Time.time - lastDirectionChangeTime) / 0.3f);
                horizontalStretch *= directionBonus;
                verticalStretch *= directionBonus;
            }
            
            targetScale = new Vector3(
                originalScale.x * horizontalStretch,
                originalScale.y * verticalStretch,
                originalScale.z
            );
        }
        else
        {
            // Alert breathing - ready to dodge
            float alertBreathing = breathingAmount * (1f + alertnessLevel * 0.8f);
            float breathingPhase = Time.time * breathingSpeed + (alertnessLevel * 0.3f);
            float breathingScale = 1f + Mathf.Sin(breathingPhase) * alertBreathing;
            
            // Add subtle tension variations when alert
            float tensionVariation = Mathf.PerlinNoise(Time.time * 1.5f, 0) * 0.03f * alertnessLevel;
            breathingScale += tensionVariation;
            
            targetScale = originalScale * breathingScale;
        }
        
        // Quick bounce for dodge feedback
        if (bounceTimer > 0)
        {
            float bounceIntensity = 0.2f; // Stronger bounce for dodge feedback
            float bounceScale = 1f + (bounceIntensity * bounceTimer * Mathf.Sin(bounceTimer * 20f)); // Faster bounce
            targetScale *= bounceScale;
            bounceTimer = Mathf.Max(0, bounceTimer - Time.deltaTime * 12f); // Faster decay
        }
        
        // Subtle overshoot for responsiveness
        float overshoot = 1f + 0.03f * Mathf.Sin(Time.time * 15f) * alertnessLevel;
        
        // Very fast scale interpolation for dodge responsiveness
        float dynamicScaleSpeed = scaleSpeed * (1f + alertnessLevel * 0.3f);
        Vector3 newScale = Vector3.Lerp(transform.localScale, targetScale * overshoot, Time.deltaTime * dynamicScaleSpeed);
        
        transform.localScale = newScale;
        
        // Handle rotation effects for left-right movement
        UpdateRotationEffects();
    }
    
    private void UpdateRotationEffects()
    {
        bool isMoving = movementScript.IsMoving;
        
        if (isMoving)
        {
            // Calculate rotation based on horizontal movement
            float horizontalMovement = movementDirection.x;
            
            if (Mathf.Abs(horizontalMovement) > 0.3f) // Only rotate for significant horizontal movement
            {
                // Set target rotation based on movement direction and speed
                float rotationIntensity = Mathf.Abs(horizontalMovement) * movementMagnitude * 100f; // Scale up for visibility
                targetRotation = -horizontalMovement * maxRotationAngle * Mathf.Clamp01(rotationIntensity);
            }
            else
            {
                // Return to neutral when not moving horizontally
                targetRotation = 0f;
            }
        }
        else
        {
            // Return to neutral when not moving
            targetRotation = 0f;
        }
        
        // Smoothly interpolate rotation
        currentRotation = Mathf.Lerp(currentRotation, targetRotation, Time.deltaTime * rotationSpeed);
        
        // Apply rotation
        transform.rotation = Quaternion.Euler(0, 0, currentRotation);
    }
    
    private void UpdatePersonalitySystem(bool isMoving)
    {
        float previousAlertness = alertnessLevel;
        
        if (isMoving)
        {
            timeMoving += Time.deltaTime;
            timeIdle = 0f;
            
            // Build alertness very quickly for dodge-ready state
            alertnessLevel = Mathf.Min(maxAlertness, alertnessLevel + Time.deltaTime * alertnessBuildup);
            
            // Smooth velocity for better effects
            Vector2 currentVelocity = (Vector2)transform.position - lastPosition;
            velocitySmoothed = Mathf.Lerp(velocitySmoothed, currentVelocity.magnitude, Time.deltaTime * 8f); // Faster response
        }
        else
        {
            timeIdle += Time.deltaTime;
            timeMoving = 0f;
            
            // Stay somewhat alert even when idle (ready to dodge)
            alertnessLevel = Mathf.Max(0.1f, alertnessLevel - Time.deltaTime * alertnessDecay); // Never fully calm
            velocitySmoothed = Mathf.Lerp(velocitySmoothed, 0f, Time.deltaTime * 5f);
        }
        
        // Notify other systems of alertness changes (renamed from excitement)
        if (Mathf.Abs(alertnessLevel - previousAlertness) > 0.01f)
        {
            OnExcitementChanged?.Invoke(alertnessLevel); // Keep same event name for compatibility
        }
    }
    
    private void HandleBounceEffects(bool isMoving)
    {
        Vector2 currentDirection = movementDirection;
        
        // Detect movement state changes for quick dodge feedback
        if (isMoving && !wasMovingLastFrame)
        {
            // Just started moving - instant dodge response
            bounceTimer = 0.8f; // Shorter, snappier bounce
            anticipationTimer = 0.6f; // Quick anticipation
            OnMovementStarted?.Invoke();
        }
        else if (!isMoving && wasMovingLastFrame)
        {
            // Just stopped moving - check if it was high speed for enhanced bounce
            if (lastStopSpeed > highSpeedThreshold)
            {
                // High-speed stop - bigger bounce effect
                float speedMultiplier = Mathf.Clamp01(lastStopSpeed / (highSpeedThreshold * 2f));
                bounceTimer = 0.4f + (stopBounceIntensity * speedMultiplier); // Enhanced bounce based on speed
                
                // Add some extra rotation wobble for dramatic effect
                currentRotation += UnityEngine.Random.Range(-5f, 5f) * speedMultiplier;
            }
            else
            {
                // Normal stop - quick settle
                bounceTimer = 0.4f; // Short settle bounce
            }
            
            OnMovementStopped?.Invoke();
        }
        
        // Track movement speed for stop effects
        if (isMoving)
        {
            lastStopSpeed = movementMagnitude;
        }
        
        // Detect direction changes for dodge maneuvers
        if (isMoving && lastMoveDirection.magnitude > 0.1f)
        {
            float directionDot = Vector2.Dot(currentDirection, lastMoveDirection);
            if (directionDot < 0.7f) // More sensitive to direction changes
            {
                OnDirectionChanged?.Invoke(currentDirection);
                bounceTimer = Mathf.Max(bounceTimer, 0.6f); // Quick direction change bounce
                lastDirectionChangeTime = Time.time;
                
                // Boost alertness on direction changes
                alertnessLevel = Mathf.Min(maxAlertness, alertnessLevel + 0.3f);
                
                // Add rotation effect for sharp direction changes
                if (directionDot < 0.3f) // Very sharp turn
                {
                    float turnIntensity = 1f - directionDot; // 0 to 1 based on sharpness
                    currentRotation += UnityEngine.Random.Range(-3f, 3f) * turnIntensity;
                }
            }
        }
        
        wasMovingLastFrame = isMoving;
        if (isMoving)
        {
            lastMoveDirection = currentDirection;
        }
    }

    private void SetAliveTrue()
    {
        isAlive = true;
    }

    private void SetAliveFalse()
    {
        isAlive = false;
    }
    
    /// <summary>
    /// Initialize animation effects after PlayerManager has applied all effects
    /// This should be called after skin effects are applied
    /// </summary>
    public void InitializeAnimation()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
        lastPosition = transform.position;
        currentRotation = 0f;
        targetRotation = 0f;
        transform.rotation = Quaternion.identity; // Reset rotation
        animationInitialized = true;
    }
    
    /// <summary>
    /// Public method to trigger special animations from other systems
    /// </summary>
    public void TriggerSpecialAnimation(string animationType, float intensity = 1f)
    {
        switch (animationType.ToLower())
        {
            case "bounce":
                bounceTimer = intensity;
                break;
            case "alertness":
            case "excitement": // Keep compatibility
                alertnessLevel = Mathf.Min(maxAlertness, alertnessLevel + intensity);
                break;
            case "anticipation":
                anticipationTimer = intensity;
                break;
            case "dodge":
                // Special dodge trigger - combines multiple effects
                bounceTimer = 0.8f;
                anticipationTimer = 0.4f;
                alertnessLevel = Mathf.Min(maxAlertness, alertnessLevel + 0.5f);
                break;
            case "highspeedstop":
                // Trigger high-speed stop effect manually
                bounceTimer = 0.4f + (stopBounceIntensity * intensity);
                currentRotation += UnityEngine.Random.Range(-5f, 5f) * intensity;
                break;
            case "rotation":
            case "spin":
                // Add immediate rotation effect
                currentRotation += intensity * maxRotationAngle * (UnityEngine.Random.value > 0.5f ? 1f : -1f);
                break;
        }
    }
    
    // Getters for other systems
    public float AlertnessLevel => alertnessLevel; // Primary getter
    public float ExcitementLevel => alertnessLevel; // Compatibility getter
    public bool IsAnimationInitialized => animationInitialized;
    public Vector2 MovementDirection => movementDirection;
    public float MovementMagnitude => movementMagnitude;
}
