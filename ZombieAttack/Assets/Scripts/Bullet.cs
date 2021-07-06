using UnityEngine;

namespace ZombieAttack
{
    public class Bullet : MonoBehaviour
    {
        Rigidbody bulletRigidbody;
        float damage;

        private void Awake() => bulletRigidbody = GetComponent<Rigidbody>();

        private void OnCollisionEnter(Collision other)
        {
            gameObject.SetActive(false);
            if (other.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Nemico colpito");
                
                other.transform.GetComponent<Health>().DealDamage(damage);
            }
        }

        private void OnDisable()
        {
            transform.rotation = Quaternion.identity;
            transform.position = Vector3.zero;
            bulletRigidbody.velocity = Vector3.zero;
        }

        public void Throw(Vector3 force, ForceMode forceMode, float damage)
        {
            bulletRigidbody.AddForce(force, forceMode);
            this.damage = damage;
        }
    }
}