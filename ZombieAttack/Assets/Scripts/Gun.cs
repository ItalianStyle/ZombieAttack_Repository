using System.Collections;
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
        [SerializeField] float damage = 1f;
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
                
                //Posiziono il bullet
                bullet.transform.parent = muzzleTransform;
                bullet.transform.localPosition = Vector3.zero;
                bullet.transform.parent = null;
                bullet.transform.rotation = Quaternion.LookRotation(playerTransform.forward, playerTransform.up);
                
                //Shooting
                bullet.SetActive(true);
                bullet.GetComponent<Bullet>().Throw(bullet.transform.forward * shootForce, ForceMode.Impulse, damage);
                
                //Post-shoot stuff
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