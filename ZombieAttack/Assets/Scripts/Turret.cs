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
        public float damage;
        [SerializeField] float maxRange = 10f;
        [SerializeField] float rotationSpeed = 1f;
        public int BuildingCost 
        {
            get => 1;
            private set => _ = value;
        }

        public int SellingCost
        {
            get => 1;
            private set => _ = value;
        }

        MeshRenderer turretMeshRenderer;
        SphereCollider rangeCollider;
        Animator gunAnimator;

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
            rangeCollider = GetComponent<SphereCollider>();
            gunAnimator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            turretMeshRenderer.material.color = Color.green;

            enemiesOnSight = new List<EnemyMovement>();
            rangeCollider.radius = maxRange;

            OnTurretEnabled.Invoke(this);
        }

        private void OnDisable()
        {
            turretMeshRenderer.material.color = Color.gray;
            enemiesOnSight.Clear();
            gunAnimator.SetBool("canActiveTurret", false);

            OnTurretDisabled.Invoke(this);
        }

        private void Update()
        {
            if (enemiesOnSight.Count == 0)
                CanShoot(false);
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
                        enemiesOnSight.Add(enemy);
                        enemy.GetComponent<Health>().OnEnemyDead += RemoveEnemyFromList;                      
                    }
                }
                else
                    CanShoot(false);
            }
            else if (enemiesOnSight.Count > 0)
            {
                LockOnTarget(enemiesOnSight[0].transform);
                Ray ray = new Ray(transform.position, enemiesOnSight[0].transform.position - transform.position);
                CanShoot(Physics.Raycast(ray, out RaycastHit info, maxRange, LayerMask.GetMask("Enemy", "Building", "FinalObjective")) && info.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (enabled && other.TryGetComponent(out EnemyMovement enemy))
            {
                RemoveEnemyFromList(enemy.GetComponent<Health>());
                CanShoot(false);
            }
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

        private void CanShoot(bool canShoot)
        {
            gunAnimator.SetBool("canActiveTurret", canShoot);
        }

        private void RemoveEnemyFromList(Health enemyToRemove)
        {
            EnemyMovement enemy = enemyToRemove.GetComponent<EnemyMovement>();
            if (enemiesOnSight.Contains(enemy))
                enemiesOnSight.Remove(enemy);           
        }
    }
}