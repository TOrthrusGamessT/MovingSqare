using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Destroy : StateMachineBehaviour
{
    BossController _bossController;
    Animator _animator;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _bossController = animator.GetComponent<BossController>();
        _animator = animator;
        Death();
    }

    private void Death()
    {
        _bossController.GetComponent<BoxCollider2D>().enabled = false;
        Rigidbody2D _rb = _bossController.GetComponent<Rigidbody2D>();
        _rb.gravityScale = 1;
        _rb.linearDamping = 16;
        foreach (var particleSystem in _bossController.destroyEffect)
        {
            particleSystem.Play();
        }

        
        LeanTween.scale(_bossController.rewardText, new Vector3(1, 1, 1), 1f).setEaseInQuad().setOnComplete(() =>
        {
            DataManager.MoneyCollected = 500;
            UIManagerGameRoom.instance.FinishLvlState();
        });
        LeanTween.rotate(_bossController.gameObject, new Vector3(0, 0, 180), 30f).setEaseInQuad()
        .setOnComplete(() =>
        {
            _animator.SetTrigger("CanExitState");
            return;
        });
        
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
