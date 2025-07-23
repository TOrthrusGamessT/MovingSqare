using UnityEngine;

public class MoveTo : StateMachineBehaviour
{
    [Header("Movement Settings")]
    public Vector3 targetPosition;
    public float duration = 1f;
    public LeanTweenType easeType = LeanTweenType.easeOutQuad;
    public bool useLocalPosition = true;
    public bool useRelativeMovement = false;
    
    [Header("Animation Options")]
    public bool setTriggerOnComplete = true;
    public string completeTriggerName = "CanExitState";

    private Animator _animator;
    private Vector3 _startPosition;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _animator = animator;
        
        if (useLocalPosition)
        {
            _startPosition = animator.transform.localPosition;
            Vector3 finalTarget = useRelativeMovement ? _startPosition + targetPosition : targetPosition;
            
            Debug.Log($"MoveTo: Starting local movement to {finalTarget} over {duration} seconds");
            
            LeanTween.moveLocal(animator.gameObject, finalTarget, duration)
                .setEase(easeType)
                .setOnComplete(() =>
                {
                    Debug.Log($"MoveTo: Animation completed. Triggering: {completeTriggerName}");
                    if (setTriggerOnComplete && !string.IsNullOrEmpty(completeTriggerName))
                    {
                        _animator.SetTrigger(completeTriggerName);
                        Debug.Log($"MoveTo: Trigger '{completeTriggerName}' sent successfully");
                    }
                });
        }
        else
        {
            _startPosition = animator.transform.position;
            Vector3 finalTarget = useRelativeMovement ? _startPosition + targetPosition : targetPosition;
            
            Debug.Log($"MoveTo: Starting world movement to {finalTarget} over {duration} seconds");
            
            LeanTween.move(animator.gameObject, finalTarget, duration)
                .setEase(easeType)
                .setOnComplete(() =>
                {
                    Debug.Log($"MoveTo: Animation completed. Triggering: {completeTriggerName}");
                    if (setTriggerOnComplete && !string.IsNullOrEmpty(completeTriggerName))
                    {
                        _animator.SetTrigger(completeTriggerName);
                        Debug.Log($"MoveTo: Trigger '{completeTriggerName}' sent successfully");
                    }
                });
        }
    }
    
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Optional: Add any per-frame logic here if needed
        // For example, you could check if the object should stop moving based on some condition
    }
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("MoveTo: State exiting - cancelling any ongoing tween");
        // Cancel any ongoing tween if the state is exited early
        LeanTween.cancel(animator.gameObject);
    }
}
