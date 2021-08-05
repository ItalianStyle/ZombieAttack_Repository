using System.Collections;
using UnityEngine;

namespace ZombieAttack
{
    //Questa classe è usata dal waveText per cominciare l'ondata di nemici dopo la scritta
    public class WaveBeginnerFlag : MonoBehaviour
    {
        [SerializeField] int timeToStartWave = 1;

        //Called at the end of WaveText_appearing animation
        public void StartEnemyWave() => EnemyManager.instance.SpawnWave();

        //Called at the end of WaveText_Won_appearing animation
        public void StartTimer()
        {
            UI_Manager.instance.SetTimerText(true);
            StartCoroutine(WaitForNextWave());
        }

        IEnumerator WaitForNextWave()
        {
            for(int timer = timeToStartWave; timer > 0;)
            {
                UI_Manager.instance.UpdateTimeText(timer);
                yield return new WaitForSeconds(1);
                timer--;
            }
            //Tolgo il timer
            UI_Manager.instance.SetTimerText(false);
            //Faccio partire l'animazione del testo di presentazione ondata
            UI_Manager.instance.PlayWaveText(isVictoryText: false);
        }
    }
}