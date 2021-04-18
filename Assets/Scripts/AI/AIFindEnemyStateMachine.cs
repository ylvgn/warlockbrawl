using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFindEnemyStateMachine : StateMachineBehaviour
{
    AIController AIController;
    Character enemy;
    bool isEnable;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AIController = animator.gameObject.GetComponent<AIController>();
        isEnable = true;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!isEnable) return;
        enemy = AIController.FindNearestEnemy();

        // 如果找到敌人
        if (enemy)
        {
            isEnable = false;
            AIController.Attack(enemy);
        }
    }
}
