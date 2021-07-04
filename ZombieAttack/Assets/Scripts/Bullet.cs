using UnityEngine;

namespace ZombieAttack
{
    public class Bullet : MonoBehaviour
    {
        Rigidbody bulletRigidbody;
        float damage;

        private void Awake() => bulletRigidbody = GetComponent<Rigidbody>();

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Nemico colpito");
                gameObject.SetActive(false);
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