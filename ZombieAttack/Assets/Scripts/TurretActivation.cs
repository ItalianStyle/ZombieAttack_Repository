using UnityEngine;

namespace ZombieAttack
{
    public class TurretActivation : MonoBehaviour
    {
        Turret turret;
        [SerializeField] Transform pivotText = null;
        [SerializeField] CanvasGroup activateTurretPanel = null;
        Camera cam;

        private void Awake()
        {
            turret = GetComponentInChildren<Turret>();
            pivotText = transform.GetChild(0);
            activateTurretPanel = GameObject.Find("UI/ActivateTurretPanel").GetComponent<CanvasGroup>();
            cam = Camera.main;
        }

        private void Start()
        {
            UI_Manager.instance.SetCanvasGroup(activateTurretPanel, false);
            turret.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !turret.enabled)
            {
                //Mostra il testo
                UI_Manager.instance.SetCanvasGroup(activateTurretPanel, true);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if(other.CompareTag("Player") && !turret.enabled)
            {
                //activateTurretPanel.transform.position = cam.WorldToScreenPoint(pivotText.position);
                if (Input.GetKey(KeyCode.E))
                {
                    UI_Manager.instance.SetCanvasGroup(activateTurretPanel, false);
                    turret.enabled = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                UI_Manager.instance.SetCanvasGroup(activateTurretPanel, false);
            }
        }
    }
}