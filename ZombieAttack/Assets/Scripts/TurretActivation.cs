using System;
using UnityEngine;

namespace ZombieAttack
{
    public class TurretActivation : MonoBehaviour
    {
        public event Action<Turret> OnPlayerInRange = delegate { };
        public event Action OnPlayerOutOfRange = delegate { };

        [SerializeField] Turret _turret;
        bool canProcessInput = false;

        public Turret Turret
        {
            get
            {
                if (_turret is null)
                    _turret = GetComponentInChildren<Turret>();
                return _turret;
            }
        }

        private void Awake()
        {
            _turret = GetComponentInChildren<Turret>();
        }

        private void Start()
        {
            Turret.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnPlayerInRange.Invoke(Turret);
                canProcessInput = true;
            }
        }


        private void Update()
        {
            if (canProcessInput && Input.GetKeyDown(KeyCode.E))
            {
                //Meccanica di vendita
                if (Turret.enabled)
                {
                    Wallet.instance.UpdateCurrentMoney(Turret.sellingCost, true);
                    UI_Manager.instance.UpdateMoneyText();
                    Turret.enabled = false;
                }
                //Meccanica di acquisto
                else if (Wallet.instance.HasEnoughMoneyFor(Turret.buildingCost))
                {
                    Wallet.instance.UpdateCurrentMoney(Turret.buildingCost, false);
                    UI_Manager.instance.UpdateMoneyText();
                    Turret.enabled = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnPlayerOutOfRange.Invoke();
                canProcessInput = false;
            }
        }
    }
}