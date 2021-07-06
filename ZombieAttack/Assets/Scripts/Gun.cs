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
        Camera cam;

        private void Awake()
        {
            bulletMagazine = GetComponent<ObjectPooler>();
            muzzleTransform = transform.Find("Muzzle");
            playerTransform = transform.parent;
            cam = Camera.main;
        }

        public void Shoot(SimpleHealthBar gunBar)
        {
            if (canShoot)
            {
                playerTransform.GetComponent<PlayerMovement>().FaceCamera();
                Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0));
                RaycastHit hit;
                Vector3 targetPoint;
                
                if (Physics.Raycast(ray, out hit))
                    targetPoint = hit.point;
                else targetPoint = ray.GetPoint(75);

                Vector3 targetDir = (targetPoint - muzzleTransform.position);
                
                //Posiziono il bullet
                GameObject bullet = bulletMagazine.GetPooledObject("Bullet");
                bullet.transform.position = muzzleTransform.position;
                bullet.transform.forward = targetDir;
                
                //Shooting
                bullet.SetActive(true);
                bullet.GetComponent<Bullet>().Throw(targetDir.normalized * shootForce, ForceMode.Impulse, damage);

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