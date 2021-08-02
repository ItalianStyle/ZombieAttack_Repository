using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticles : StateMachineBehaviour
{
    ParticleSystem muzzleFlashPS = null;
    ParticleSystem bulletsPS = null;
    Light muzzleLight = null;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (muzzleFlashPS is null)
        {
            muzzleFlashPS = animator.transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>();
            muzzleLight = muzzleFlashPS.GetComponentInChildren<Light>();
        }

        if(bulletsPS is null)
            bulletsPS = animator.transform.GetChild(1).GetChild(1).GetComponent<ParticleSystem>();

        muzzleLight.enabled = true;

        muzzleFlashPS.Play();
        bulletsPS.Play();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        muzzleFlashPS.Stop();
        bulletsPS.Stop();
        muzzleLight.enabled = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
