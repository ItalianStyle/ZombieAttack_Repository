using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieAttack
{
    public class HealthBar : MonoBehaviour
    {
        public SimpleHealthBar healthBar;
        [SerializeField] Health playerHealth = null;
        Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
            if (playerHealth is null)
                GetComponentInParent<Health>().OnHealthPctChanged += HandleHealthChanged;
            else
                playerHealth.OnHealthPctChanged += HandleHealthChanged;
        }

        private void HandleHealthChanged(float currentHealth, float maxHealth)
        {
            healthBar.UpdateBar(currentHealth, maxHealth);
        }

        private void LateUpdate()
        {
            if (playerHealth is null)
            {
                transform.LookAt(mainCamera.transform);
                transform.Rotate(0, 180, 0);
            }
        }
    }
}