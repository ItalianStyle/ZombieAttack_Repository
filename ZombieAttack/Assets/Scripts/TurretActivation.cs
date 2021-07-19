using UnityEngine;

namespace ZombieAttack
{
    public class TurretActivation : MonoBehaviour
    {
        Turret turret;
        [SerializeField] Transform pivotText = null;

        private void Awake()
        {
            turret = GetComponentInChildren<Turret>();
            pivotText = transform.GetChild(0);
        }

        private void Start()
        {
            UI_Manager.instance.SetActivateTextPanel(false);
            turret.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !turret.enabled)
            {
                //Check money
                UI_Manager.instance.SetActivateText(turret.buildingCost);

                //Mostra il testo
                UI_Manager.instance.SetActivateTextPanel(true);
            }
        }  

        private void OnTriggerStay(Collider other)
        {
            if(other.CompareTag("Player") && !turret.enabled)
            {
                UI_Manager.instance.UpdateActivateTextPanelPosition(pivotText.position);
                if (Input.GetKey(KeyCode.E) && GameManager.instance.playerWallet.GetCurrentMoney() > turret.buildingCost)
                {
                    UI_Manager.instance.SetActivateTextPanel(false);
                    GameManager.instance.playerWallet.UpdateCurrentMoney(turret.buildingCost, false);
                    UI_Manager.instance.UpdateMoneyText(GameManager.instance.playerWallet.GetCurrentMoney());
                    turret.enabled = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                UI_Manager.instance.SetActivateTextPanel(false);
            }
        }
    }
}