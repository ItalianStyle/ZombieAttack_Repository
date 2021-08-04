using System;
using System.Collections;
using UnityEngine;

namespace ZombieAttack
{
    public class PoisoningEffect : MonoBehaviour
    {
        [Tooltip("Probability of the effect to be activated")]
        [Range(0f, 1f)] public float activationProbability;

        [Tooltip("How much the effect lasts (in seconds)")]
        [SerializeField] [Min(.1f)] float effectDuration;

        [Tooltip("After how much time damage is dealt (in seconds)")]
        [SerializeField] float intervalOfDamage;

        [SerializeField] float poisonDamage;
        [SerializeField] bool _isPoisoned;

        Health playerHealth;

        public static event Action<float> OnPoisoningEffectStarted = delegate { };
        public static event Action OnPoisoningEffectFinished = delegate { };
        public bool IsPoisoned
        {
            get => _isPoisoned;

            set
            {
                _isPoisoned = value;
                if (_isPoisoned)
                {
                    StartCoroutine(nameof(DealPoisonDamage));
                    OnPoisoningEffectStarted.Invoke(effectDuration);
                }
                else
                {
                    StopCoroutine(nameof(DealPoisonDamage));
                    OnPoisoningEffectFinished.Invoke();
                }
            }
        }

        private void Awake() => playerHealth = GetComponent<Health>();
        
        IEnumerator DealPoisonDamage()
        {
            float durationElapsed = 0f;
            float intervalElapsed = 0f;
            while (durationElapsed <= effectDuration)
            {
                durationElapsed += Time.deltaTime;
                intervalElapsed += Time.deltaTime;
                if (intervalElapsed > intervalOfDamage)
                {
                    intervalElapsed = 0f;
                    playerHealth.DealDamage(poisonDamage);
                }
                yield return null;
            }
            IsPoisoned = false;
        }
    }
}