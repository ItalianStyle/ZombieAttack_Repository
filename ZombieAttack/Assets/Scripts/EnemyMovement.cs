using UnityEngine;
using UnityEngine.AI;

namespace ZombieAttack
{
    public class EnemyMovement : MonoBehaviour
    {
        NavMeshAgent enemyAgent;

        [SerializeField] GameObject destination = null;

        private void Awake()
        {
            enemyAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if(!enemyAgent.isStopped)
                enemyAgent.SetDestination(destination.transform.position);
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
    }
}