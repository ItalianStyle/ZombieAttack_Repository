using UnityEngine;
using System.Collections;

namespace ZombieAttack
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] CharacterController Controller;
        [SerializeField] [Range(0.1f, 5f)] float timeToFaceCamera = .1f;
        Vector3 input;
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
            Controller.SimpleMove(moveDir.normalized * movSpeed);

            if (input.magnitude > 0)
            {
                Quaternion target = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, .1f);
            }
        }

        public void FaceCamera()
        {
            StartCoroutine("SmoothFaceCamera");
        }

        IEnumerator SmoothFaceCamera()
        {
            float elapsed = 0f;
            while(elapsed < timeToFaceCamera) //timeToFaceCamera = 2f
            {
                transform.eulerAngles = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, cam.transform.eulerAngles.y, 0), elapsed / timeToFaceCamera).eulerAngles;
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}