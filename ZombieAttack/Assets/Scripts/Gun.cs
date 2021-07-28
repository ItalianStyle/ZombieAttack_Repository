
using System.Collections;
using UnityEngine;

namespace ZombieAttack
{
    public class Gun : MonoBehaviour
    {
        public enum GunType { Rifle, Shotgun }
        public GunType gunType;

        [SerializeField] MeshRenderer[] gunVisual;
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
                bulletParticles.Particles.Play();
                //Post-shoot stuff
                StartCoroutine(nameof(Reload), gunBar);
                canShoot = false;
            } 
        }

        public void SetGunState(bool isActive)
        {
            if (gunVisual.Length > 0)
            {
                for (int i = 0; i < gunVisual.Length; i++)
                    gunVisual[i].enabled = isActive;
               
                enabled = isActive;
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
    }
}