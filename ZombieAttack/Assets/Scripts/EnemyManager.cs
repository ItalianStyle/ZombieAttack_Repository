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
        [SerializeField] Wave[] waves;
        int currentWave = 0;

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
            
            currentWave = 0;
            SpawnNextWave(); 
        }

        void SpawnNextWave()
        {
            if(currentWave > 0 && currentWave < waves.Length)
                waves[currentWave++].SpawnEnemy(objPooler, spawnPoints, finalObjectiveTransform);
            else
            {
                Debug.Log("Current wave fuori dal range: " + currentWave);
            }
        }
    }
}