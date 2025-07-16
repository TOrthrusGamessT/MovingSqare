using Cysharp.Threading.Tasks;
using UnityEngine;

public class SendEnemiesFromBelow : StateMachineBehaviour
{

    private BossController _bossController;
    private Animator _animator;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _bossController = animator.GetComponent<BossController>();
        _animator = animator;
        SpawnEnemiesFromBelow();
    }



    public void SpawnEnemiesFromBelow()
    {
        UniTask.Void(async () =>
        {

            await _bossController.ActivateShield();

            await _bossController.spawnManagerLvl.SpawnEnemiesFromBelow();

            await _bossController.DeactivateShield();
            _animator.SetTrigger("CanExitState");
        });
    }


    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
    
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // This method can be used for continuous updates if needed
    }
    
}
