using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    NavMeshAgent enemyAgent;

    private void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
            
    }
}