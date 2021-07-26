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

        [Min(0.01f)] float rateOfFire;
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
            switch (bulletType)
            {
                case Gun.GunType.Rifle:

                    break;

                case Gun.GunType.Shotgun:
                    totalBullets = (int)Particles.emission.GetBurst(0).count.constant;
                    break;
            }
        }
        private void OnParticleCollision(GameObject other)  //how to shoot with particle system https://www.youtube.com/watch?v=lkq8iLOr3sw&t=13s
        {
            int events = bulletsParticleSystem.GetCollisionEvents(other, collisionEvents);
            for(int i = 0; i < events; i++)
            {
                //Spark particles
            }
            if (other.layer == LayerMask.NameToLayer("Enemy"))
            {
                other.GetComponent<Health>().DealDamage(BulletDamage);
            }
        }
    }
}