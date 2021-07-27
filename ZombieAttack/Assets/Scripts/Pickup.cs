using UnityEngine;
using System;

namespace ZombieAttack
{
    // Makes objects float up & down while gently spinning.
    [RequireComponent(typeof(Collider))]
    public class Pickup : MonoBehaviour
    {
        public static event Action<Pickup> OnPickupTake;
        enum Direction { X, Y, Z }
        public enum PickupType { NotDefined, Shotgun}

        #region Variables
        // User Inputs
        public PickupType pickupType = PickupType.NotDefined;

        [Header("Floating stats")]
        [SerializeField] float amplitude = 0.5f;
        [SerializeField] float frequency = 1f;

        [Header("Rotation stats")]
        [SerializeField] Direction rotationDirection = Direction.Y;
        [SerializeField] float rotatingSpeed = 0f;

        [Header("PowerUp properties")]
        //[SerializeField] float newReloadSpeed = 999f;

        [Header("References")]
        [SerializeField] Transform pivotText;
        [SerializeField] Gun gunToActivate;

        // Position Storage Variables
        Vector3 posOffset = new Vector3();
        Vector3 tempPos = new Vector3();

        bool isPlayerNear = false;
        bool isPickedUp = false;
        #endregion

        #region Unity Methods
        private void Awake()
        {
            pivotText = transform.GetChild(0);
            switch(pickupType)
            {
                case PickupType.Shotgun:
                    gunToActivate = GameObject.FindGameObjectWithTag("Player").transform.Find("Shotgun").GetComponent<Gun>();
                    break;
            }          
        }

        void Start()
        {
            // Store the starting position & rotation of the object
            posOffset = transform.position;
            UI_Manager.instance.SetActivateTextPanel(false);
            isPlayerNear = false;
        }

        // Update is called once per frame
        void Update()
        {
            Float();

            if (isPlayerNear)
            {
                UI_Manager.instance.UpdateActivateTextPanelPosition(pivotText.position);

                //Mostra UI in base a quanti soldi ha il giocatore
                UI_Manager.instance.SetActivateText(gunToActivate);
                //Meccanica di acquisto
                if (Input.GetKeyDown(KeyCode.E) && GameManager.instance.playerWallet.GetCurrentMoney() >= gunToActivate.cost)
                {                   
                    GameManager.instance.playerWallet.UpdateCurrentMoney(gunToActivate.cost, false);
                    UI_Manager.instance.UpdateMoneyText(GameManager.instance.playerWallet.GetCurrentMoney());

                    UI_Manager.instance.SetActivateTextPanel(false);
                    isPickedUp = true;
                    gameObject.SetActive(false);                   
                }
            }
        }
        private void OnDisable()
        {
            if (isPickedUp) OnPickupTake?.Invoke(this);
            isPickedUp = false;          
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                switch (pickupType)
                {
                    case PickupType.Shotgun:
                        isPlayerNear = true;
                        //Mostra il testo
                        UI_Manager.instance.SetActivateTextPanel(true);
                        break;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerNear = false;
                UI_Manager.instance.SetActivateTextPanel(false);
            }
        }
        #endregion

        #region Custom Methods
        // Float up/down with a Sin()
        private void Float()
        {
            tempPos = posOffset;
            tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

            transform.position = tempPos;

            switch (rotationDirection)
            {
                case Direction.X:
                    transform.Rotate(Vector3.right * rotatingSpeed * Time.deltaTime);
                    break;

                case Direction.Y:
                    transform.Rotate(Vector3.up * rotatingSpeed * Time.deltaTime);
                    break;

                case Direction.Z:
                    transform.Rotate(Vector3.forward * rotatingSpeed * Time.deltaTime);
                    break;
            }
        }
        #endregion
    }
}