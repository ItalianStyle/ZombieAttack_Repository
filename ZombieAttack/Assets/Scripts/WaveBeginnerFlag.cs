using System;
using UnityEngine;

namespace ZombieAttack
{
    //Questa classe è usata dal waveText per cominciare l'ondata di nemici dopo la scritta
    public class WaveBeginnerFlag : MonoBehaviour
    {
        public static event Action<int> OnStartEnemyWave = delegate { };
        public static event Action<int> OnStartingPrepationTimer = delegate { };

        [SerializeField] int timeToStartWave = 1;
        [SerializeField] int timeToEndWave = 7;

        //Called at the end of WaveText_appearing ("Ondata X") animation
        //Start counting down the timer to finish the wave and start spawning next wave
        public void StartEnemyWave()
        {
            //GameManager.PrintExecutionLocation(this);
            OnStartEnemyWave.Invoke(timeToEndWave);
        }

        //Called at the end of WaveText_Won_appearing ("Ondata passata!") animation
        public void StartPreparationTimer() => OnStartingPrepationTimer.Invoke(timeToStartWave);
    }
}