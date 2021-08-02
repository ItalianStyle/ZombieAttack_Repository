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
        bool canRun = true;

        private void Awake()
        {
            Controller = GetComponent<CharacterController>();
            cam = Camera.main;
            playerStamina = GetComponent<StaminaSystem>();
        }
        private void OnEnable()
        {
            StaminaSystem.OnStaminaEmpty += () => canRun = false;
            StaminaSystem.OnStaminaFull += () => canRun = true;
        }

        private void OnDisable()
        {
            StaminaSystem.OnStaminaEmpty -= () => canRun = false;
            StaminaSystem.OnStaminaFull -= () => canRun = true;
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


            if (canRun)
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
                    canRun = false;
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