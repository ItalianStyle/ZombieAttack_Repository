using UnityEngine;

namespace ZombieAttack
{
    public class EnemyAttack : MonoBehaviour
    {
        [SerializeField] float collisionDamage = 10f;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.transform.GetComponent<Health>().DealDamage(collisionDamage);
            }
        }
    }
}