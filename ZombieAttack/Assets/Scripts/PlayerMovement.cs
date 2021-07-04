using UnityEngine;

namespace ZombieAttack
{
    public class PlayerMovement : MonoBehaviour
    {
        [Tooltip("How fast player can move?")]
        [SerializeField] float movementSpeed = 10f;
        [Tooltip("How fast player can rotate around himself?")]
        [SerializeField] float rotationSpeed = 30f;
        CharacterController playerController;

        private void Awake()
        {
            playerController = GetComponent<CharacterController>();
        }

        private void FixedUpdate()
        {
            //Apply movement
            playerController.SimpleMove(Input.GetAxis("Vertical") * movementSpeed * transform.forward);
            //Apply rotation
            transform.Rotate(Input.GetAxis("Horizontal") * rotationSpeed * transform.up);
        }
    }
}