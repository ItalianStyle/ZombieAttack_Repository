using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieAttack
{
    // Makes objects float up & down while gently spinning.
    public class FloatingMovement : MonoBehaviour
    {
        enum Direction { X, Y, Z }
        [Header("Floating stats")]
        [SerializeField] float amplitude = 0.5f;
        [SerializeField] float frequency = 1f;

        [Header("Rotation stats")]
        [SerializeField] Direction rotationDirection = Direction.Y;
        [SerializeField] float rotatingSpeed = 0f;

        // Position Storage Variables
        Vector3 posOffset = new Vector3();
        Vector3 tempPos = new Vector3();
        Vector3 initialPos = new Vector3();

        void OnEnable() => posOffset = transform.position; //Store the starting position & rotation of the object

        private void Start() => initialPos = transform.position; //Store the starting position
        
        void Update() => Float();
   
        private void OnDisable() => transform.position = initialPos; //Reset the current position to starting position to prevent pickup floating around wrong positions
        
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
    }
}