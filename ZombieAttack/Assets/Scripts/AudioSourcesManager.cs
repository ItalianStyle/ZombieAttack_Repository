using System.Collections.Generic;
using UnityEngine;

namespace ZombieAttack
{
    public class AudioSourcesManager : MonoBehaviour
    {
        [SerializeField] List<AudioSource> audioSources = new List<AudioSource>();

        public AudioSource MusicAudioSource => audioSources[0];
        public AudioSource SFXAudioSource => audioSources[1];

        public static AudioSourcesManager instance;

        private void OnEnable()
        {
            //Verifica se esiste un'altra istanza del AudioSourcesManager nella scena
            if (instance != null && instance != this)
                Destroy(this);
            else
                instance = this;
            DontDestroyOnLoad(this);

            //Inizializza la lista degli audio source presenti nell'audio manager
            foreach (AudioSource audioSource in GetComponents<AudioSource>())
            {
                if (!audioSources.Contains(audioSource))
                    audioSources.Add(audioSource);
            }
        }
    }
}