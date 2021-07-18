using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieAttack
{ 
    public class Turret : MonoBehaviour
    {
        public bool isActive = false;
        List<EnemyMovement> enemiesOnSight;

        private void Start()
        {
            enemiesOnSight = new List<EnemyMovement>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isActive && other.TryGetComponent(out EnemyMovement enemy))
            {
                if (!enemiesOnSight.Contains(enemy))             
                    enemiesOnSight.Add(enemy);              
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (isActive && enemiesOnSight.Count > 0)
                transform.LookAt(enemiesOnSight[0].transform);
                    
        }

        private void OnTriggerExit(Collider other)
        {
            if (isActive && other.TryGetComponent(out EnemyMovement enemy))
            {
                if (enemiesOnSight.Contains(enemy))
                {
                    enemiesOnSight.Remove(enemy);
                    if(enemiesOnSight.Count == 0)
                        transform.localRotation = Quaternion.identity;
                }
            }
        }
    }
}