using UnityEngine;

public class ScaleTo : StateMachineBehaviour
{
    [Header("Scale Settings")]
    public Vector3 targetScale = Vector3.one;
    public float duration = 1f;
    public LeanTweenType easeType = LeanTweenType.easeOutQuad;
    public bool useLocalScale = true;
    
    [Header("Animation Options")]
    public bool setTriggerOnComplete = true;
    public string completeTriggerName = "CanExitState";
    
    [Header("Safety Options")]
    public bool useSafetyTrigger = true;
    public float safetyDelay = 0.1f;

    private Animator _animator;
    private bool _hasTriggered = false;
    private int _tweenId = -1;
    private int _safetyTweenId = -1;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _animator = animator;
        _hasTriggered = false;
        
        Debug.Log($"ScaleTo: Starting scale animation to {targetScale} over {duration} seconds");
        Debug.Log($"ScaleTo: Current scale: {animator.transform.localScale}");
        
        if (useLocalScale)
        {
            _tweenId = LeanTween.scale(animator.gameObject, targetScale, duration)
                .setEase(easeType)
                .setOnComplete(() =>
                {
                    Debug.Log($"ScaleTo: LeanTween completed successfully!");
                    TriggerCompletion();
                }).id;
        }
        else
        {
            // For world scale (less common, but available if needed)
            Vector3 currentScale = animator.transform.lossyScale;
            _tweenId = LeanTween.value(animator.gameObject, currentScale, targetScale, duration)
                .setOnUpdate((Vector3 value) =>
                {
                    if (animator.transform.parent != null)
                    {
                        animator.transform.localScale = new Vector3(
                            value.x / animator.transform.parent.lossyScale.x,
                            value.y / animator.transform.parent.lossyScale.y,
                            value.z / animator.transform.parent.lossyScale.z
                        );
                    }
                    else
                    {
                        animator.transform.localScale = value;
                    }
                })
                .setEase(easeType)
                .setOnComplete(() =>
                {
                    Debug.Log($"ScaleTo: LeanTween completed successfully!");
                    TriggerCompletion();
                }).id;
        }
        
        // Safety fallback trigger
        if (useSafetyTrigger)
        {
            _safetyTweenId = LeanTween.delayedCall(duration + safetyDelay, () =>
            {
                Debug.LogWarning($"ScaleTo: Safety trigger activated after {duration + safetyDelay} seconds");
                TriggerCompletion();
            }).id;
        }
        
        Debug.Log($"ScaleTo: Tween ID: {_tweenId}, Safety ID: {_safetyTweenId}");
    }
    
    private void TriggerCompletion()
    {
        if (_hasTriggered)
        {
            Debug.Log("ScaleTo: Completion already triggered, ignoring duplicate call");
            return;
        }
        
        _hasTriggered = true;
        
        Debug.Log($"ScaleTo: Animation completed. Triggering: {completeTriggerName}");
        Debug.Log($"ScaleTo: Final scale: {_animator.transform.localScale}");
        
        // Cancel safety trigger if it hasn't fired yet
        if (_safetyTweenId != -1)
        {
            LeanTween.cancel(_safetyTweenId);
        }
        
        if (setTriggerOnComplete && !string.IsNullOrEmpty(completeTriggerName))
        {
            _animator.SetTrigger(completeTriggerName);
            Debug.Log($"ScaleTo: Trigger '{completeTriggerName}' sent successfully");
            
            // Additional check to see if trigger was actually set
            bool triggerExists = false;
            for (int i = 0; i < _animator.parameterCount; i++)
            {
                if (_animator.GetParameter(i).name == completeTriggerName)
                {
                    triggerExists = true;
                    break;
                }
            }
            
            if (!triggerExists)
            {
                Debug.LogError($"ScaleTo: Trigger '{completeTriggerName}' does not exist in the Animator Controller!");
            }
        }
    }
    
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Optional: Add any per-frame logic here if needed
    }
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("ScaleTo: State exiting - cancelling any ongoing tweens");
        
        // Cancel both main and safety tweens
        if (_tweenId != -1)
        {
            LeanTween.cancel(_tweenId);
        }
        if (_safetyTweenId != -1)
        {
            LeanTween.cancel(_safetyTweenId);
        }
        
        // Also cancel by GameObject as backup
        LeanTween.cancel(animator.gameObject);
    }
}
