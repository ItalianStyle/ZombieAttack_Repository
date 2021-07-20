using UnityEngine;

namespace ZombieAttack
{
    public class TurretActivation : MonoBehaviour
    {
        Turret turret;
        [SerializeField] Transform pivotText = null;
        bool isPlayerNear = false;

        private void Awake()
        {
            turret = GetComponentInChildren<Turret>();
            pivotText = transform.GetChild(0);
        }

        private void Start()
        {
            UI_Manager.instance.SetActivateTextPanel(false);
            turret.enabled = false;
            isPlayerNear = false;
        }

        private void Update()
        {
            if (isPlayerNear)
            {
                UI_Manager.instance.UpdateActivateTextPanelPosition(pivotText.position);

                //Mostra UI in base allo stato della torretta
                UI_Manager.instance.SetActivateText(turret);
                if (Input.GetKeyDown(KeyCode.E))
                { 
                    //Meccanica di vendita
                    if (turret.enabled)
                    {
                        GameManager.instance.playerWallet.UpdateCurrentMoney(turret.sellingCost, true);
                        UI_Manager.instance.UpdateMoneyText(GameManager.instance.playerWallet.GetCurrentMoney());
                        turret.enabled = false;
                    }
                    //Meccanica di acquisto
                    else if (GameManager.instance.playerWallet.GetCurrentMoney() > turret.buildingCost)
                    {
                        GameManager.instance.playerWallet.UpdateCurrentMoney(turret.buildingCost, false);
                        UI_Manager.instance.UpdateMoneyText(GameManager.instance.playerWallet.GetCurrentMoney());
                        turret.enabled = true;
                    }                   
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerNear = true;
                //Mostra il testo
                UI_Manager.instance.SetActivateTextPanel(true);
            }
        }  

        private void OnTriggerExit(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                isPlayerNear = false;
                UI_Manager.instance.SetActivateTextPanel(false);
            }
        }
    }
}