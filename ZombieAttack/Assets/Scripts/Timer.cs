using System;
using UnityEngine;

namespace ZombieAttack
{
    public class Timer : StateMachineBehaviour
    {
        public static event Action OnWaveTimerFinished = delegate { };
        public static event Action OnPreparationTimerFinished = delegate { };
        bool isRestarting = false;

        // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GameManager.GameRestarted += (_) =>
            {
                //Reset timer
                animator.SetTrigger("StopTimer");
                animator.SetFloat("Time", 0f);
                isRestarting = true;

                
            };
            EnemyManager.OnWaveKilled += () => animator.SetTrigger("StopTimer");
        }

        // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.IsName("TimerText_Active") || stateInfo.IsName("TimerText_BlinkingActive"))
            {
                animator.SetFloat("Time", animator.GetFloat("Time") - Time.deltaTime);

                if (stateInfo.IsName("TimerText_Active"))
                    UI_Manager.instance.SetTimerText((animator.GetFloat("Time")).ToString("0"));

                else if (stateInfo.IsName("TimerText_BlinkingActive"))
                    UI_Manager.instance.SetTimerText((animator.GetFloat("Time")).ToString("0.0"));
            }
        }

        // OnStateExit is called before OnStateExit is called on any state inside this state machine
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Time for the wave is out
            if (!isRestarting && stateInfo.IsName("TimerText_BlinkingActive"))
            {
                if (animator.GetBool("IsWaveTimer"))
                    OnWaveTimerFinished.Invoke();
                else
                    OnPreparationTimerFinished.Invoke();
                EnemyManager.OnWaveKilled -= () => animator.SetTrigger("StopTimer");
                /*GameManager.GameRestarted += (waveIndex) =>
                {
                    animator.SetFloat("Time", 0f);
                    animator.SetTrigger("StopTimer");
                };*/
            }         
        }

        // OnStateMove is called before OnStateMove is called on any state inside this state machine
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateIK is called before OnStateIK is called on any state inside this state machine
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateMachineEnter is called when entering a state machine via its Entry Node
        //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        //{
        //    
        //}

        // OnStateMachineExit is called when exiting a state machine via its Exit Node
        //override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        //{
        //      
        //}
    }
}