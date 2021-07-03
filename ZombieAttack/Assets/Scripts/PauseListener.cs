using UnityEngine;

namespace Larochiens_Adventure
{
    public class PauseListener : MonoBehaviour
    {
        [SerializeField] bool pauseState = false;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                pauseState = !pauseState;
                GameManager.instance.SetStatusGame(GameManager.GameState.Paused);
                UI_Manager.instance.SetFinishScreen(GameManager.GameState.Paused);
            }
        }
    }
}
