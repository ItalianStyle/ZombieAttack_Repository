
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
        ShotgunBulletHandler shotgunBullets;

        Transform playerTransform;
        ObjectPooler bulletMagazine;
        
        [Header("Rifle variables")]
        [SerializeField] float reloadTime = .2f;
        [SerializeField] float shootForce = 10f;
        [SerializeField] float bulletLifetime = 1f;
        [SerializeField] float damage = 1f;

        bool canShoot = true;

        private void Awake()
        {
            gunVisual = GetComponentsInChildren<MeshRenderer>();
            bulletMagazine = GetComponent<ObjectPooler>();
            muzzleTransform = transform.Find("Muzzle");
            shotgunBullets = muzzleTransform.GetComponentInChildren<ShotgunBulletHandler>();
            playerTransform = transform.parent;
        }

        private void Start()
        {
            if (gunType is GunType.Shotgun)
                shotgunBullets.BulletDamage = damage;
        }

        public void Shoot(SimpleHealthBar gunBar)
        {
            if (canShoot)
            {
                playerTransform.GetComponent<PlayerMovement>().FaceCamera();
                switch(gunType)
                {
                    case GunType.Rifle:
                        //Posiziono il bullet
                        GameObject bullet = bulletMagazine.GetPooledObject("Bullet");
                        bullet.transform.position = muzzleTransform.position;
                        bullet.transform.forward = muzzleTransform.forward;

                        //Shooting
                        bullet.SetActive(true);
                        bullet.GetComponent<Bullet>().Throw(bullet.transform.forward * shootForce, ForceMode.Impulse, damage);
                        
                        StartCoroutine("CountBulletLifetime", bullet);

                        break;

                    case GunType.Shotgun:
                        shotgunBullets.Particles.Play();
                        break;
                }

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

        IEnumerator CountBulletLifetime(GameObject bullet)
        {
            yield return new WaitForSeconds(bulletLifetime);
            bullet.SetActive(false);
        }
    }
}