using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieAttack
{ 
    public class Turret : MonoBehaviour
    {
        public event Action<Turret> OnTurretEnabled = delegate { };
        public event Action<Turret> OnTurretDisabled = delegate { };

        List<EnemyMovement> enemiesOnSight;

        [Header("Stats")]
        [SerializeField] float damage;
        [SerializeField] float reloadTime = .1f;
        [SerializeField] float maxRange = 10f;
        [SerializeField] float rotationSpeed = 1f;
        public int buildingCost = 1;
        public int sellingCost = 1;
        private float timer;

        MeshRenderer turretMeshRenderer;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(255, 255, 0, .25f);          
            Gizmos.DrawSphere(transform.position, maxRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, maxRange);
        }

        private void Awake()
        {
            turretMeshRenderer = GetComponent<MeshRenderer>();
        }

        private void OnEnable()
        {
            turretMeshRenderer.material.color = Color.green;

            enemiesOnSight = new List<EnemyMovement>();
            GetComponent<SphereCollider>().radius = maxRange;
            timer = reloadTime;

            OnTurretEnabled.Invoke(this);
        }

        private void Update()
        {
            timer += Time.deltaTime;
        }

        private void OnDisable()
        {
            turretMeshRenderer.material.color = Color.gray;
            enemiesOnSight.Clear();

            OnTurretDisabled.Invoke(this);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!enabled) return;

            if (enemiesOnSight.Count == 0)
            {
                if (other.TryGetComponent(out EnemyMovement enemy))
                {
                    if (enemy != null && !enemiesOnSight.Contains(enemy))
                    {
                        enemy.GetComponent<Health>().OnEnemyDead += RemoveEnemyFromList;
                        enemiesOnSight.Add(enemy);
                    }
                }
            }
            else if (enemiesOnSight.Count > 0)
            {
                LockOnTarget(enemiesOnSight[0].transform);
                if (timer >= reloadTime)
                {
                    timer = 0f;
                    Shoot();
                }
            }                  
        }        

        private void OnTriggerExit(Collider other)
        {
            if (enabled && other.TryGetComponent(out EnemyMovement enemy))
                RemoveEnemyFromList(enemy.GetComponent<Health>());           
        }

        //how to rotate towards targets: https://www.youtube.com/watch?v=QKhn2kl9_8I&t=793s
        private void LockOnTarget(Transform _target)
        {
            if(_target == null)
                return;
  
            Vector3 direction = _target.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
            transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        }

        private void Shoot()
        {
            Ray ray = new Ray(transform.position, enemiesOnSight[0].transform.position - transform.position);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, maxRange, LayerMask.GetMask("Enemy", "Building"))) //Filtering layers to consider in raycast: https://answers.unity.com/questions/1108781/set-ray-only-when-raycast-a-specific-layer.html
            {
                if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                    hitInfo.collider.GetComponent<Health>().DealDamage(damage);
            }
        }

        private void RemoveEnemyFromList(Health enemyToRemove)
        {
            EnemyMovement enemy = enemyToRemove.GetComponent<EnemyMovement>();
            if (enemiesOnSight.Contains(enemy))
                enemiesOnSight.Remove(enemy);           
        }
    }
}