using System;
using UnityEngine;

namespace ZombieAttack
{
    public class TurretActivation : MonoBehaviour
    {
        public event Action<Turret> OnPlayerInRange = delegate { };
        public event Action OnPlayerOutOfRange = delegate { };

        [SerializeField] Turret _turret;
        [Header("Icon colors")]
        [Tooltip("Color of turret icon when is activated")]
        [SerializeField] Color activatedIconColor = Color.green;
        [Tooltip("Color of turret icon when is deactivated")]
        [SerializeField] Color deactivatedIconColor = Color.red;
        bool canProcessInput = false;
        SpriteRenderer iconRenderer = null;

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
            iconRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        
        private void Start() => CanEnableTurret(false);
        

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
                    Wallet.instance.UpdateCurrentMoney(Turret.SellingCost, true);
                    CanEnableTurret(false);
                }
                //Meccanica di acquisto
                else if (Wallet.instance.HasEnoughMoneyFor(Turret.BuildingCost))
                {
                    Wallet.instance.UpdateCurrentMoney(Turret.BuildingCost, false);
                    CanEnableTurret(true);
                }
            }
        }

        public void CanEnableTurret(bool canEnable)
        {
            Turret.enabled = canEnable;
            iconRenderer.color = Turret.enabled ? activatedIconColor : deactivatedIconColor;
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