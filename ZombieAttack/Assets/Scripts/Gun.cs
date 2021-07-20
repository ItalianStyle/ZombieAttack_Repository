
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

        public void Shoot(SimpleHealthBar gunBar)
        {
            if (canShoot)
            {
                playerTransform.GetComponent<PlayerMovement>().FaceCamera();
                                
                //Posiziono il bullet
                GameObject bullet = bulletMagazine.GetPooledObject("Bullet");
                bullet.transform.position = muzzleTransform.position;
                bullet.transform.forward = muzzleTransform.forward;
                
                //Shooting
                bullet.SetActive(true);
                bullet.GetComponent<Bullet>().Throw(bullet.transform.forward * shootForce, ForceMode.Impulse, damage);

                //Post-shoot stuff
                StartCoroutine("Reload", gunBar);
                StartCoroutine("CountBulletLifetime", bullet);
                canShoot = false;
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