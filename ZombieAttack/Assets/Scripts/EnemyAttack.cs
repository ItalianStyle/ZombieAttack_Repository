using System.Collections;
using UnityEngine;

namespace ZombieAttack
{
    public class EnemyAttack : MonoBehaviour
    {
        [Tooltip("How much damage the enemy deals to the player.")]
        [SerializeField] float collisionDamage = 10f;

        [Tooltip("How much damage the enemy deals to the final objective.")]
        [SerializeField] float meleeDamage = 10f;

        [Tooltip("How much time should the enemy wait before dealing damage again to Player.")]
        [SerializeField] float timeToDealCollisionDmg = .1f;

        [Tooltip("How much time should the enemy wait before dealing damage again to FinalObjective.")]
        [SerializeField] float timeToDealMeleeDmg = .1f;
        
        bool canDamagePlayer = true;
        bool canDamageObjective = true;

        private void OnCollisionEnter(Collision collision)
        {
            if (canDamagePlayer)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    collision.transform.GetComponent<Health>().DealDamage(collisionDamage);
                    StartCoroutine(nameof(WaitForDealDamage), true);
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (canDamageObjective)
            {               
                if (other.gameObject.CompareTag("Finish"))
                {
                    other.transform.GetComponent<Health>().DealDamage(meleeDamage);
                    StartCoroutine(nameof(WaitForDealDamage), false);
                }
            }
        }

        IEnumerator WaitForDealDamage(bool isDamagingPlayer)
        {
            if (isDamagingPlayer)
            {
                canDamagePlayer = false;
                yield return new WaitForSeconds(timeToDealCollisionDmg);
                canDamagePlayer = true;
            }
            else
            {
                canDamageObjective = false;
                yield return new WaitForSeconds(timeToDealMeleeDmg);
                canDamageObjective = true;
            }
        }
    }
}