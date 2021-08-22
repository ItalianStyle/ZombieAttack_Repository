using UnityEngine;

namespace ZombieAttack
{
    public class PlayTurretStateAudio : StateMachineBehaviour
    {
        readonly string[] turretActiveStateNames = { "StartShooting", "Shooting", "EndShooting" };
        [SerializeField] AudioClip[] turretActive_SFXs = new AudioClip[3];
        
        AudioSource turretAudioSource = null;
        
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(!turretAudioSource)
                turretAudioSource = animator.GetComponent<AudioSource>();

            for(int i = 0; i < turretActiveStateNames.Length; i++)
            {
                if (stateInfo.IsName(turretActiveStateNames[i]))
                {
                    MyAudioManager.instance.PlayAudioSourceWithClip(turretAudioSource, turretActive_SFXs[i], false);
                    break;
                }
            }     
        }
    }
}