
using System.Collections;
using UnityEngine;

namespace ZombieAttack
{
    [RequireComponent(typeof(AudioSource))]
    public class Gun : MonoBehaviour
    {
        public enum GunType { Rifle, Shotgun }
        public GunType gunType;

        [SerializeField] MeshRenderer[] gunVisual;
        [SerializeField] AudioClip gunShootSFX;
        [SerializeField] AudioClip gunLoadSFX;
        Transform muzzleTransform;
        BulletHandler bulletParticles;

        PlayerMovement playerMovement;
        AudioSource gunAudioSource = null;

        [Header("Stats")]
        [SerializeField] float reloadTime = .2f;
        public float damage = 1f;
        public int cost;
        bool canShoot = true;
        bool canPlaySound = false;

        private void Awake()
        {
            gunVisual = GetComponentsInChildren<MeshRenderer>();
            muzzleTransform = transform.Find("Muzzle");
            bulletParticles = muzzleTransform.GetComponentInChildren<BulletHandler>();
            playerMovement = transform.parent.GetComponent<PlayerMovement>();
            gunAudioSource = GetComponent<AudioSource>();
        }

        private void Start() => bulletParticles.BulletDamage = damage;

        private void OnEnable()
        {
            MyAudioManager.OnInstanceReady += () => canPlaySound = true;
            if(canPlaySound)
                MyAudioManager.instance.PlayAudioSourceWithClip(gunAudioSource, gunLoadSFX, true);
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
                bulletParticles.SetShootingEffect(false);
        }

        public void Shoot(SimpleHealthBar gunBar)
        {
            if (canShoot)
            {
                playerMovement.FaceCamera();
                MyAudioManager.instance.PlayAudioSourceWithClip(gunAudioSource, gunShootSFX, true);
                bulletParticles.SetShootingEffect(true);
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

                bulletParticles.SetShootingEffect(false);
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