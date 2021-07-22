using UnityEngine;

namespace ZombieAttack
{
    public class ShotgunBulletHandler : MonoBehaviour
    {
        float _bulletDamage;
        public float BulletDamage 
        { 
            private get => _bulletDamage; 
            set 
            {
                if (value > 0)
                    _bulletDamage = value / totalBullets;
                else
                    _bulletDamage = 0.1f;

                Debug.Log(BulletDamage);
            }
        }

        public ParticleSystem Particles { get => GetComponent<ParticleSystem>(); }

        int totalBullets;

        private void Start() => totalBullets = (int) Particles.emission.GetBurst(0).count.constant;

        private void OnParticleCollision(GameObject other)
        {
            if (other.CompareTag("Enemy"))
            {
                Debug.Log("Colpito");
                other.GetComponent<Health>().DealDamage(BulletDamage);
            }
        }
    }
}