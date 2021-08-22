using UnityEngine;
using UnityEngine.AI;

namespace ZombieAttack
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyMovement : MonoBehaviour
    {
        NavMeshAgent enemyAgent;
        Transform destination = null;
        Animator enemyAnimator = null;

        private void Awake()
        {
            enemyAgent = GetComponent<NavMeshAgent>();
            enemyAnimator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (!enemyAgent.isStopped)
                enemyAgent.SetDestination(destination.position);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Finish") && !gameObject.CompareTag("EnemyMedium"))
            {
                enemyAgent.isStopped = true;
                enemyAnimator.SetBool("CanAttackObjective", true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Finish"))
            {
                enemyAgent.isStopped = false;
                enemyAnimator.SetBool("CanAttackObjective", false);
            }
        }       

        public void SetTargetDestination(Transform targetDestinationTransform) => destination = targetDestinationTransform;
    }
}