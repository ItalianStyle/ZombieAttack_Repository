using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Larochiens_Adventure
{
    // Makes objects float up & down while gently spinning.
    [RequireComponent(typeof(Collider))]
    public class Pickup : MonoBehaviour
    {
        enum Direction { X, Y, Z }
        public enum PickupType { NotDefined, Sword, Shield, FinalSword }

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
        [SerializeField] float newReloadSpeed = 999f;
        
        [Header("References")]
        [SerializeField] Light lightEffect = null;
        [SerializeField] EnemySpawnManager enemySpawnManager = null;

        // Position Storage Variables
        Vector3 posOffset = new Vector3();
        Vector3 tempPos = new Vector3();

        #endregion

        #region Unity Methods
        // Use this for initialization
        void Start()
        {
            // Store the starting position & rotation of the object
            posOffset = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            // Float up/down with a Sin()
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                switch (pickupType)
                {
                    case PickupType.Sword:
                        UI_Manager.instance.SetReloadIcon(true, PickupType.Sword);
                        UI_Manager.instance.SetHPBar(GameManager.CharacterType.Player, true);
                        GameManager.instance.ManageBossDoor(true);
                        enemySpawnManager.SpawnNextWave();
                        break;

                    case PickupType.Shield:
                        UI_Manager.instance.SetReloadIcon(true, PickupType.Shield);
                        break;

                    case PickupType.FinalSword:
                        enemySpawnManager.SpawnNextWave();
                        
                        //Potenzio la ricarica del giocatore
                        other.GetComponent<ObjectThrower>().SetReloadTime = newReloadSpeed;

                        UI_Manager.instance.SetReloadIcon(false, PickupType.Sword);
                        UI_Manager.instance.SetReloadIcon(true, PickupType.FinalSword);
                        break;
                }
                if (lightEffect != null)
                    lightEffect.enabled = false;
                gameObject.SetActive(false);
            }
        }
        #endregion
    }
}