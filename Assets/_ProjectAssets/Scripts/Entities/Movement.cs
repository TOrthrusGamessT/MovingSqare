using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.TouchPhase;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public float speed;
    
    [Header("Scale Effects")]
    [SerializeField] private float stretchAmount = 0.2f; // How much to stretch when moving
    [SerializeField] private float scaleSpeed = 8f; // How fast scale changes
    [SerializeField] private float breathingAmount = 0.05f; // Subtle breathing effect
    [SerializeField] private float breathingSpeed = 2f; // Speed of breathing
    [SerializeField] private float minMovementForStretch = 0.01f; // Minimum movement to trigger stretch

    
    private float maxX= 2.17f;
    private float minX = -2.17f;
    private float maxY= 4.82f;
    private float minY= -4.82f;
    
    private bool isMoving;
    private Vector2 currentInputValue;
    private Vector2 smoothInputValue;
    private Touch _touch;
    private bool alive = true;
    
    // Scale effect variables
    private Vector3 originalScale;
    private Vector3 targetScale;
    private Vector2 lastPosition;
    private Vector2 movementDirection;
    private float movementMagnitude;
    private bool scaleEffectsInitialized = false;


    private void OnEnable()
    {
        PlayerLife.onPlayerDie += SetAliveFalse;
        AdsManager.onReviveADFinish += SetAliveTrue;
    }

    private void OnDisable()
    {
        PlayerLife.onPlayerDie -= SetAliveFalse;
        AdsManager.onReviveADFinish -= SetAliveTrue;
    }    public void Update()
    {
        HandleMovement();
        if (scaleEffectsInitialized)
        {
            UpdateScale();
        }
    }
    
    private void HandleMovement()
    {
        if (Input.touchCount > 0 && alive)
        {
            _touch = Input.GetTouch(0);

            if (_touch.phase == TouchPhase.Moved)
            {
                Vector2 nextPosition = new Vector2(
                    transform.position.x + _touch.deltaPosition.x * speed,
                    transform.position.y + _touch.deltaPosition.y * speed);
                
                nextPosition.x = Mathf.Clamp(nextPosition.x, minX, maxX);
                nextPosition.y = Mathf.Clamp(nextPosition.y, minY, maxY);
                
                // Calculate movement for scale effects
                if (scaleEffectsInitialized)
                {
                    Vector2 currentPosition = transform.position;
                    movementDirection = (nextPosition - lastPosition);
                    movementMagnitude = movementDirection.magnitude;
                    isMoving = movementMagnitude > minMovementForStretch;
                    
                    // Normalize movement direction for stretch calculation
                    if (movementMagnitude > 0)
                    {
                        movementDirection = movementDirection.normalized;
                    }
                    
                    lastPosition = nextPosition;
                }
                
                transform.position = nextPosition;
            }
            else
            {
                isMoving = false;
            }
        }
        else
        {
            isMoving = false;
            movementMagnitude = 0;
        }
    }
    
    private void UpdateScale()
    {
        if (!alive)
        {
            return;
        }

        if (isMoving && movementMagnitude > minMovementForStretch)
        {
            // Create directional stretch effect
            float horizontalStretch = 1f + (Mathf.Abs(movementDirection.x) * stretchAmount * Mathf.Clamp01(movementMagnitude * 10f));
            float verticalStretch = 1f + (Mathf.Abs(movementDirection.y) * stretchAmount * Mathf.Clamp01(movementMagnitude * 10f));
            
            // Add compression effect on the opposite axis for more dynamic feel
            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.y))
            {
                // Moving more horizontally - stretch X, compress Y slightly
                verticalStretch = Mathf.Max(0.8f, 1f - (stretchAmount * 0.3f));
            }
            else if (Mathf.Abs(movementDirection.y) > Mathf.Abs(movementDirection.x))
            {
                // Moving more vertically - stretch Y, compress X slightly
                horizontalStretch = Mathf.Max(0.8f, 1f - (stretchAmount * 0.3f));
            }
            
            targetScale = new Vector3(
                originalScale.x * horizontalStretch,
                originalScale.y * verticalStretch,
                originalScale.z
            );
        }
        else
        {
            // Add subtle breathing effect when idle
            float breathingScale = 1f + Mathf.Sin(Time.time * breathingSpeed) * breathingAmount;
            targetScale = originalScale * breathingScale;
        }
        
        // Smoothly interpolate to target scale
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }

    private void SetAliveTrue()
    {
        alive = true;
    }

    private void SetAliveFalse()
    {
        alive = false;
    }
    
    /// <summary>
    /// Initialize scale effects after PlayerManager has applied all effects
    /// This should be called after skin effects are applied
    /// </summary>
    public void InitializeScaleEffects()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
        lastPosition = transform.position;
        scaleEffectsInitialized = true;
    }
}