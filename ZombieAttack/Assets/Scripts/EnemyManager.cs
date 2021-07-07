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
        int currentWave = 0;

        ObjectPooler objPooler = null;

        public static EnemyManager instance;

        private void Awake()
        {
            if (instance is null)
                instance = this;
            
            //else if (instance != this) Destroy(gameObject);  

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

        public void SpawnNextWave()
        {
            UI_Manager.instance.PlayWaveText(currentWave);
            if(currentWave > 0 && currentWave < waves.Length)
                waves[currentWave++].SpawnEnemy(objPooler, spawnPoints, finalObjectiveTransform);
            else
            {
                Debug.Log("Current wave fuori dal range: " + currentWave);
            }
        }
    }
}