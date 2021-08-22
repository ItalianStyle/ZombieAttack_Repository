
using System.Collections;
using UnityEngine;

namespace ZombieAttack
{
    public class DisableMeshRenderer : MonoBehaviour
    {
        Transform playerTransform;
        [SerializeField] Transform obstacle = null;
        [SerializeField] MeshRenderer obstacleRenderer = null;
        public bool isActive = true;

        private void Start()
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            StartCoroutine(nameof(CheckObstacles));
        }

        public IEnumerator CheckObstacles()
        {
            while (isActive)
            {
                Vector3 camToPlayerDirection = playerTransform.position - transform.position;
                if (Physics.Raycast(transform.position, camToPlayerDirection.normalized, out RaycastHit info, camToPlayerDirection.magnitude, LayerMask.GetMask("Building", "FinalObjective", "Player"), QueryTriggerInteraction.Ignore))
                {
                    Transform previousObstacle = obstacle;
                    MeshRenderer previousObstacleRenderer = obstacleRenderer;
                    obstacle = info.transform;
                    if (obstacle != previousObstacle)
                    {
                        obstacleRenderer = obstacle.GetComponent<MeshRenderer>() ? obstacle.GetComponent<MeshRenderer>() : obstacle.GetComponentInChildren<MeshRenderer>();
                        obstacleRenderer.enabled = obstacle.gameObject.layer == LayerMask.NameToLayer("Player") ? true : false;
                        if(previousObstacleRenderer)
                            previousObstacleRenderer.enabled = true;                        
                    }                   
                }
                else if (obstacleRenderer && !enabled)
                    obstacleRenderer.enabled = true;

                yield return new WaitForSeconds(.2f);
            }
        }
    }
}