using UnityEngine;

namespace ZombieAttack
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] CharacterController Controller;
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
            transform.rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
        }
    }
}