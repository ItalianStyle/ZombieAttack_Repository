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

        [Header("Stats")]
        [SerializeField] float damage;
        [SerializeField] float reloadTime = .1f;
        [SerializeField] float maxRange = 10f;
        
        private float timer;


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(255, 255, 0, .25f);
            
            Gizmos.DrawSphere(transform.position, maxRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, maxRange);
        }

        private void Start()
        {
            enemiesOnSight = new List<EnemyMovement>();
            GetComponent<SphereCollider>().radius = maxRange;
            timer = reloadTime;
        }

        private void Update()
        {
            timer += Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {

            if (isActive && other.TryGetComponent(out EnemyMovement enemy))
            {
                if (enemy != null && !enemiesOnSight.Contains(enemy))
                {
                    enemy.GetComponent<Health>().OnEnemyDead += RemoveEnemyFromList;
                    enemiesOnSight.Add(enemy);
                }
            }
        }

        private void RemoveEnemyFromList(Health enemyToRemove)
        {
            EnemyMovement enemy = enemyToRemove.GetComponent<EnemyMovement>();
            if (enemiesOnSight.Contains(enemy))
            {
                enemiesOnSight.Remove(enemy);
                if (enemiesOnSight.Count == 0)
                    transform.localRotation = Quaternion.identity;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (isActive && enemiesOnSight.Count > 0)
            {
                transform.LookAt(enemiesOnSight[0].transform);
                if (timer >= reloadTime)
                {
                    timer = 0f;
                    Shoot();
                }
            }
                    
        }

        private void Shoot()
        {
            Ray ray = new Ray(transform.position, enemiesOnSight[0].transform.position - transform.position);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, maxRange, LayerMask.GetMask("Enemy", "Building"))) //Filtering layers to consider in raycast: https://answers.unity.com/questions/1108781/set-ray-only-when-raycast-a-specific-layer.html
            {
                if (hitInfo.collider.CompareTag("Enemy"))
                    hitInfo.collider.GetComponent<Health>().DealDamage(damage);
                else
                    Debug.Log("Non ho colpito il nemico");
            }
            else
                Debug.Log("non ho colpito niente");
        }

        private void OnTriggerExit(Collider other)
        {
            if (isActive && other.TryGetComponent(out EnemyMovement enemy))
            {
                RemoveEnemyFromList(enemy.GetComponent<Health>());
            }
        }
    }
}