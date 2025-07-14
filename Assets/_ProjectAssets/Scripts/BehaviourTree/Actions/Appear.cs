using UnityEngine;

public class Appear : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LeanTween.moveLocalY( animator.gameObject, 2.951164f, 4f).setEaseInQuad().setOnComplete(() => animator.SetTrigger("StartBossBehaviour"));
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
