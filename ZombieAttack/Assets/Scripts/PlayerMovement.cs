using UnityEngine;

namespace ZombieAttack
{
    public class PlayerMovement : MonoBehaviour
    {
        [Tooltip("How fast player can move?")]
        [SerializeField] float movementSpeed = 10f;
        [Tooltip("How fast player can rotate around himself?")]
        [SerializeField] float rotationSpeed = 30f;

        private void FixedUpdate()
        {
            //Apply movement
            transform.Translate(Input.GetAxis("Vertical") * movementSpeed * transform.forward, Space.World);

            //Apply rotation
            transform.Rotate(Input.GetAxis("Horizontal") * rotationSpeed * transform.up);
        }
    }
}