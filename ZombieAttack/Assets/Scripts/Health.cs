using System;
using UnityEngine;

namespace ZombieAttack
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float maxHealth = 100;
        float currentHealth;

        public event Action<float, float> OnHealthPctChanged = delegate { }; //delegate is to avoid null checks

        private void OnEnable() => currentHealth = maxHealth;

        void ModifyHealth(float amount)
        {
            if (currentHealth > 0f && currentHealth <= maxHealth)
            {
                currentHealth += amount;
                if (currentHealth <= 0f)
                {
                    gameObject.SetActive(false);

                    if (gameObject.CompareTag("Finish"))  //Check if it's FinalObjective
                    {
                        Debug.Log("GAME OVER");  //Game Over
                    }
                    else if (gameObject.CompareTag("Player"))
                    {
                        Debug.Log("Sei morto");
                    }
                    else if (gameObject.CompareTag("Enemy"))
                    {
                        //Play sound
                        //Earn money
                    }
                }
                else if (currentHealth > maxHealth)
                    currentHealth = maxHealth;
                OnHealthPctChanged(currentHealth, maxHealth);
            }
        }

        public void DealDamage(float damage) => ModifyHealth(-damage);
    }
}