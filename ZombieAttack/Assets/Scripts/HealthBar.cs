using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieAttack
{
    public class HealthBar : MonoBehaviour
    {
        public SimpleHealthBar enemyHealthBar;

        Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
            GetComponentInParent<Health>().OnHealthPctChanged += HandleHealthChanged;
        }

        private void HandleHealthChanged(float currentHealth, float maxHealth)
        {
            enemyHealthBar.UpdateBar(currentHealth, maxHealth);
        }

        private void LateUpdate()
        {
            transform.LookAt(mainCamera.transform);
            transform.Rotate(0, 180, 0);
        }
    }
}