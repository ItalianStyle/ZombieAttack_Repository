using UnityEngine;
using System.Collections;

namespace ZombieAttack
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] CharacterController Controller;
        Vector3 input;
        [SerializeField] float walkSpeed;
        [SerializeField] float sprintSpeed;
        public float movSpeed;

        [SerializeField] Camera cam;
        private void Awake()
        {
            Controller = GetComponent<CharacterController>();
            cam = Camera.main;
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
            movSpeed = Input.GetKey(KeyCode.LeftShift)? sprintSpeed : walkSpeed;
           
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