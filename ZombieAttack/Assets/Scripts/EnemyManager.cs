using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ZombieAttack
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] List<Transform> spawnPoints = null;
        [SerializeField] Transform finalObjectiveTransform = null;
        [SerializeField] Wave[] waves = null;
        public int currentWave = 0;
        int currentEnemies = 0;
        int killedEnemies = 0;
        int currentMaxEnemies = 0;

        ObjectPooler objPooler = null;
       
        public static EnemyManager instance;

        private void Awake()
        {
            instance = this;
            finalObjectiveTransform = GameObject.FindGameObjectWithTag("Finish").transform;
            objPooler = GameObject.Find("Enemies").GetComponent<ObjectPooler>();
        }

        private void Start()
        {
            for(int i = 0; i < transform.childCount; i++)
                spawnPoints.Add(transform.GetChild(i));
            
            currentWave = 0;
            currentEnemies = 0;
        }

        public void SpawnWave()
        {           
            if (waves.Length > 0)
            {
                if (currentWave >= 0 && currentWave < waves.Length)
                { 
                    currentMaxEnemies = waves[currentWave].maxEnemies;
                    InvokeRepeating(nameof(SpawnEnemy), 0f, waves[currentWave].timeBetweenSpawns);
                }
                else
                    Debug.Log("Current wave fuori dal range: " + currentWave);
            }
            else
                Debug.Log("Non ci sono ondate!");
            Debug.Log("Spawnata ondata: " + (currentWave+1));
        }

        public void SpawnEnemy()
        {
            GameObject enemy = objPooler.GetPooledObject("Enemy");
            SetupEnemy(enemy);
            enemy.SetActive(true);
            currentEnemies++;
            if (currentEnemies == currentMaxEnemies)
            {
                CancelInvoke(nameof(SpawnEnemy));
                currentWave++;
            }
        }

        private void SetupEnemy(GameObject enemy)
        {
            enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //Choose spawnpoint
            enemy.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
            enemy.GetComponent<EnemyMovement>().SetDestination(finalObjectiveTransform);          
            enemy.GetComponent<Health>().OnEnemyDead += IncreaseKillCount;
        }

        private void IncreaseKillCount(Health enemyHealth)
        {
            killedEnemies++;
            enemyHealth.OnEnemyDead -= IncreaseKillCount;
            if (killedEnemies >= currentMaxEnemies)
            {
                if (currentWave > waves.Length - 1)
                {
                    currentWave = 0;
                    //Vittoria del gioco
                    GameManager.instance.SetStatusGame(GameManager.GameState.Won);
                    UI_Manager.instance.SetFinishScreen(GameManager.GameState.Won);
                }
                else
                {
                    //Vittoria dell'ondata
                    GameManager.instance.SetStatusGame(GameManager.GameState.WaveWon);
                    UI_Manager.instance.SetFinishScreen(GameManager.GameState.WaveWon);
                }
            }
        }
    }
}