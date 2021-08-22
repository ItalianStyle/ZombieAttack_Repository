using System;
using System.Diagnostics;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Debug = UnityEngine.Debug;

namespace ZombieAttack
{
    public class GameManager : MonoBehaviour
    {
        public enum EntityType { Player, FinalObjective, Enemy }
     
        public static event Action GamePaused;
        public static event Action GameResumed;
        public static event Action<int> GameRestarted; //int waveIndex

        [Header("Button References")]
        [SerializeField] Button playButton = null;
        [SerializeField] Button restartWaveButton = null;
        [SerializeField] Button resumeButton = null;
        [SerializeField] Button exitGameButton = null;
        //[SerializeField] Button skipTutorialButton = null;

        public GameObject player = null;
        Vector3 startPlayerPosition;
        Quaternion startPlayerRotation;
        
        public enum GameState { Won, Lost, Paused, Resumed, WaveWon, Restarted, notDefined }
        public GameState currentGameState = GameState.notDefined;
       
        public bool isPaused = false;

        public static GameManager instance;

        private void OnEnable()
        {
            //Verifica se esiste un'altra istanza del GameManager nella scena
            if (instance != null && instance != this)
                DestroyImmediate(gameObject);
            else
            {
                instance = this;
                DontDestroyOnLoad(this);
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
        }

        private void OnDisable()
        {
            PauseListener.OnPauseKeyPressed -= () => SetStatusGame(GameState.Paused);
            Timer.OnWaveTimerFinished -= () => SetStatusGame(GameState.Lost);
            EnemyManager.OnWaveKilled -= () => SetStatusGame(GameState.WaveWon);
            EnemyManager.OnAllWavesKilled -= () => SetStatusGame(GameState.Won);
            Health.OnPlayerDead -= () => SetStatusGame(GameState.Lost);
            Health.OnObjectiveDestroyed -= () => SetStatusGame(GameState.Lost);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            //Togli la eventuale pausa da un tentativo precendente del giocatore
            Time.timeScale = 1f;
            //Disattiva il boss
            //isBossActive = false;

            //Prendi tutte le reference della scena attuale
            GetReferences(scene.buildIndex);

            switch (scene.buildIndex)
            {
                case 0:
                    playButton.onClick.AddListener(LoadGame);
                    exitGameButton.onClick.AddListener(QuitGame);
                    break;

                case 1:
                    //Save player position at the begin of the game
                    startPlayerPosition = player.transform.position;
                    startPlayerRotation = player.transform.rotation;

                    //SetStatusGame(GameState.BeginGameWithTutorial);
                    SetStatusGame(GameState.Resumed);
                    
                    //Prepara i bottoni dei menu
                    playButton.onClick.AddListener(() => RestartGameFrom(0));
                    restartWaveButton.onClick.AddListener(() => RestartGameFrom(EnemyManager.instance.CurrentWave));
                    resumeButton.onClick.AddListener(ResumeGame);
                    exitGameButton.onClick.AddListener(MainMenu);

                    //skipTutorialButton.onClick.AddListener(delegate { SetMousePointer(false); });
                    
                    Wallet.InitializeWalletInstance(100);

                    PauseListener.OnPauseKeyPressed += () => SetStatusGame(GameState.Paused);

                    Timer.OnWaveTimerFinished += () => SetStatusGame(GameState.Lost);

                    EnemyManager.OnWaveKilled += () => SetStatusGame(GameState.WaveWon);
                    EnemyManager.OnAllWavesKilled += () => SetStatusGame(GameState.Won);

                    Health.OnPlayerDead += () => SetStatusGame(GameState.Lost);
                    Health.OnObjectiveDestroyed += () => SetStatusGame(GameState.Lost);
                    break;

                default:
                    Debug.LogError("Indice di scena non riconosciuto");
                    break;
            }
        }

        //Prendi tutti i riferimenti in base alla scena corrente
        private void GetReferences(int sceneIndex)
        {
            playButton = GameObject.FindGameObjectWithTag("PlayButton").GetComponent<Button>();
            exitGameButton = GameObject.FindGameObjectWithTag("QuitGameButton").GetComponent<Button>();
            switch (sceneIndex)
            {
                case 0:
                    break;

                case 1:
                    player = GameObject.FindGameObjectWithTag("Player");

                    resumeButton = GameObject.FindGameObjectWithTag("ResumeButton").GetComponent<Button>();
                    restartWaveButton = GameObject.FindGameObjectWithTag("RestartWaveButton").GetComponent<Button>();
                    //skipTutorialButton = GameObject.FindGameObjectWithTag("SkipButton").GetComponent<Button>();

                    //inputToCamera = FindObjectOfType<CinemachineFreeLook>();
                    break;

                default:
                    Debug.LogError("Indice di scena non riconosciuto");
                    break;
            }
        }

        private void RestartGameFrom(int waveIndex)
        {
            Time.timeScale = 1f;
            //Reactivate player when he loses the round
            if(!player.activeInHierarchy)
                player.SetActive(true);

            //Reset player position
            CanEnablePlayer(false);
            player.transform.SetPositionAndRotation(startPlayerPosition, startPlayerRotation);
            CanEnablePlayer(true);
            
            
            SetStatusGame(GameState.Restarted, waveIndex);
        }

        public void LoadGame() => SceneManager.LoadScene("Gioco");

        public void ResumeGame()
        {
            //Riabilito il timeScale
            Time.timeScale = 1f;
            SetStatusGame(GameState.Resumed);
        }

        public void MainMenu()
        {
            Destroy(EnemyManager.instance);
            SceneManager.LoadScene(0);
        }

        public void QuitGame() => Application.Quit();

        //Wave index is used when Restarting the game
        public void SetStatusGame(GameState newGameState, int waveIndex = -1)
        {
            currentGameState = newGameState;
            //SetCamera(currentGameState is GameState.Resumed);
            PrepareForOffGameWindow(currentGameState != GameState.Resumed && currentGameState != GameState.Restarted && currentGameState != GameState.WaveWon);

            switch(currentGameState)
            {
                case GameState.Paused:
                    isPaused = true;
                    //Informo che il gioco è in pausa
                    GamePaused?.Invoke();
                    break;

                case GameState.Resumed:
                    isPaused = false;
                    //Informo che il gioco è ripreso
                    GameResumed?.Invoke();
                    break;

                case GameState.Restarted:
                    if (waveIndex < 0)
                        Debug.LogError("Ti sei dimenticato di dare come parametro anche l'indice di ondata da cui ricominciare");
                    else
                        GameRestarted?.Invoke(waveIndex);
                    break;
            }
        }

        //If we are leaving the match (pause, endscreen), free the mouse and disable the player, viceversa otherwise
        private void PrepareForOffGameWindow(bool canShowMenu)
        {
            //Libero / Blocco il mouse
            SetMousePointer(canShowMenu);

            //Disabilito / Abilito il giocatore
            CanEnablePlayer(!canShowMenu);
        }

        //Enable/Disable the player in game
        public void CanEnablePlayer(bool canEnable)
        {
            //Attiva il movimento
            player.GetComponent<CharacterController>().enabled = canEnable;
            player.GetComponent<PlayerMovement>().enabled = canEnable;

            player.GetComponent<StaminaSystem>().enabled = canEnable;
            player.GetComponent<PlayerShooting>().enabled = canEnable;
        }

        //Se true, libera il mouse, se false il mouse viene bloccato
        private static void SetMousePointer(bool canUnlockMouse)
        {
            Cursor.lockState = canUnlockMouse ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = canUnlockMouse;
        }

        public static void PrintExecutionLocation(object useThisKeyword, string language = "it")
        {

            string message;
            string id = " (ID: " + useThisKeyword.GetHashCode() + ")";
            //Define language to print message
            switch (language)
            {
                case "en":
                    message = new StackTrace(1).GetFrame(0).GetMethod().Name + "() of " + useThisKeyword.GetType().FullName + id;
                    break;
                case "it":
                    message = new StackTrace(1).GetFrame(0).GetMethod().Name + "() di " + useThisKeyword.GetType().FullName + id;
                    break;
                default:
                    message = "Language not known";
                    break;
            }
            Debug.Log(message);
        }
        /*

        //Source: https://answers.unity.com/questions/805199/how-do-i-scale-a-gameobject-over-time.html
        //Coroutine per far crescere i pupazzi di neve
        public static IEnumerator GrowObjectScale(GameObject objectToScale, float time, Vector3 maxScale, Vector3 bufferMaxScale)
        {
            float currentTime = 0.0f;

            Vector3 originalScale = objectToScale.transform.localScale;
            do
            {
                objectToScale.transform.localScale = Vector3.Lerp(originalScale, maxScale, currentTime / time);
                currentTime += Time.deltaTime;
                yield return null;
            } while ((currentTime <= time) || objectToScale.transform.localScale.x < maxScale.x - bufferMaxScale.x);
        }
     */
    }
}