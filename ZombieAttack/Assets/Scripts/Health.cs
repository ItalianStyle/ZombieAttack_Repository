using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ZombieAttack
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float maxHealth = 100;
        [Header("Hits effects")]
        [SerializeField] Color damagedColor = Color.yellow;
        [SerializeField] Color healedColor = Color.green;
        [SerializeField] float timeOfDamagedColor = .1f;

        Material currentMaterial;
        Color normalColor;
        float _currentHealth;

        float CurrentHealth
        {
            get => _currentHealth;

            set
            {
                _currentHealth = value;
                //Check for min and max values for health
                if (_currentHealth < 0f)
                    _currentHealth = 0f;
                else if (_currentHealth > maxHealth)
                    _currentHealth = maxHealth;
                
                //Fire the event everytime health is changed
                OnHealthPctChanged(CurrentHealth, maxHealth);
            }
        }
        public bool CanHeal { get => CurrentHealth < maxHealth; }
        public bool IsAlive { get => CurrentHealth > 0; }

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
            float temp = CurrentHealth;
            CurrentHealth += amount;

            if (CurrentHealth == 0f)
            {
                gameObject.SetActive(false);

                if (gameObject.CompareTag("Finish"))  //Check if it's FinalObjective
                {
                    //Game Over by FinalObjective's death
                    GameManager.instance.SetStatusGame(GameManager.GameState.Lost);
                    UI_Manager.instance.SetFinishScreen(GameManager.GameState.Lost);
                }
                else if (gameObject.CompareTag("Player"))
                {
                    //Game Over by Player's death
                    GameManager.instance.SetStatusGame(GameManager.GameState.Lost);
                    UI_Manager.instance.SetFinishScreen(GameManager.GameState.Lost);
                }
                else if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    //Updating kill counter
                    OnEnemyDead.Invoke(this);
                    //Play sound
                    //Earn money
                }
            }
            else if (gameObject.activeInHierarchy)
                StartCoroutine(nameof(ColorDamaged), temp > CurrentHealth ? damagedColor : healedColor);           
        }

        IEnumerator ColorDamaged(Color damageColor)
        {
            currentMaterial.SetColor("_Color", damageColor);
            yield return new WaitForSeconds(timeOfDamagedColor);
            currentMaterial.SetColor("_Color", normalColor);        
        }

        public void DealDamage(float damage) => ModifyHealth(-damage);

        public void Heal(float amount) => ModifyHealth(amount);

        public void TryToDealPoisoningDamage()
        {
            if (TryGetComponent(out PoisoningEffect poisoningEffect) && !poisoningEffect.IsPoisoned)
            {
                if (Random.Range(0f, 1f) <= poisoningEffect.activationProbability)
                    poisoningEffect.IsPoisoned = true;
            }
        }
    }
}