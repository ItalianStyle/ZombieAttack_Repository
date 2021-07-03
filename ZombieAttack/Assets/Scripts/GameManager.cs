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
     /*
        public enum CharacterType { Player, Boss, Enemy }
        public static event Action GamePaused;
        public static event Action GameResumed;

        [Header("Button References")]
        [SerializeField] Button playButton = null;
        [SerializeField] Button resumeButton = null;
        [SerializeField] Button exitGameButton = null;
        [SerializeField] Button skipTutorialButton = null;

        [Header("Mesh References")]
        [SerializeField] GameObject bossDoorClosed = null;
        [SerializeField] GameObject bossDoorOpen = null;

        [SerializeField] GameObject chestClosed = null;
        [SerializeField] MeshRenderer chestOpened = null;

        public GameObject player = null;

        PauseListener pauseListener = null;
        
        public enum GameState { Won, Lost, Paused, Resumed, BeginGameWithTutorial, notDefined }
        public GameState currentGameState = GameState.notDefined;

        public bool isPaused = false;
        public bool isBossActive = false;
     */
        public static GameManager instance;

        private void OnEnable()
        {
            //Verifica se esiste un'altra istanza del GameManager nella scena
            if (instance != null && instance != this)
                Destroy(gameObject);
            else
                instance = this;

            DontDestroyOnLoad(this);

            //SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /*
        private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            //Togli la eventuale pausa da un tentativo precendente del giocatore
            Time.timeScale = 1f;
            //Disattiva il boss
            isBossActive = false;

            //Prendi tutte le reference della scena attuale
            GetReferences(scene.buildIndex);

            playButton.onClick.AddListener(LoadGame);
            switch (scene.buildIndex)
            {
                case 0:
                    GetComponent<ObjectPooler>().enabled = false;
                    exitGameButton.onClick.AddListener(QuitGame);
                    break;

                case 1:
                    GetComponent<ObjectPooler>().enabled = true;
                    SetStatusGame(GameState.BeginGameWithTutorial);
                    
                    //Prepara i bottoni dei menu
                    resumeButton.onClick.AddListener(ResumeGame);
                    exitGameButton.onClick.AddListener(MainMenu);
                    skipTutorialButton.onClick.AddListener(delegate { SetMousePointer(false); });

                    ManageBossDoor(false);
                    break;

                default:
                    Debug.LogError("Indice di scena non riconosciuto");
                    break;
            }
        }

        //Attiva il il movimento del giocatore e il lanciatore di spade a seconda della situazione
        public void CanEnablePlayer(bool canEnable)
        {
            //Attiva il movimento
            player.GetComponent<PlayerMovement>().enabled = canEnable;
            //se è la situazione iniziale dove il giocatore non ha ancora raccolto l'arma, non attivare il lanciatore di spade
            GetComponent<ObjectThrower>().enabled = player.GetComponent<Equipment>().currentWeaponType is Equipment.WeaponType.None ? false : canEnable;
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
                    //pauseListener = FindObjectOfType<PauseListener>();

                    resumeButton = GameObject.FindGameObjectWithTag("ResumeButton").GetComponent<Button>();
                    skipTutorialButton = GameObject.FindGameObjectWithTag("SkipButton").GetComponent<Button>();

                    bossDoorClosed = GameObject.FindGameObjectWithTag("Door_closed");
                    bossDoorOpen = GameObject.FindGameObjectWithTag("Door_open");

                    chestClosed = GameObject.FindGameObjectWithTag("Finish");
                    chestOpened = GameObject.FindGameObjectWithTag("Chest_Open").GetComponent<MeshRenderer>();

                    //inputToCamera = FindObjectOfType<CinemachineFreeLook>();
                    break;

                default:
                    Debug.LogError("Indice di scena non riconosciuto");
                    break;
            }
        }

        public void LoadGame() => SceneManager.LoadScene("Gioco");

        public void ResumeGame()
        {
            //Riabilito il timeScale
            Time.timeScale = 1f;
            SetStatusGame(GameState.Resumed);
        }

        public void MainMenu() => SceneManager.LoadScene(0);

        public void QuitGame() => Application.Quit();

        public void SetStatusGame(GameState newGameState)
        {
            currentGameState = newGameState;
            if (currentGameState != GameState.Resumed)
            {
                //Libero il mouse
                SetMousePointer(true);

                //Disabilito il giocatore
                CanEnablePlayer(false);
            }
            
            else if (currentGameState is GameState.Paused)
                //Informo che il gioco è in pausa
                GamePaused?.Invoke();

            else
            {
                //Blocco il mouse
                SetMousePointer(false);

                //Abilito il giocatore
                CanEnablePlayer(true);
            }
            

            switch (currentGameState)
            {
                case GameState.Won:
                    //Apri lo scrigno del tesoro
                    chestClosed.GetComponent<MeshRenderer>().enabled = false;
                    chestOpened.enabled = true;

                    //Disattivo il trigger per la pausa
                    pauseListener.enabled = false;
                    break;

                case GameState.Lost:
                case GameState.BeginGameWithTutorial:
                case GameState.Paused:
                    //Disattivo il trigger per la pausa
                    pauseListener.enabled = false;

                    //Disattivo l'input della telecamera
                    //SetCamera(false);

                    break;

                case GameState.Resumed:
                    //Attivo il trigger per la pausa
                    pauseListener.enabled = true;

                    //Riattivo l'HUD del gioco
                    //UI_Manager.instance.SetHUD(true);

                    //Attivo l'input della telecamera
                    //SetCamera(true);

                    //Informo che il gioco è ripreso
                    GameResumed?.Invoke();
                    break;
            }
        }

        //Se true, libera il mouse, se false il mouse viene bloccato
        private static void SetMousePointer(bool canUnlockMouse)
        {
            Cursor.lockState = canUnlockMouse ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = canUnlockMouse ? true : false;
        }

        //Apri o chiudi il portone della stanza boss
        public void ManageBossDoor(bool canOpen)
        {
            BoxCollider[] doorColliders = new BoxCollider[2];

            if (bossDoorClosed && bossDoorOpen)
            {
                bossDoorClosed.GetComponent<MeshRenderer>().enabled = !canOpen;
                bossDoorClosed.GetComponent<BoxCollider>().enabled = !canOpen;

                bossDoorOpen.GetComponent<MeshRenderer>().enabled = canOpen;

                doorColliders = bossDoorOpen.GetComponentsInChildren<BoxCollider>();
                if (doorColliders.Length > 0)
                {
                    foreach (BoxCollider collider in doorColliders)
                        collider.enabled = canOpen;
                }
            }
            else
                Debug.LogError("Riferimenti alle porte mancanti! Verifica eventualmente i tag di ricerca");
        }

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