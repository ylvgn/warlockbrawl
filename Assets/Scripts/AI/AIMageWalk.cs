using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMageWalk : StateMachineBehaviour
{
    AIController AIController;
    Vector3 dir;
    float distance;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Random.InitState((int)Time.time % System.Int32.MaxValue);
        AIController = animator.gameObject.GetComponent<AIController>();
        dir = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        distance = Random.Range(5, 10);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   
        AIController.Walk(dir, distance);
    }
}
