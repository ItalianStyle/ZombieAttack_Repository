using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieAttack
{
    public class PlayerShooting : MonoBehaviour
    {
        [SerializeField] Gun[] guns;
        [SerializeField] int _currentGun;
        [SerializeField] bool canScrollMouse = false;
        [SerializeField] SimpleHealthBar gunBar = null;

        Pickup pickup;
         int CurrentGunIndex
        {
            get => _currentGun;
            set
            {
                _currentGun = value;                              
                if (_currentGun < 0)
                    _currentGun = guns.Length - 1;
                else if (_currentGun >= guns.Length)
                    _currentGun = 0;
            }
        }

        private void Awake()
        {
            guns = GetComponentsInChildren<Gun>();
        }

        private void Start()
        {
            InitializeGuns();
            GameManager.GameRestarted += (waveIndex) => InitializeGuns();
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
                guns[CurrentGunIndex].Shoot(gunBar);
            
            if (canScrollMouse)
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0) 
                    SetCurrentGun(CurrentGunIndex + 1);
               
                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                    SetCurrentGun(CurrentGunIndex - 1);                
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Pickup pickup) && pickup.pickupType is Pickup.PickupType.Shotgun)
            {
                pickup.OnPickupTake += () =>
                {
                    if (pickup.pickupType is Pickup.PickupType.Shotgun)
                        SetCurrentGun();
                };
            }
        }

        private void OnDestroy()
        {
            GameManager.GameRestarted -= (waveIndex) => InitializeGuns();
        }

        void InitializeGuns()
        {
            canScrollMouse = false;
            SetCurrentGun(0);
            SetupGuns();
        }

        void SetupGuns()
        {
            for(int i = 0; i < guns.Length; i++)
            {
                if (i != CurrentGunIndex)
                    guns[i].SetGunState(false);
                
                else
                    guns[i].SetGunState(true);
            }
        }

        private void SetCurrentGun()
        {
            canScrollMouse = true;
            SetCurrentGun(1); // 1 == Shotgun
            pickup.OnPickupTake -= () =>
            {
                if (pickup.pickupType is Pickup.PickupType.Shotgun)
                    SetCurrentGun();
            };
        }

        //Changes current gun of player by giving new gun index
        private void SetCurrentGun(int newCurrentGunIndex)
        {
            if (CurrentGunIndex != newCurrentGunIndex)
            {
                guns[CurrentGunIndex].SetGunState(false);
                CurrentGunIndex = newCurrentGunIndex;
                guns[CurrentGunIndex].SetGunState(true);
            }
        }
    }
}
