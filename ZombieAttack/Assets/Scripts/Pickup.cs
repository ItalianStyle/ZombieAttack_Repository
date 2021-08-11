using System.Collections;
using UnityEngine;
using System;

namespace ZombieAttack
{
    [RequireComponent(typeof(Collider), typeof(Renderer))]
    public class Pickup : MonoBehaviour
    {
        public event Action OnPickupTake = delegate { };
        public event Action OnPlayerNear = delegate { };
        public event Action OnPlayerFar = delegate { };

        public enum PickupType { NotDefined, Shotgun, Health, Stamina}

        // User Inputs
        public PickupType pickupType = PickupType.NotDefined;

        [Header("PowerUp properties and stats")]
        public float timeToRespawn;
        [SerializeField] [Min(0.1f)] float healAmount;
        [SerializeField] [Min(.1f)] float staminaRecoverAmount;
       
        [Header("References")]
        public Gun gunToActivate;
        [SerializeField] WorldSpaceCanvas pickupTimerCanvas;
        [SerializeField] Renderer meshRenderer = null;

        bool canProcessInput = false;

        private void Awake()
        {
            if (pickupType is PickupType.Shotgun)
                gunToActivate = GameObject.FindGameObjectWithTag("Player").transform.Find("Shotgun").GetComponent<Gun>();
           
            else if(pickupType is PickupType.Health || pickupType is PickupType.Stamina)
                pickupTimerCanvas = GetComponentInChildren<WorldSpaceCanvas>();          
        }

        private void OnEnable()
        {            
            pickupTimerCanvas.OnTimerFinished -= () => SetPickup(true);
            EnemyManager.OnAllWavesKilled += ResetPickup;
            GameManager.GameRestarted -= (_) => ResetPickup();
        }

        private void OnDisable()
        {
            if (pickupType is PickupType.Shotgun)
                GameManager.GameRestarted += (_) => ResetPickup();

            else if (pickupType is PickupType.Health || pickupType is PickupType.Stamina)
                pickupTimerCanvas.OnTimerFinished += () => SetPickup(true);  //Re-enable the pickup when timer is finished
        }              

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (pickupType is PickupType.Shotgun)
                {
                    OnPlayerNear.Invoke();
                    canProcessInput = true;
                }
                else if (pickupType is PickupType.Health && other.TryGetComponent(out Health playerHealth) && playerHealth.CanHeal)
                {
                    playerHealth.Heal(healAmount);
                    SetPickup(false);
                }
                else if (pickupType is PickupType.Stamina && other.TryGetComponent(out StaminaSystem playerStamina) && playerStamina.canRecoverStamina)
                {
                    playerStamina.Recover(staminaRecoverAmount);
                    SetPickup(false);
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

        private void ResetPickup()
        {
            SetPickup(true);
        }

        void SetPickup(bool isActive)
        {
            GetComponent<FloatingMovement>().enabled = isActive;

            if (pickupType is PickupType.Shotgun)
                gameObject.SetActive(isActive);

            else if(pickupType is PickupType.Health || pickupType is PickupType.Stamina)
            {
                if (isActive)
                    GetComponentInChildren<ParticleSystem>().Play();
                else
                    GetComponentInChildren<ParticleSystem>().Stop();
                GetComponent<Collider>().enabled = isActive;

                //Set pickup trasparency according to its state
                Color temp = meshRenderer.material.color;
                temp.a = isActive ? 1f : .4f;
                meshRenderer.material.color = temp;

                enabled = isActive;
            }

            //Show up the world canvas for timer
            if(!isActive)
                OnPickupTake.Invoke();
        }
    }
}