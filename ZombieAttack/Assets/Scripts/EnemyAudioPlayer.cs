using System.Collections;
using UnityEngine;

namespace ZombieAttack
{
    public class EnemyAudioPlayer : MonoBehaviour
    {
        [Header("Sound effects")]
        [SerializeField] AudioClip _breathSFX = null;
        [SerializeField] AudioClip[] _attackSoundEffects = null;
        [SerializeField] AudioClip[] _hitSoundEffects = null;
        [SerializeField] AudioClip[] _deadSoundEffects = null;

        Health enemyHealth;
        AudioSource enemyAudioSource;
        bool isAudioPaused = false;

        AudioClip HitSFX => _hitSoundEffects[Random.Range(0, _hitSoundEffects.Length)];
        AudioClip DeadSFX => _deadSoundEffects[Random.Range(0, _deadSoundEffects.Length)];
        public AudioClip BreathSFX => _breathSFX;
        public AudioClip AttackSFX => _attackSoundEffects[Random.Range(0, _attackSoundEffects.Length)];

        private void Awake()
        {
            enemyHealth = GetComponent<Health>();
            enemyAudioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            enemyHealth.OnEnemyDead += (_) => MyAudioManager.instance.PlayClipAt(DeadSFX, transform.position);

            GameManager.GamePaused += () =>
            {
                enemyAudioSource.Pause();
                isAudioPaused = true;
            };
            GameManager.GameResumed += () =>
            {
                if (isAudioPaused)
                {
                    enemyAudioSource.Play();
                    isAudioPaused = false;
                }
            };
            MyAudioManager.instance.PlayAudioSourceWithClip(enemyAudioSource, BreathSFX, false);
        }

        private void OnDisable()
        {
            enemyHealth.OnEnemyDead -= (_) => MyAudioManager.instance.PlayClipAt(DeadSFX, transform.position);

            GameManager.GamePaused -= () =>
            {
                enemyAudioSource.Pause();
                isAudioPaused = true;
            };
            GameManager.GameResumed -= () =>
            {
                if (isAudioPaused)
                {
                    enemyAudioSource.Play();
                    isAudioPaused = false;
                }
            };
        }

        public void PlayHitSFX(Health enemyHealth)
        {
            if (enemyHealth.IsAlive)
                MyAudioManager.instance.PlayAudioSourceWithClip(enemyAudioSource, HitSFX, true);
        }
    }
}