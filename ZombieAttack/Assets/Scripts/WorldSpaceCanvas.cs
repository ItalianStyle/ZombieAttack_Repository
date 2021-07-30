using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ZombieAttack
{
    public class WorldSpaceCanvas : MonoBehaviour
    {
        enum CanvasMode { None, Pickup, Turret }
        public event Action OnTimerFinished = delegate { };

        [SerializeField] CanvasMode worldSpaceCanvasMode = CanvasMode.None;
        Camera mainCamera;
        [SerializeField] Pickup pickup;
        [SerializeField] TurretActivation turretActivation;

        [SerializeField] CanvasGroup thisCanvasGroup;       
        Text text; //Can be countdown text or shop text
        Image image; //Can be timer background image (filled) or keyIcon

        private void Awake()
        {
            mainCamera = Camera.main;
            if (pickup = GetComponentInParent<Pickup>())
                worldSpaceCanvasMode = CanvasMode.Pickup;
            else if (turretActivation = GetComponentInParent<TurretActivation>())
                worldSpaceCanvasMode = CanvasMode.Turret;
            else
                worldSpaceCanvasMode = CanvasMode.None;

            thisCanvasGroup = GetComponent<CanvasGroup>();
            text = GetComponentInChildren<Text>();           
            image = GetComponentInChildren<Image>();                            
        }

        private void OnEnable()
        {
            UI_Manager.SetCanvasGroup(thisCanvasGroup, true);
            switch (worldSpaceCanvasMode)
            {
                case CanvasMode.Pickup:
                    switch (pickup.pickupType)
                    {
                        case Pickup.PickupType.Health:
                            
                            pickup.OnPickupTake -= EnableTimer;
                            break;
                        case Pickup.PickupType.Shotgun:
                            pickup.OnPickupTake += () => enabled = false;
                            pickup.OnPlayerFar += () => enabled = false;
                            pickup.OnPlayerNear -= () =>
                            {
                                SetupPickupText();
                                enabled = true;
                            };

                            break;
                    }
                    break;

                case CanvasMode.Turret:
                    turretActivation.OnPlayerOutOfRange += () => enabled = false;
                    turretActivation.OnPlayerInRange -= SetupShopText;

                    turretActivation.Turret.OnTurretEnabled += SetupShopText;
                    turretActivation.Turret.OnTurretDisabled += SetupShopText;
                    break;
            }      
        }        

        private void Start()
        {   
            enabled = false;
        }

        void Update()
        {
            transform.LookAt(mainCamera.transform);
            transform.Rotate(0, 180, 0);
        }

        private void OnDisable()
        {
            UI_Manager.SetCanvasGroup(thisCanvasGroup, false);
            switch (worldSpaceCanvasMode)
            {
                case CanvasMode.Pickup:
                    switch (pickup.pickupType)
                    {
                        case Pickup.PickupType.Health:
                            pickup.OnPickupTake += EnableTimer;
                            OnTimerFinished.Invoke();
                            break;

                        case Pickup.PickupType.Shotgun:
                            pickup.OnPickupTake -= () => enabled = false;
                            pickup.OnPlayerFar -= () => enabled = false;
                            pickup.OnPlayerNear += () =>
                            { 
                                SetupPickupText(); 
                                enabled = true;
                            };
                            break;
                    }
                    break;

                case CanvasMode.Turret:
                    turretActivation.OnPlayerOutOfRange -= () => enabled = false;
                    turretActivation.OnPlayerInRange += SetupShopText;

                    turretActivation.Turret.OnTurretEnabled -= SetupShopText;
                    turretActivation.Turret.OnTurretDisabled -= SetupShopText;
                    break;
            }
        }

        private void SetupPickupText()
        {
            if (Wallet.instance.HasEnoughMoneyFor(pickup.gunToActivate.cost))
                SetText(canActiveKeyIcon: true, Color.white, "Compra (-" + pickup.gunToActivate.cost.ToString() + "$)");

            //Se il giocatore non ha i soldi
            else
                SetText(canActiveKeyIcon: false, Color.red, "Raccogli " + pickup.gunToActivate.cost.ToString() + "$");
        }

        private void SetupShopText(Turret turret)
        {
            //Se la torretta è già attiva
            if (turret.enabled)
                SetText(true, Color.white, "Disattiva (+" + turret.sellingCost.ToString() + "$)");

            //Se il giocatore non ha attivato la torretta ed ha i soldi
            else if (Wallet.instance.HasEnoughMoneyFor(turret.buildingCost))
                SetText(true, Color.white, "Attiva (-" + turret.buildingCost.ToString() + "$)");

            //Se il giocatore non ha attivato la torretta e non ha i soldi
            else
                SetText(false, Color.red, "Raccogli " + turret.buildingCost.ToString() + "$");

            enabled = true;
        }

        private void SetText(bool canActiveKeyIcon, Color textColor, string textToShow)
        {
            image.enabled = canActiveKeyIcon;
            text.color = textColor;
            text.text = textToShow;
        }
        private void EnableTimer()
        {
            if (pickup.pickupType is Pickup.PickupType.Health)
            {
                enabled = true;
                StartCoroutine(nameof(WaitForRespawning), pickup.timeToRespawn);
            }
        }

        IEnumerator WaitForRespawning(float waitTime)
        {
            float elapsedTime = waitTime;
            while (elapsedTime > 0)
            {
                elapsedTime -= Time.deltaTime;
                text.text = ((int)elapsedTime).ToString(); //Mostra UI in base a quanto tempo manca per respawnare il pickup 
                image.fillAmount = elapsedTime / waitTime;
                yield return new WaitForEndOfFrame();
            }
            enabled = false;
        }
    }
}