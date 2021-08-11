using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ZombieAttack
{
    [RequireComponent(typeof(Renderer))]
    public class Health : MonoBehaviour
    {
        [SerializeField] float maxHealth = 100;
        [Header("Hits effects")]
        [SerializeField] Color damagedColor = Color.yellow;
        [SerializeField] Color healedColor = Color.green;
        [SerializeField] float timeOfDamagedColor = .1f;

        SpriteRenderer iconRenderer;
        Material currentMaterial;
        Color normalIconColor;
        Color normalMaterialColor;
        float _currentHealth;

        float CurrentHealth
        {
            get => _currentHealth;

            set
            {
                //Check for min and max values for health
                _currentHealth = Mathf.Clamp(value, 0f, maxHealth);
               
                //Fire the event everytime health is changed
                OnHealthPctChanged(CurrentHealth, maxHealth);
            }
        }

        public bool CanHeal { get => CurrentHealth < maxHealth; }
        public bool IsAlive { get => CurrentHealth > 0f; }

        public event Action<float, float> OnHealthPctChanged = delegate { }; //delegate is to avoid null checks
        public event Action<Health> OnEnemyDead = delegate { };
        public static event Action OnPlayerDead = delegate { };
        public static event Action OnObjectiveDestroyed = delegate { };

        private void Awake()
        {
            iconRenderer = GetComponentInChildren<SpriteRenderer>();
            normalIconColor = iconRenderer.color;
            currentMaterial = GetComponent<Renderer>().material;
            normalMaterialColor = currentMaterial.color;
        }

        private void OnEnable()
        {
            CurrentHealth = maxHealth;
            iconRenderer.color = normalIconColor;
            currentMaterial.SetColor("_Color", normalMaterialColor);
        }

        void ModifyHealth(float amount)
        {
            float temp = CurrentHealth;
            CurrentHealth += amount;

            if (CurrentHealth == 0f)
            {
                gameObject.SetActive(false);
                    
                if (gameObject.CompareTag("Finish"))  //Check if it's FinalObjective
                    OnObjectiveDestroyed.Invoke();  //Game Over by FinalObjective's death

                else if (gameObject.CompareTag("Player"))
                    OnPlayerDead.Invoke();  //Game Over by Player's death

                else if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
                    OnEnemyDead.Invoke(this);   //Updating kill counter
                    //Play sound               
            }
            else if (gameObject.activeInHierarchy)
                StartCoroutine(nameof(ColorDamaged), temp > CurrentHealth ? damagedColor : healedColor);  

            //If current health > 0f and gameObject is not active in the scene
            else
                gameObject.SetActive(true);
        }

        IEnumerator ColorDamaged(Color damageColor)
        {
            iconRenderer.color = damageColor;
            currentMaterial.SetColor("_Color", damageColor);
            yield return new WaitForSeconds(timeOfDamagedColor);
            currentMaterial.SetColor("_Color", normalMaterialColor);
            iconRenderer.color = normalIconColor;
        }

        public void DealDamage(float damage) => ModifyHealth(-damage);

        public void Heal(float amount) => ModifyHealth(amount);

        public void TryToDealPoisoningDamage()
        {
            if (TryGetComponent(out PoisoningEffect poisoningEffect) && !poisoningEffect.IsPoisoned && IsAlive)
            {
                if (Random.Range(0f, 1f) <= poisoningEffect.activationProbability)
                    poisoningEffect.IsPoisoned = true;
            }
        }
    }
}