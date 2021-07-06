using UnityEngine;

namespace ZombieAttack
{
    public class PlayerMovement : MonoBehaviour
    {
        /*[Tooltip("How fast player can move?")]
        [SerializeField] float movementSpeed = 10f;
        [Tooltip("How fast player can rotate around himself?")]
        [SerializeField] float rotationSpeed = 30f;
        CharacterController playerController;
        */
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
            /*//Apply movement
            playerController.SimpleMove(Input.GetAxis("Vertical") * movementSpeed * transform.forward);
            //Apply rotation
            transform.Rotate(Input.GetAxis("Horizontal") * rotationSpeed * transform.up);
            */

            input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            Vector3 moveDir = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * input;
            Controller.Move(moveDir.normalized * movSpeed * Time.deltaTime);

            if (input.magnitude > 0)
            {
                Quaternion target = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, .1f);
            }
        }
    }
}