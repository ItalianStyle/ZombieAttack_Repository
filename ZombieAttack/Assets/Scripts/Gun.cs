
using System.Collections;
using UnityEngine;

namespace ZombieAttack
{
    public class Gun : MonoBehaviour
    {
        public enum GunType { Rifle, Shotgun }
        public GunType gunType;

        Transform muzzleTransform;
        ShotgunBulletHandler shotgunBullets;

        Transform playerTransform;
        ObjectPooler bulletMagazine;
        
        [Header("Rifle variables")]
        [SerializeField] float reloadTime = .2f;
        [SerializeField] float shootForce = 10f;
        [SerializeField] float bulletLifetime = 1f;
        [SerializeField] float damage = 1f;

        [Header("Shotgun variables")]
        //These 2 controls the spread of the cone
        public float scaleLimit = 2.0f;
        public float z = 10f;
        //Shoots multiple rays to check the programming
        public int count = 30;
        bool canShoot = true;

        private void Awake()
        {
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
                        /*for (int i = 0; i < count; ++i)
                            ShootRay();   
                        */
                        shotgunBullets.Particles.Play();
                        break;
                }
                shotgunBullets.TryGetComponent(out ParticleSystem ps);
                ps.Play();

                //Post-shoot stuff
                StartCoroutine("Reload", gunBar);
                canShoot = false;
            }
            
        }

        void ShootRay()
        {
            //  The Ray-hits will be in a circular area
            float randomRadius = Random.Range(0, scaleLimit);
            float randomAngle = Random.Range(0, 2 * Mathf.PI);

            //Calculating the raycast direction
            Vector3 direction = new Vector3(
                randomRadius * Mathf.Cos(randomAngle),
                randomRadius * Mathf.Sin(randomAngle),
                z
            );

            //Make the direction match the transform
            //It is like converting the Vector3.forward to transform.forward
            direction = muzzleTransform.TransformDirection(direction.normalized);

            //Raycast and debug
            Ray r = new Ray(muzzleTransform.position, direction);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit))
            {
                //Debug.DrawLine(muzzleTransform.position, hit.point);
                if (hit.collider.CompareTag("Enemy"))
                {
                    Debug.Log("Nemico colpito");
                    hit.collider.GetComponent<Health>().DealDamage(damage / count);
                }
            }
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