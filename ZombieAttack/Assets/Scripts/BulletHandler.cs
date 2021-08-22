using System.Collections.Generic;
using UnityEngine;
using static ZombieAttack.Gun;

namespace ZombieAttack
{
    public class BulletHandler : MonoBehaviour
    {
        public enum Bullet_Type { NotDefined, Rifle, Shotgun, Turret }
        
        float _bulletDamage;
        int totalBullets;
        //This particle system triggers the bullets
        [SerializeField] List<ParticleSystem> muzzleFlashPS;
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

        private void Awake() => bulletsPS = GetComponent<ParticleSystem>();

        private void Start() => SetupBullet();
        
        private void OnParticleCollision(GameObject other)  //how to shoot with particle system https://www.youtube.com/watch?v=lkq8iLOr3sw&t=13s
        {
            int events = bulletsPS.GetCollisionEvents(other, collisionEvents);
            bool canPlayHitEnemySound = true;
            for (int i = 0; i < events; i++)
            {
                //TODO: Spark particles
                if (other.layer == LayerMask.NameToLayer("Enemy"))
                {
                    if (other.TryGetComponent(out Health enemyHealth))
                    {
                        //Health calculations
                        if (enemyHealth.IsAlive)
                        {
                            enemyHealth.DealDamage(BulletDamage);
                            //Play sound
                            if (other.TryGetComponent(out EnemyAudioPlayer enemyAudio) && canPlayHitEnemySound)
                            {
                                enemyAudio.PlayHitSFX(enemyHealth);
                                canPlayHitEnemySound = false;
                            }
                        }
                        else
                            break;
                    }
                }  
            }
        }

        public void SetShootingEffect(bool canActiveEffect)
        {
            if (canActiveEffect)
                muzzleFlashPS[0].Play();
            else
            {
                for (int i = 0; i < muzzleFlashPS.Count; i++)
                    muzzleFlashPS[i].Stop();
            }
        }

        private void SetupBullet()
        {
            if (BulletType is Bullet_Type.Rifle || BulletType is Bullet_Type.Shotgun)
            {
                BulletDamage = gun.damage;
                if (BulletType is Bullet_Type.Shotgun)
                    totalBullets = (int)bulletsPS.emission.GetBurst(0).count.constant;
            }
            else if (BulletType is Bullet_Type.Turret)
                BulletDamage = turret.damage;
        }
    }
}