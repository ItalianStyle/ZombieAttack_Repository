using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace ZombieAttack
{
    public class EnemyManager : MonoBehaviour
    {
        public static event Action OnWaveKilled = delegate { };
        public static event Action OnAllWavesKilled = delegate { };

        public int CurrentWave
        {
            get => _currentWave;

            private set
            {
                _currentWave = value;
                if (_currentWave >= waves.Length)
                    _currentWave = waves.Length - 1;
                else if (_currentWave < 0)
                    _currentWave = 0;
            }
        }
        public bool IsLastWave => CurrentWave == waves.Length - 1;
        public bool IsFirstWave => CurrentWave == 0;

        public static EnemyManager instance;

        [SerializeField] List<Transform> spawnPoints = null;
        [SerializeField] Wave[] waves = null;

        Transform finalObjectiveTransform = null;
        int _currentWave = 0;
        int killedEnemies = 0;       
        int[] spawnedEnemies;
        List<GameObject> activeEnemies = new List<GameObject>();

        private void OnEnable()
        {
            if (instance != null && instance != this)
                DestroyImmediate(gameObject);
            else
            {
                instance = this;
                SceneManager.sceneLoaded += OnSceneLoaded;
            }              
        }      

        private void OnDisable()
        {
            Timer.OnPreparationTimerFinished -= SpawnWave;
            WaveBeginnerFlag.OnStartEnemyWave -= (timeToEndWave) => SpawnWave();
            CancelInvoke(nameof(SpawnEnemy));
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            if(scene.buildIndex == 1)
            { 
                finalObjectiveTransform = GameObject.FindGameObjectWithTag("Finish").transform;
                //Initialize Spawn points
                for (int i = 0; i < transform.childCount; i++)
                    spawnPoints.Add(transform.GetChild(i));

                CurrentWave = 0;
                GetWavesFromAssets();
                foreach (Wave wave in waves)
                    wave.InitializeEnemyTypesIndexList();

                spawnedEnemies = new int[waves[CurrentWave].maxEnemyTypes.Length];
                activeEnemies = new List<GameObject>();

                Timer.OnPreparationTimerFinished += () =>
                {
                    CurrentWave++;
                    UI_Manager.instance.PlayWaveTextAnimation(false);
                };

                WaveBeginnerFlag.OnStartEnemyWave += (_) => SpawnWave();

                GameManager.GameRestarted += RestartFromWave;
            }
        }

        //Looks for wave assets
        private void GetWavesFromAssets()
        {
            string[] guids;

            // search for a ScriptObject called ScriptObj
            guids = AssetDatabase.FindAssets("t:Wave", new[] { "Assets/Scriptable_objects/Enemy_Waves" });
            
            for(int i = 0; i < guids.Length; i++)
            {
                string wavePath = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (wavePath.Contains("wave_" + (i+1).ToString()))
                    waves[i] = (Wave)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[i]), typeof(Wave));
            }
        }

        private void RestartFromWave(int waveIndex)
        {
            //GameManager.PrintExecutionLocation(this);
            foreach (GameObject enemy in activeEnemies)
                enemy.SetActive(false);

            GetWavesFromAssets();
            foreach (Wave wave in waves)
                wave.InitializeEnemyTypesIndexList();

            CurrentWave = waveIndex;
            spawnedEnemies = new int[waves[CurrentWave].maxEnemyTypes.Length];
            activeEnemies = new List<GameObject>();
            
            //Start the wave
            UI_Manager.instance.PlayWaveTextAnimation(isVictoryText: false);
        }

        public void SpawnWave()
        {
            //GameManager.PrintExecutionLocation(this);
            if (waves.Length > 0)
            {              
                spawnedEnemies = new int[waves[CurrentWave].maxEnemyTypes.Length];
                InvokeRepeating(nameof(SpawnEnemy), 0f, waves[CurrentWave].timeBetweenSpawns);
            }
        }

        void SpawnEnemy()
        {
            //GameManager.PrintExecutionLocation(this);
            //Choose enemy type to spawn
            int enemyTypeIndex = waves[CurrentWave].SelectEnemyType();

            //If the list of callable type of enemies is not empty
            if (enemyTypeIndex != -1)
            {
                //If spawned enemies haven't reached the maximum number allowed
                if (spawnedEnemies[enemyTypeIndex] < waves[CurrentWave].maxEnemyTypes[enemyTypeIndex])
                {
                    GameObject enemy; 
                    switch(enemyTypeIndex)
                    {
                        case 0:
                            enemy = ObjectPooler.SharedInstance.GetPooledObject("Enemy", "EnemySmall");
                            break;

                        case 1:
                            enemy = ObjectPooler.SharedInstance.GetPooledObject("Enemy", "EnemyMedium");
                            break;

                        case 2:
                            enemy = ObjectPooler.SharedInstance.GetPooledObject("Enemy", "EnemyBig");
                            break;

                        default:
                            Debug.LogWarning("Indice del tipo di nemico non riconosciuto!");
                            enemy = null;
                            break;
                    }
                    SetupEnemy(enemy);
                    //Debug.Log("Nemico è attivo");
                    enemy.SetActive(true);
                    //Debug.Log("Aggiungo il nemico alla lista dei nemici attivi");
                    activeEnemies.Add(enemy);
                    //Increase enemy counting for this type
                    spawnedEnemies[enemyTypeIndex]++;
                }
                //else if the list of callable type of enemies is empty, remove the index from the list and restart the method
                else
                {
                    waves[CurrentWave].DiscardEnemyType(enemyTypeIndex);
                    SpawnEnemy();
                }
            }
            else
            {
                //Debug.LogWarning("List of callable type of enemies is empty");
                CancelInvoke(nameof(SpawnEnemy));
            }              
        }

        private void SetupEnemy(GameObject enemy)
        {
            enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //Choose spawnpoint
            enemy.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
            enemy.GetComponent<EnemyMovement>().SetDestination(enemy.CompareTag("EnemyMedium")? GameManager.instance.player.transform : finalObjectiveTransform);          
            enemy.GetComponent<Health>().OnEnemyDead += IncreaseKillCount;
        }

        private void IncreaseKillCount(Health enemyHealth)
        {
            //GameManager.PrintExecutionLocation(this);
            if (activeEnemies.Contains(enemyHealth.gameObject))
            {
                activeEnemies.Remove(enemyHealth.gameObject);
                killedEnemies++;
                enemyHealth.OnEnemyDead -= IncreaseKillCount;

                //If wave is finished
                if (killedEnemies >= waves[CurrentWave].MaxEnemies)
                {
                    killedEnemies = 0;
                    if (IsLastWave)
                    {
                        //Vittoria del gioco
                        OnAllWavesKilled.Invoke();
                    }
                    else
                    {
                        //Vittoria dell'ondata
                        OnWaveKilled.Invoke();
                    }
                }
            }
            else
            {
                Debug.LogError("Il nemico non cè nella lista dei nemici attivi, non si può rimuovere");
            }
        }
    }
}