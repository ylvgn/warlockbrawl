using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIIdleStateMachine : StateMachineBehaviour
{
    float clipLength;
    float frameRate;
    float totalFrame;
    float curFrame;
    AnimationClip animationClip;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("CanWalk", false);
        animator.SetBool("FindEnemy", false);
        // var AIController = animator.gameObject.GetComponent<AIController>();
        // AnimationClip animationClip = animator.runtimeAnimatorController.animationClips.Where(clip => clip.name == "Idle").FirstOrDefault(); // using System.Linq
        // or AnimationClip a = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        //Debug.Log("animationClipName= " + animationClip.name);
        animationClip = animator.runtimeAnimatorController.animationClips[0];
        if (animationClip) {
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
                animator.SetBool("CanWalk", t % 5 == 0);
                animator.SetBool("FindEnemy", t % 6 == 0);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 结束时候判断1次
        if (animationClip)
        {
            int t = Random.Range(0, 1000);
            animator.SetBool("CanWalk", t % 10 == 0);
            animator.SetBool("FindEnemy", t % 3 == 0);
        }
    }
}
