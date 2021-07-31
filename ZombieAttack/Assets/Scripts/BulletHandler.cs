using System.Collections.Generic;
using UnityEngine;

namespace ZombieAttack
{
    public class BulletHandler : MonoBehaviour
    {
        Gun.GunType bulletType;
        float _bulletDamage;
        int totalBullets;
        ParticleSystem bulletsParticleSystem;
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

        public float BulletDamage 
        { 
            private get => _bulletDamage; 
            set 
            {
                switch (bulletType)
                {
                    case Gun.GunType.Rifle:
                        _bulletDamage = value > 0 ? value : 1f;
                        break;

                    case Gun.GunType.Shotgun:
                        _bulletDamage = value > 0 ? value / totalBullets : .1f;
                        break;
                }
            }
        }

        public ParticleSystem Particles { get => bulletsParticleSystem; }

        private void Awake()
        {
            bulletsParticleSystem = GetComponent<ParticleSystem>();
        }

        private void Start()
        {           
            switch (transform.parent.parent.name)
            {
                case "Shotgun":
                    bulletType = Gun.GunType.Shotgun;
                    totalBullets = (int)Particles.emission.GetBurst(0).count.constant;
                    break;

                case "Gun":
                    bulletType = Gun.GunType.Rifle;
                    break;

                default:
                    Debug.LogError("Nome del nonno del particle system non riconosciuto");
                    break;
            }
            BulletDamage = GetComponentInParent<Gun>().damage;
        }

        private void OnParticleCollision(GameObject other)  //how to shoot with particle system https://www.youtube.com/watch?v=lkq8iLOr3sw&t=13s
        {          
            int events = bulletsParticleSystem.GetCollisionEvents(other, collisionEvents);
            for(int i = 0; i < events; i++)
            {
                //TODO: Spark particles
                if (other.layer == LayerMask.NameToLayer("Enemy"))
                {
                    if (other.TryGetComponent(out Health enemyHealth))
                    {
                        if (enemyHealth.IsAlive)
                            enemyHealth.DealDamage(BulletDamage);
                        else
                            break;
                    }
                }  
            }
        }
    }
}