using System.Collections.Generic;
using UnityEngine;
using static ZombieAttack.Gun;

namespace ZombieAttack
{
    public class BulletHandler : MonoBehaviour
    {
        public enum Bullet_Type { NotDefined, Rifle, Shotgun, Turret }
        
        //Gun.GunType bulletType;
        float _bulletDamage;
        int totalBullets;

        ParticleSystem bulletsPS;
        Gun gun = null;
        Turret turret = null;

        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

        public Bullet_Type BulletType
        {
            get
            {
                if (gun = GetComponentInParent<Gun>())
                {
                    switch (gun.gunType)
                    {
                        case GunType.Shotgun:
                            return Bullet_Type.Shotgun;

                        case GunType.Rifle:
                            return Bullet_Type.Rifle;
                    }
                }
                else if (turret = GetComponentInParent<Turret>())
                    return Bullet_Type.Turret;
                    
                else
                    Debug.LogError("Tipo di proiettile non definito!");

                return Bullet_Type.NotDefined;        
            }
        }

        public float BulletDamage 
        { 
            private get => _bulletDamage; 
            set 
            {
                switch (BulletType)
                {
                    case Bullet_Type.Rifle:
                    case Bullet_Type.Turret:
                        _bulletDamage = value > 0 ? value : 1f;
                        break;

                    case Bullet_Type.Shotgun:
                        _bulletDamage = value > 0 ? value / totalBullets : .1f;
                        break;
                }
            }
        }

        public ParticleSystem Particles { get => bulletsPS; }

        private void Awake()
        {
            bulletsPS = GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            SetupBullet();
        }

        private void OnParticleCollision(GameObject other)  //how to shoot with particle system https://www.youtube.com/watch?v=lkq8iLOr3sw&t=13s
        {
            int events = bulletsPS.GetCollisionEvents(other, collisionEvents);
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

        private void SetupBullet()
        {
            if (BulletType is Bullet_Type.Rifle || BulletType is Bullet_Type.Shotgun)
            {
                BulletDamage = gun.damage;
                if (BulletType is Bullet_Type.Shotgun)
                    totalBullets = (int)Particles.emission.GetBurst(0).count.constant;
            }
            else if (BulletType is Bullet_Type.Turret)
                BulletDamage = turret.damage;
        }
    }
}