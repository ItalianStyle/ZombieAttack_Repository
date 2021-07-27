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
            canScrollMouse = false;
            SetCurrentGun(0);
            SetupGuns();
            Pickup.OnPickupTake += SetCurrentGun;
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

        private void SetCurrentGun(Pickup pickup)
        {
            if (pickup.pickupType is Pickup.PickupType.Shotgun)
            {
                canScrollMouse = true;
                SetCurrentGun(1); // 1 == Shotgun
            }
        }

        //Changes current gun of player by giving new gun index
        private void SetCurrentGun(int newCurrentGunIndex)
        {
            //Debug.Log("CurrentGunIndex: " + CurrentGunIndex + "\nnewCurrentGunIndex: " + newCurrentGunIndex);
            if (CurrentGunIndex != newCurrentGunIndex)
            {
                guns[CurrentGunIndex].SetGunState(false);
                CurrentGunIndex = newCurrentGunIndex;
                guns[CurrentGunIndex].SetGunState(true);
            }
        }
    }
}
