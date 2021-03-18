using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIMageIdle : StateMachineBehaviour
{
    AIController AIController;
    Vector3 dir;
    float clipLength;
    float frameRate;
    float totalFrame;
    AnimationClip animationClip;
    float curFrame;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AIController = animator.gameObject.GetComponent<AIController>();
        // AnimationClip animationClip = animator.runtimeAnimatorController.animationClips.Where(clip => clip.name == "Idle").FirstOrDefault();
        // AnimationClip a = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        animationClip = animator.runtimeAnimatorController.animationClips[0];
        if (animationClip) {
            //Debug.Log("animationClipName= " + animationClip.name);
            clipLength = stateInfo.length;
            frameRate = animationClip.frameRate; // 帧(FPS)
            totalFrame = clipLength * frameRate; // 总帧数 = 帧 x 播放总时间
            //Debug.Log("totalFrame=" + totalFrame + " frameRate=" + frameRate + " clipLength=" + clipLength);
        }
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        curFrame ++;
        if (animationClip){
            curFrame = curFrame % totalFrame;
            if (curFrame % 10 == 0) { // 每10帧判断一次
                int t = Random.Range(0, 1000);
                if (t % 5 == 0) {
                    animator.SetBool("CanWalk", true);
                }
            }
        }
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   
        // 结束时候判断1次
        if (animationClip){
            int t = Random.Range(0, 1000);
            animator.SetBool("CanWalk", t % 10 == 0);
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    // override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //    // Implement code that processes and affects root motion
    //     Debug.Log("OnStateMove");
    // }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    // override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //    // Implement code that sets up animation IK (inverse kinematics)
    //    Debug.Log("OnStateIK");
    // }
}
