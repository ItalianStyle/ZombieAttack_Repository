using UnityEngine;
using System.Collections;
using System;

namespace ZombieAttack
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] CharacterController Controller;
        Vector3 input;
        [SerializeField] float walkSpeed;
        [SerializeField] float sprintSpeed;
      
        [HideInInspector]
        public float movSpeed; //This variable is automatically set to walkSpeed or sprintSpeed

        Camera cam;
        StaminaSystem playerStamina;
        PoisoningEffect playerPoisoningStatus;
        bool _canRun = true;

        bool CanRun
        {
            get => _canRun && !playerPoisoningStatus.IsPoisoned;

            set
            {
                //Se la stamina non è piena ma è finito l'effetto dell'avvelenamento non permettere al giocatore di correre
                if (!_canRun && value is true)
                    _canRun = false;
                else 
                    _canRun = value;                   
            }
        }

        private void Awake()
        {
            Controller = GetComponent<CharacterController>();
            cam = Camera.main;
            playerStamina = GetComponent<StaminaSystem>();
            playerPoisoningStatus = GetComponent<PoisoningEffect>();
        }

        private void OnEnable()
        {
            StaminaSystem.OnStaminaEmpty += () => CanRun = false;
            StaminaSystem.OnStaminaFull += () => CanRun = true;

            PoisoningEffect.OnPoisoningEffectStarted += (duration) => CanRun = false;
            PoisoningEffect.OnPoisoningEffectFinished += () => CanRun = true;
        }

        private void OnDisable()
        {
            StaminaSystem.OnStaminaEmpty -= () => CanRun = false;
            StaminaSystem.OnStaminaFull -= () => CanRun = true;

            PoisoningEffect.OnPoisoningEffectStarted -= (duration) => CanRun = false;
            PoisoningEffect.OnPoisoningEffectFinished -= () => CanRun = true;
        }

        private void Update()
        {
            input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            Vector3 moveDir = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * input;
            
            if (input.magnitude > 0)
            {
                Quaternion target = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, .1f);
            }

            //Sprint mechanic -> https://www.youtube.com/watch?v=JUTFiyBjlnc&ab_channel=SingleSaplingGames
            //Stamina mechanic -> https://www.youtube.com/watch?v=x9zOct1AMxo&ab_channel=StuartSpence
            if (CanRun)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    //Start decrease stamina
                    playerStamina.isPlayerRunning = true;
                    movSpeed = sprintSpeed;
                }
                else if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    //Start increase stamina
                    CanRun = false;
                    playerStamina.isPlayerRunning = false;
                    movSpeed = walkSpeed;
                }
                else
                    movSpeed = walkSpeed;
            }
            else
                movSpeed = walkSpeed;
            Controller.SimpleMove(moveDir.normalized * movSpeed);
        }

        public void FaceCamera()
        {
            StartCoroutine("SmoothFaceCamera");
        }

        IEnumerator SmoothFaceCamera()
        {
            float elapsed = 0f;
            float timeToFaceCamera = 2f;
            while(elapsed < timeToFaceCamera)
            {
                transform.eulerAngles = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, cam.transform.eulerAngles.y, 0), elapsed / timeToFaceCamera).eulerAngles;
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}