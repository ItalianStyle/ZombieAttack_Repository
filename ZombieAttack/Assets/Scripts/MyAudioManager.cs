using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZombieAttack
{
    public class MyAudioManager : MonoBehaviour
    {
        /*
         * public enum VolumeType { Master, Background, SFX }

        public float backgroundVolume = 1f;
        public float sfxVolume = 1f;

        [Header("Background clips")]
        [Tooltip("Musica di sottofondo del menu")]
        [SerializeField] AudioClip menuBackgroundClip = null;
        [Tooltip("Musica di sottofondo del gioco")]
        [SerializeField] AudioClip gameBackgroundClip = null;
        [Tooltip("Musica di sottofondo quando c'è il boss")]
        [SerializeField] AudioClip bossFightClip = null;

        public static MyAudioManager instance = null;

        AudioSource audioSource = null;

        private void Awake()
        {
            //Prendi il componente audioSource per la musica di background
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            //Verifica se esiste un'altra istanza del MyAudioManager nella scena
            if (instance != null && instance != this)
                Destroy(gameObject);
            else
                instance = this;

            DontDestroyOnLoad(this);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Start()
        {
            SetVolume(.5f, VolumeType.Master);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            //Fai loopare la musica di background
            audioSource.loop = true;
            PlayBackgroundClip(scene.buildIndex); 
        }

        public void PlayBackgroundClip(int index)
        {
            if (audioSource)
            {
                //Setta la traccia in base alla scena
                switch (index)
                {
                    case 0:
                        audioSource.clip = menuBackgroundClip;
                        break;
                    case 1:
                        audioSource.clip = gameBackgroundClip;
                        break;
                    case 2:
                        audioSource.clip = bossFightClip;
                        break;
                }
                
                if (audioSource.isPlaying)
                    audioSource.Stop();
                //Eseguila
                audioSource.volume = backgroundVolume;
                audioSource.Play();
            }
        }

        //Suona l'effetto sonoro 
        public void PlayAudioSourceWithClip(AudioSource audioSource, AudioClip clip, bool isOneShot)
        {
            if (isOneShot)
                audioSource.PlayOneShot(clip, sfxVolume);
            else
            {
                audioSource.clip = clip;
                audioSource.volume = sfxVolume;
                audioSource.Play();
            }
        }

        //Imposta uno dei volumi: Master, Background, Effetti sonori
        public void SetVolume(float value, VolumeType volumeType)
        {
            switch(volumeType)
            {
                case VolumeType.Background:
                    backgroundVolume = value;
                    audioSource.volume = backgroundVolume;
                    break;

                case VolumeType.Master:
                    AudioListener.volume = value;
                    break;

                case VolumeType.SFX:
                    sfxVolume = value;
                    break;
            }
        }
        */
    }
}