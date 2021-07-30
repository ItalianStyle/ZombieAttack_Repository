using System.Collections;
using UnityEngine;
using System;

namespace ZombieAttack
{
    [RequireComponent(typeof(Collider))]
    public class Pickup : MonoBehaviour
    {
        public event Action OnPickupTake = delegate { };
        public event Action OnPlayerNear = delegate { };
        public event Action OnPlayerFar = delegate { };

        public enum PickupType { NotDefined, Shotgun, Health}

        // User Inputs
        public PickupType pickupType = PickupType.NotDefined;

        [Header("PowerUp properties and stats")]
        public float timeToRespawn;
        [SerializeField] [Min(0.1f)] float healAmount;
       
        [Header("References")]
        public Gun gunToActivate;
        [SerializeField] WorldSpaceCanvas pickupTimerCanvas;

        bool canProcessInput = false;

        private void Awake()
        {
            switch(pickupType)
            {
                case PickupType.Shotgun:
                    gunToActivate = GameObject.FindGameObjectWithTag("Player").transform.Find("Shotgun").GetComponent<Gun>();
                    break;

                case PickupType.Health:
                    pickupTimerCanvas = GetComponentInChildren<WorldSpaceCanvas>();
                    break;
            }          
        }
        private void OnEnable()
        {            
            pickupTimerCanvas.OnTimerFinished -= () => SetPickup(true);
        }

        private void OnDisable()
        {
            OnPickupTake.Invoke();
            if(pickupType is PickupType.Health)
                pickupTimerCanvas.OnTimerFinished += () => SetPickup(true);  //Re-enable the pickup when timer is finished
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                switch (pickupType)
                {
                    case PickupType.Shotgun:
                        OnPlayerNear.Invoke();
                        canProcessInput = true;
                        break;

                    case PickupType.Health:
                        if (other.TryGetComponent(out Health playerHealth) && playerHealth.CanHeal)
                        {
                            playerHealth.Heal(healAmount);
                            SetPickup(false);
                        }
                        break;
                }               
            }
        }

        private void Update()
        {
            if (pickupType is PickupType.Shotgun)
            {
                //Meccanica di acquisto
                if (canProcessInput && Input.GetKeyDown(KeyCode.E) && Wallet.instance.HasEnoughMoneyFor(gunToActivate.cost))
                {
                    Wallet.instance.UpdateCurrentMoney(gunToActivate.cost, false);
                    UI_Manager.instance.UpdateMoneyText();

                    SetPickup(false);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnPlayerFar.Invoke();
                canProcessInput = false;
            }
        }

        void SetPickup(bool canActive)
        {
            GetComponent<FloatingMovement>().enabled = canActive;
            switch(pickupType)
            {
                case PickupType.Health:
                    if (canActive)
                        GetComponentInChildren<ParticleSystem>().Play();
                    else
                        GetComponentInChildren<ParticleSystem>().Stop();
                    GetComponent<Collider>().enabled = canActive;

                    Color temp = GetComponent<MeshRenderer>().material.color;
                    temp.a = canActive ? 1f : .4f;
                    GetComponent<MeshRenderer>().material.color = temp;

                    enabled = canActive;
                    break;

                case PickupType.Shotgun:
                    gameObject.SetActive(canActive);
                    break;
            }
        }
    }
}