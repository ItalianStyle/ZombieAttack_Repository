using System;
using UnityEngine;

namespace ZombieAttack
{
    public class PauseListener : MonoBehaviour
    {
        public static event Action OnPauseKeyPressed = delegate { };

        //Im in playing the game state, don't listen to GameResumed event anymore
        private void OnEnable()
        {
            GameManager.GameResumed -= () => enabled = true;
            GameManager.GameRestarted += (_) => enabled = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                OnPauseKeyPressed.Invoke();
                enabled = false;
            }
        }

        //Im in pause state, return to listen pause key when game is resumed
        private void OnDisable()
        {
            GameManager.GameResumed += () => enabled = true;
            GameManager.GameRestarted += (_) => enabled = true;
        }
    }
}