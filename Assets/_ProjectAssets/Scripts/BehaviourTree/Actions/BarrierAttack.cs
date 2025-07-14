using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

public class BarrierAttack : StateMachineBehaviour
{
    public GameObject littleBarrier;
    public GameObject mediumBarrier;
    public GameObject bigBarrier;

    public float timeBetweenSpawnBarrier = 1f;

    public Constants.BarrierSet[] barierSet;
    private GameObject barier;
    private BossController _bossController;
    private Animator _animator;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _bossController = animator.GetComponent<BossController>();
        _animator = animator;
        Attack();
    }
    
    
    public void Attack()
    {

        UniTask.Void(async () =>
        {

            await _bossController.ActivateShield();

            foreach (var t in barierSet)
            {
                switch (t.barrierPosition)
                {
                    case Constants.BarrierPosition.Left:
                        {
                            DefineBarrierType(t.barrierType);
                            GameObject newBarrier =
                                Instantiate(barier, _bossController.leftBarrierSpawnPPoint.position, Quaternion.identity);
                            newBarrier.GetComponent<BarierBehaviour>().Appear(Constants.BarrierPosition.Left);
                            break;
                        }

                    case Constants.BarrierPosition.Right:
                        {
                            DefineBarrierType(t.barrierType);
                            GameObject newBarrier =
                                Instantiate(barier, _bossController.rightBarrierSpawnPPoint.position, Quaternion.identity);
                            newBarrier.GetComponent<BarierBehaviour>().Appear(Constants.BarrierPosition.Right);
                            break;
                        }
                }

                await UniTask.Delay((int)(timeBetweenSpawnBarrier * 1000));
            }

            await _bossController.DeactivateShield();
            _animator.SetTrigger("CanExitState");
        });
    }

    private void DefineBarrierType(Constants.BarrierType barrierType)
    {
        switch (barrierType)
        {
            case Constants.BarrierType.LittleBarrier:
                {
                    barier = littleBarrier;
                    break;
                }
            case Constants.BarrierType.MediumBarrier:
                {
                    barier = mediumBarrier;
                    break;
                }

            case Constants.BarrierType.BigBarrier:
                {
                    barier = bigBarrier;
                    break;
                }
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

}
