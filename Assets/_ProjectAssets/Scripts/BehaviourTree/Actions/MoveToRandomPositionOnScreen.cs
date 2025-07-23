using UnityEngine;

public class MoveToRandomPositionOnScreen : StateMachineBehaviour
{
    [Header("Movement Settings")]
    public float duration = 1f;
    public LeanTweenType easeType = LeanTweenType.easeOutQuad;
    public bool useLocalPosition = false;
    
    [Header("Screen Boundary Settings")]
    public bool includeObjectSize = true;
    public float boundaryPadding = 0.5f;
    
    [Header("Position Constraints")]
    [Range(0f, 1f)] public float minXPercent = 0f;
    [Range(0f, 1f)] public float maxXPercent = 1f;
    [Range(0f, 1f)] public float minYPercent = 0f;
    [Range(0f, 1f)] public float maxYPercent = 1f;
    
    [Header("Animation Options")]
    public bool setTriggerOnComplete = true;
    public string completeTriggerName = "CanExitState";
    
    [Header("Safety Settings")]
    public float safetyTriggerDelay = 0.1f;

    private Animator _animator;
    private Camera _mainCamera;
    private int _tweenId = -1;
    private bool _hasTriggered = false;
    private float _stateStartTime;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _animator = animator;
        _mainCamera = Camera.main;
        _hasTriggered = false;
        _stateStartTime = Time.time;
        
        Debug.Log($"MoveToRandomPositionOnScreen: State entered at time {_stateStartTime}, duration set to {duration}, safety delay {safetyTriggerDelay}");
        
        if (_mainCamera == null)
        {
            Debug.LogError("MoveToRandomPositionOnScreen: Main camera not found!");
            TriggerComplete();
            return;
        }
        
        Vector3 randomPosition = GetRandomScreenPosition(animator.gameObject);
        Debug.Log($"MoveToRandomPositionOnScreen: Moving from {animator.gameObject.transform.position} to {randomPosition} over {duration} seconds");
        MoveToPosition(animator.gameObject, randomPosition);
        
