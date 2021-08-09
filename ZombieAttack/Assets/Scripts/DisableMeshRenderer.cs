
using System.Collections;
using System.Collections.Generic;
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
            playerTransform = GameManager.instance.player.transform;
            StartCoroutine(nameof(CheckObstacles));
        }

        public IEnumerator CheckObstacles()
        {
            while (isActive)
            { 
                if (Physics.Raycast(transform.position, playerTransform.position - transform.position, out RaycastHit info, 100, LayerMask.GetMask("Building", "FinalObjective", "Player"), QueryTriggerInteraction.Ignore))
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