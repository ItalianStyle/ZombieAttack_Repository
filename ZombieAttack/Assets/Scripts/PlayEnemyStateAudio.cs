using UnityEngine;

namespace ZombieAttack
{
    public class PlayEnemyStateAudio : StateMachineBehaviour
    {
        float timeElapsed = 0f;
        AudioSource enemyAudioSource;
        EnemyAudioPlayer enemyAudioPlayer;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemyAudioSource = animator.GetComponent<AudioSource>();
            enemyAudioPlayer = animator.GetComponent<EnemyAudioPlayer>();
            timeElapsed = 0f;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            timeElapsed += Time.deltaTime;
            if (stateInfo.IsName("Walking"))
            {
                if (!enemyAudioSource.isPlaying && timeElapsed > enemyAudioPlayer.GetBreathSFX.length && !GameManager.instance.isPaused)
                {
                    MyAudioManager.instance.PlayAudioSourceWithClip(audioSource: enemyAudioSource, enemyAudioPlayer.GetBreathSFX, false);
                    timeElapsed = 0f;
                }
            }
            else if(stateInfo.IsName("Attacking"))
            {
                if (!enemyAudioSource.isPlaying && timeElapsed > enemyAudioPlayer.GetAttackSFX.length && !GameManager.instance.isPaused)
                {
                    MyAudioManager.instance.PlayAudioSourceWithClip(audioSource: enemyAudioSource, enemyAudioPlayer.GetAttackSFX, false);
                    timeElapsed = 0f;
                }
            }
        }
    }
}