        // Safety trigger as backup
        LeanTween.delayedCall(duration + safetyTriggerDelay, () => 
        {
            if (!_hasTriggered)
            {
                float elapsed = Time.time - _stateStartTime;
                Debug.LogWarning($"MoveToRandomPositionOnScreen: Safety trigger activated after {elapsed:F2}s - main callback may have failed");
                TriggerComplete();
            }
        });
    }
    
    private void TriggerComplete()
    {
        if (_hasTriggered)
        {
            Debug.Log("MoveToRandomPositionOnScreen: Trigger already sent, ignoring duplicate");
            return;
        }
        
        _hasTriggered = true;
        float elapsed = Time.time - _stateStartTime;
        Debug.Log($"MoveToRandomPositionOnScreen: Animation completed after {elapsed:F2}s (expected {duration}s). Triggering: {completeTriggerName}");
        
        if (setTriggerOnComplete && !string.IsNullOrEmpty(completeTriggerName) && _animator != null)
        {
            _animator.SetTrigger(completeTriggerName);
            Debug.Log($"MoveToRandomPositionOnScreen: Trigger '{completeTriggerName}' sent successfully");
        }
    }
    
    private Vector3 GetRandomScreenPosition(GameObject obj)
    {
        // Get screen boundaries in world coordinates
        float worldScreenHeight = _mainCamera.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        
        // Calculate base boundaries
        float leftBound = -worldScreenWidth / 2f;
        float rightBound = worldScreenWidth / 2f;
        float bottomBound = -worldScreenHeight / 2f;
        float topBound = worldScreenHeight / 2f;
        
        // Account for object size if enabled
        if (includeObjectSize)
        {
            Vector3 objectSize = GetObjectSize(obj);
            float halfWidth = objectSize.x / 2f;
            float halfHeight = objectSize.y / 2f;
            
            leftBound += halfWidth + boundaryPadding;
            rightBound -= halfWidth + boundaryPadding;
            bottomBound += halfHeight + boundaryPadding;
            topBound -= halfHeight + boundaryPadding;
        }
        else
        {
            // Just add padding
            leftBound += boundaryPadding;
            rightBound -= boundaryPadding;
            bottomBound += boundaryPadding;
            topBound -= boundaryPadding;
        }
        
        // Apply percentage constraints
        float constrainedLeft = Mathf.Lerp(leftBound, rightBound, minXPercent);
        float constrainedRight = Mathf.Lerp(leftBound, rightBound, maxXPercent);
        float constrainedBottom = Mathf.Lerp(bottomBound, topBound, minYPercent);
        float constrainedTop = Mathf.Lerp(bottomBound, topBound, maxYPercent);
        
        // Ensure valid ranges
        if (constrainedLeft >= constrainedRight)
        {
            float temp = constrainedLeft;
            constrainedLeft = constrainedRight;
            constrainedRight = temp;
        }
        
        if (constrainedBottom >= constrainedTop)
        {
            float temp = constrainedBottom;
            constrainedBottom = constrainedTop;
            constrainedTop = temp;
        }
        
        // Generate random position within constraints
        float randomX = Random.Range(constrainedLeft, constrainedRight);
        float randomY = Random.Range(constrainedBottom, constrainedTop);
        
        return new Vector3(randomX, randomY, obj.transform.position.z);
    }
    
    private Vector3 GetObjectSize(GameObject obj)
    {
        Vector3 size = Vector3.one;
        
        // Try different components to get size
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            size = spriteRenderer.bounds.size;
            return size;
        }
        
        Collider2D collider2D = obj.GetComponent<Collider2D>();
        if (collider2D != null)
        {
            size = collider2D.bounds.size;
            return size;
        }
        
        Collider collider3D = obj.GetComponent<Collider>();
        if (collider3D != null)
        {
            size = collider3D.bounds.size;
            return size;
        }
        
        // Fallback to transform scale
        size = obj.transform.lossyScale;
        return size;
    }
    
    private void MoveToPosition(GameObject obj, Vector3 targetPosition)
    {
        if (useLocalPosition)
        {
            // Convert world position to local position if needed
            if (obj.transform.parent != null)
            {
                targetPosition = obj.transform.parent.InverseTransformPoint(targetPosition);
            }
            
            _tweenId = LeanTween.moveLocal(obj, targetPosition, duration)
                .setEase(easeType)
                .setOnComplete(() =>
                {
                    float elapsed = Time.time - _stateStartTime;
                    Debug.Log($"MoveToRandomPositionOnScreen: LeanTween LOCAL callback triggered after {elapsed:F2}s");
                    TriggerComplete();
                }).id;
        }
        else
        {
            _tweenId = LeanTween.move(obj, targetPosition, duration)
                .setEase(easeType)
                .setOnComplete(() =>
                {
                    float elapsed = Time.time - _stateStartTime;
                    Debug.Log($"MoveToRandomPositionOnScreen: LeanTween WORLD callback triggered after {elapsed:F2}s");
                    TriggerComplete();
                }).id;
        }
        
        Debug.Log($"MoveToRandomPositionOnScreen: Started tween with ID: {_tweenId}");
    }
    
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Optional: Add any per-frame logic here if needed
    }
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float elapsed = Time.time - _stateStartTime;
        Debug.Log($"MoveToRandomPositionOnScreen: State exiting after {elapsed:F2}s - cancelling any ongoing tween");
        
        // Cancel specific tween if we have an ID
        if (_tweenId != -1)
        {
            LeanTween.cancel(_tweenId);
            Debug.Log($"MoveToRandomPositionOnScreen: Cancelled tween with ID: {_tweenId}");
        }
        else
        {
            // Fallback to cancel all tweens on this object
            LeanTween.cancel(animator.gameObject);
        }
        
        _tweenId = -1;
        _hasTriggered = false;
    }
}
