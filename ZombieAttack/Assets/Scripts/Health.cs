using System;
using System.Collections;
using UnityEngine;

namespace ZombieAttack
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float maxHealth = 100;
        [Header("Hits effects")]
        [SerializeField] Color damagedColor = Color.yellow;
        [SerializeField] float timeOfDamagedColor = .1f;

        Material currentMaterial;
        Color normalColor;
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

        private void Awake()
        {
            currentMaterial = GetComponent<Renderer>().material;
            normalColor = currentMaterial.color;
        }

        private void OnEnable()
        {
            CurrentHealth = maxHealth;
            currentMaterial.SetColor("_Color", normalColor);
        }

        private void OnDisable() => CurrentHealth = maxHealth;

        void ModifyHealth(float amount)
        {
            if (CurrentHealth > 0f && CurrentHealth <= maxHealth)
            {
                CurrentHealth += amount;
                StartCoroutine("ColorDamaged", damagedColor);
                
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
                        GameManager.instance.SetStatusGame(GameManager.GameState.Lost);
                        UI_Manager.instance.SetFinishScreen(GameManager.GameState.Lost);
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

        IEnumerator ColorDamaged(Color damageColor)
        {
            currentMaterial.SetColor("_Color", damageColor);
            yield return new WaitForSeconds(timeOfDamagedColor);
            currentMaterial.SetColor("_Color", normalColor);        
        }

        public void DealDamage(float damage) => ModifyHealth(-damage);
    }
}