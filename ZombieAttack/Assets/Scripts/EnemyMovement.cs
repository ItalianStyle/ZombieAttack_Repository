using UnityEngine;
using UnityEngine.AI;

namespace ZombieAttack
{
    public class EnemyMovement : MonoBehaviour
    {
        NavMeshAgent enemyAgent;

        [SerializeField] Transform destination = null;

        private void Awake()
        {
            enemyAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (!enemyAgent.isStopped)
                enemyAgent.SetDestination(destination.position);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.CompareTag("Finish"))
            {
                enemyAgent.isStopped = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Finish"))
            {               
                enemyAgent.isStopped = false;
            }
        }

        public void SetDestination(Transform destinationTransform)
        {
            destination = destinationTransform;
        }
    }
}