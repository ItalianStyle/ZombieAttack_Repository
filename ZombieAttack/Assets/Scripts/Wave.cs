using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System;
using Random = UnityEngine.Random;

namespace ZombieAttack
{
    [CreateAssetMenu(fileName = "New Enemy Wave", menuName = "Enemy Wave")]
    public class Wave : ScriptableObject
    {
        public int maxEnemies;

        [SerializeField] float timeBetweenSpawns = 1f;

        int currentEnemies = 0;
        int killedEnemies = 0;

        public void SpawnEnemy(ObjectPooler objPooler, List<Transform> spawnPoints, Transform finalObjectiveTransform)
        {
            GameObject enemy = objPooler.GetPooledObject("Enemy");

            //Choose spawnpoint
            enemy.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;

            enemy.GetComponent<EnemyMovement>().SetDestination(finalObjectiveTransform);
            enemy.GetComponent<Health>().OnEnemyDead += IncreaseKillCount;
            enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;
            enemy.SetActive(true);

            currentEnemies++;
            if (currentEnemies < maxEnemies)
                WaitBeforeSpawn(timeBetweenSpawns);
        }

        private void IncreaseKillCount(Health enemyHealth)
        {
            killedEnemies++;
            enemyHealth.OnEnemyDead -= IncreaseKillCount;
            if (killedEnemies >= maxEnemies)
            {
                //Vittoria
                GameManager.instance.SetStatusGame(GameManager.GameState.Won);
                UI_Manager.instance.SetFinishScreen(GameManager.GameState.Won);
            }
        }

        private void WaitBeforeSpawn(float time)
        {
            Thread.Sleep((int)time * 1000);
        }
    }
}
