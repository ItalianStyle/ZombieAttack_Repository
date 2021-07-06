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
        [SerializeField] float timeBetweenSpawns = 1f;
        [SerializeField] int maxEnemies = 10;

        int currentEnemies = 0;
        public int killedEnemies = 0;
        ObjectPooler objPooler;

        //public static EnemySpawnManager instance;

        private void Awake()
        {
            /*if (instance is null)
            {
                instance = this;
            }
            else if (instance != this)
                Destroy(this.gameObject);  
            */
            finalObjectiveTransform = GameObject.FindGameObjectWithTag("Finish").transform;
            objPooler = GameObject.Find("Enemies").GetComponent<ObjectPooler>();
        }

        private void Start()
        {
            for(int i = 0; i < transform.childCount; i++)
                spawnPoints.Add(transform.GetChild(i));
            
            currentEnemies = 0;
            SpawnEnemy();
        }

        void SpawnEnemy()
        {
            GameObject enemy = objPooler.GetPooledObject("Enemy");
            
            //Choose spawnpoint
            enemy.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;

            enemy.GetComponent<EnemyMovement>().SetDestination(finalObjectiveTransform);
            enemy.GetComponent<Health>().OnEnemyDead += IncreaseKillCount;
            enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;
            enemy.SetActive(true);

            currentEnemies++;
            if(currentEnemies < maxEnemies)
                StartCoroutine(WaitBeforeSpawn());
        }

        private void IncreaseKillCount(Health enemyHealth)
        {
            killedEnemies++;
            enemyHealth.OnEnemyDead -= IncreaseKillCount;
            if(killedEnemies >= maxEnemies)
            {
                //Vittoria
                GameManager.instance.SetStatusGame(GameManager.GameState.Won);
                UI_Manager.instance.SetFinishScreen(GameManager.GameState.Won);
            }
        }

        IEnumerator WaitBeforeSpawn()
        {
            yield return new WaitForSeconds(timeBetweenSpawns);
            SpawnEnemy();
        }
    }
}