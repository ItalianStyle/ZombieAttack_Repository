﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ZombieAttack
{
    public class EnemySpawnManager : MonoBehaviour
    {
        [SerializeField] List<Transform> spawnPoints = null;
        [SerializeField] Transform finalObjectiveTransform = null;
        [SerializeField] float timeBetweenSpawns = 1f;
        [SerializeField] int maxEnemies = 10;

        int currentEnemies = 0;
        ObjectPooler objPooler;

        public static EnemySpawnManager instance;

        private void Awake()
        {
            if (instance is null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
                Destroy(this.gameObject);

            finalObjectiveTransform = GameObject.FindGameObjectWithTag("Finish").transform;
            objPooler = GetComponent<ObjectPooler>();
        }

        private void OnEnable()
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                spawnPoints.Add(transform.GetChild(i));
            }
        }

        private void Start()
        {
            SpawnEnemy();
        }

        void SpawnEnemy()
        {
            GameObject enemy = objPooler.GetPooledObject("Enemy");
            
            //Choose spawnpoint
            enemy.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count - 1)].position;

            enemy.GetComponent<EnemyMovement>().SetDestination(finalObjectiveTransform);

            enemy.SetActive(true);
            currentEnemies++;
            if(currentEnemies < maxEnemies)
                StartCoroutine(WaitBeforeSpawn());
        }

        IEnumerator WaitBeforeSpawn()
        {
            yield return new WaitForSeconds(timeBetweenSpawns);
            SpawnEnemy();
        }
    }
}