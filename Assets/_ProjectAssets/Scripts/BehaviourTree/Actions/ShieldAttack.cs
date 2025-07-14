using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAttack : StateMachineBehaviour
{
    private BossController _bossController;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _bossController = animator.GetComponent<BossController>();
        _bossController.SpawnEnemiesFromBelow();
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    
}
