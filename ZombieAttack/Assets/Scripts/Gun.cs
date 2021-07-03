using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieAttack
{
    public class Gun : MonoBehaviour
    {
        ObjectPooler bulletMagazine;
        Transform muzzleTransform;
        Transform playerTransform;
        [SerializeField] float reloadTime = .2f;
        [SerializeField] float shootForce = 10f;
        [SerializeField] float bulletLifetime = 1f;
        bool canShoot = true;

        private void Awake()
        {
            bulletMagazine = GetComponent<ObjectPooler>();
            muzzleTransform = transform.Find("Muzzle");
            playerTransform = transform.parent;
        }

        public void Shoot()
        {
            if (canShoot)
            {
                GameObject bullet = bulletMagazine.GetPooledObject("Bullet");
                Transform bulletTransform = bullet.transform;

                bulletTransform.parent = muzzleTransform;
                //Resetto la posizione
                bulletTransform.localPosition = Vector3.zero;

                
                
                bulletTransform.parent = null;
                //Setto la rotazione
                bulletTransform.rotation = Quaternion.LookRotation(playerTransform.forward, playerTransform.up);
                bullet.SetActive(true);
                
                bulletTransform.GetComponent<Rigidbody>().AddForce(bulletTransform.forward * shootForce, ForceMode.Impulse);
                StartCoroutine(Reload());
                StartCoroutine("CountBulletLifetime", bullet);
                canShoot = false;
            }
        }

        IEnumerator Reload()
        {
            yield return new WaitForSeconds(reloadTime);
            canShoot = true;
        }

        IEnumerator CountBulletLifetime(GameObject bullet)
        {
            yield return new WaitForSeconds(bulletLifetime);
            bullet.SetActive(false);
        }
    }
}