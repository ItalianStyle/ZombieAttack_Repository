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
         int CurrentGun
        {
            get => _currentGun;
            set
            {
                guns[_currentGun].SetGunState(false);

                _currentGun = value;                              
                if (_currentGun < 0)
                    _currentGun = guns.Length - 1;
                else if (_currentGun >= guns.Length)
                    _currentGun = 0;

                guns[_currentGun].SetGunState(true);
            }
        }
        private void Awake()
        {
            guns = GetComponentsInChildren<Gun>();
        }

        private void Start()
        {
            canScrollMouse = false;
            CurrentGun = 0;
            SetupGuns();
            Pickup.OnPickupTake += SetCurrentGun;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
                guns[CurrentGun].Shoot(gunBar);
            
            if (canScrollMouse)
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                    CurrentGun++;
                
                if(Input.GetAxis("Mouse ScrollWheel") < 0)
                    CurrentGun--;
            }
        }

        void SetupGuns()
        {
            for(int i = 0; i < guns.Length; i++)
            {
                if (i != CurrentGun)
                {
                    guns[i].SetGunState(false);
                }
                else
                {
                    guns[i].SetGunState(true);
                }
            }
        }

        private void SetCurrentGun(Pickup pickup)
        {
            if (pickup.pickupType is Pickup.PickupType.Shotgun)
            {
                canScrollMouse = true;
                CurrentGun = 1; // 1 == Shotgun
            }
        }
    }
}
