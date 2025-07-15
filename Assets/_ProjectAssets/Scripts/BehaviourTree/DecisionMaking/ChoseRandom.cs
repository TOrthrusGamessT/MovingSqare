using UnityEngine;

public class ChoseRandom : StateMachineBehaviour
{
    public int optionsAvailable = 3; // Number of options to choose from
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int randomChoice = Random.Range(0, optionsAvailable);
        animator.SetInteger("RandomChoice", randomChoice);
        Debug.Log(randomChoice);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // This method is called on each frame while the state is active.
        // You can implement logic to update the state based on random choices.
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // This method is called when the state machine exits this state.
        Debug.Log("Exiting ChoseRandom state.");
    }

}
