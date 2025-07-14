using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class LaserAttack : StateMachineBehaviour
{
    public float laserDuration = 2f; // Duration for which the lasers will be active
    private BossController _bossController;
    private Animator _animator;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _bossController = animator.GetComponent<BossController>();
        foreach (var laser in _bossController.lasers)
        {
            laser.Play();
        }
        _animator = animator;
        LeaveState(laserDuration);
    }

    private async void LeaveState(float delay)
    {
        await UniTask.Delay((int)(delay * 1000));
        _animator.SetTrigger("CanExitState");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Laser Attack Update");
    }
    
     override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
     {
         
     }


}
