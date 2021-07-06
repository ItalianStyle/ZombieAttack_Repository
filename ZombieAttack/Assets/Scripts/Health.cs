using System;
using UnityEngine;

namespace ZombieAttack
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float maxHealth = 100;

        float _currentHealth;
        float CurrentHealth
        {
            get => _currentHealth;

            set
            {              
                //Check for min and max values for health
                if (_currentHealth < 0f)
                    _currentHealth = 0f;
                else if (_currentHealth > maxHealth)
                    _currentHealth = maxHealth;
                else
                    _currentHealth = value;

                //Fire the event everytime health is changed
                OnHealthPctChanged(CurrentHealth, maxHealth);
            }
        }

        public event Action<float, float> OnHealthPctChanged = delegate { }; //delegate is to avoid null checks
        public event Action<Health> OnEnemyDead = delegate { };

        private void OnEnable() => CurrentHealth = maxHealth;

        private void OnDisable() => CurrentHealth = maxHealth;

        void ModifyHealth(float amount)
        {
            if (CurrentHealth > 0f && CurrentHealth <= maxHealth)
            {
                CurrentHealth += amount;
                if (CurrentHealth <= 0f)
                {
                    gameObject.SetActive(false);

                    if (gameObject.CompareTag("Finish"))  //Check if it's FinalObjective
                    {
                        Debug.Log("GAME OVER");  //Game Over
                        GameManager.instance.SetStatusGame(GameManager.GameState.Lost);
                        UI_Manager.instance.SetFinishScreen(GameManager.GameState.Lost);
                    }
                    else if (gameObject.CompareTag("Player"))
                    {
                        Debug.Log("Sei morto");
                    }
                    else if (gameObject.CompareTag("Enemy"))
                    {
                        //Aggiorno il counter dei morti
                        OnEnemyDead.Invoke(this);
                        //Play sound
                        //Earn money
                    }
                }             
            }
        }

        public void DealDamage(float damage) => ModifyHealth(-damage);
    }
}