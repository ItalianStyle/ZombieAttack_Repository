using System.Collections;
using System.Collections.Generic;
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

        Health playerHealth;

        private void Awake()
        {
            playerHealth = GetComponent<Health>();
            enabled = false;
        }

        private void OnEnable()
        {
            UI_Manager.instance.SetPoisoningIcon(true);
            StartCoroutine(nameof(DealPoisonDamage));
        }

        void OnDisable()
        {
            StopCoroutine(nameof(DealPoisonDamage));
            UI_Manager.instance.SetPoisoningIcon(false);
        }

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
            enabled = false;
        }
    }
}