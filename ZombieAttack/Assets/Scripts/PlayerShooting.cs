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

        Gun CurrentGun => guns[CurrentGunIndex];

        private void Awake() => guns = GetComponentsInChildren<Gun>();

        private void Start()
        {
            InitializeGuns();
            GameManager.GameRestarted += (_) => InitializeGuns();
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
                CurrentGun.Shoot(gunBar);

            else if (canScrollMouse)
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
                        SetShotgunAsCurrentGun();
                };
            }
        }

        private void OnDestroy() => GameManager.GameRestarted -= (_) => InitializeGuns();

        void InitializeGuns()
        {
            canScrollMouse = false;
            SetCurrentGun(0);
            SetupGuns();
        }

        void SetupGuns()
        {
            for(int i = 0; i < guns.Length; i++)
                guns[i].SetGunState(i == CurrentGunIndex);
        }

        private void SetShotgunAsCurrentGun()
        {
            canScrollMouse = true;
            SetCurrentGun(1); // 1 == Shotgun  
        }

        //Changes current gun of player by giving new gun index
        private void SetCurrentGun(int newCurrentGunIndex)
        {
            if (CurrentGunIndex != newCurrentGunIndex)
            {
                CurrentGun.SetGunState(false);
                CurrentGunIndex = newCurrentGunIndex;
                CurrentGun.SetGunState(true);
            }
        }
    }
}
