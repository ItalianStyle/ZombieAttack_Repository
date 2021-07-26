
using System.Collections;
using UnityEngine;

namespace ZombieAttack
{
    public class Gun : MonoBehaviour
    {
        public enum GunType { Rifle, Shotgun }
        public GunType gunType;

        public MeshRenderer[] gunVisual;
        Transform muzzleTransform;
        BulletHandler bulletParticles;

        Transform playerTransform;
        
        [Header("Stats")]
        [SerializeField] float reloadTime = .2f;
        [SerializeField] float damage = 1f;
        public int cost;
        bool canShoot = true;

        private void Awake()
        {
            gunVisual = GetComponentsInChildren<MeshRenderer>();
            muzzleTransform = transform.Find("Muzzle");
            bulletParticles = muzzleTransform.GetComponentInChildren<BulletHandler>();
            playerTransform = transform.parent;
        }

        private void Start()
        {
            bulletParticles.BulletDamage = damage;
        }

        public void Shoot(SimpleHealthBar gunBar)
        {
            if (canShoot)
            {
                playerTransform.GetComponent<PlayerMovement>().FaceCamera();
                /*switch(gunType)
                {
                    case GunType.Rifle:
                        //Posiziono il bullet
                        GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject("Bullet");
                        bullet.transform.position = muzzleTransform.position;
                        bullet.transform.forward = muzzleTransform.forward;

                        //Shooting
                        bullet.SetActive(true);
                        bullet.GetComponent<Bullet>().Throw(bullet.transform.forward * shootForce, ForceMode.Impulse, damage);
                        
                        StartCoroutine("CountBulletLifetime", bullet);

                        break;

                    case GunType.Shotgun:
                        bulletParticles.Particles.Play();
                        break;
                }*/
                bulletParticles.Particles.Play();
                //Post-shoot stuff
                StartCoroutine("Reload", gunBar);
                canShoot = false;
            } 
        }

        public void SetGunState(bool isActive)
        {
            for (int i = 0; i < gunVisual.Length; i++)
                gunVisual[i].enabled = isActive;
            
            enabled = isActive;
        }

        IEnumerator Reload(SimpleHealthBar gunBar)
        {
            float elapsed = 0f;
            while(elapsed < reloadTime)
            {
                elapsed += Time.deltaTime;
                gunBar.UpdateBar(elapsed, reloadTime);
                yield return null;
            }
            canShoot = true;
        }
    }
}