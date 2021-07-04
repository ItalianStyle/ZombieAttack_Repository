﻿using UnityEngine;
using UnityEngine.AI;

namespace ZombieAttack
{
    public class EnemyMovement : MonoBehaviour
    {
        NavMeshAgent enemyAgent;

        [SerializeField] GameObject destination;

        private void Awake()
        {
            enemyAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            enemyAgent.SetDestination(destination.transform.position);
        }
    }
}