using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

namespace ZombieAttack
{
    [RequireComponent(typeof(AudioSource))]
    public class MyAudioManager : MonoBehaviour
    {
        public static event Action OnInstanceReady = delegate { };

        public enum VolumeType { Master, Music, SFX }

        [SerializeField] List<string> _volumeParameters = new List<string>() { "MasterVolume", "MusicVolume", "sfxVolume" };
        [SerializeField] float _volumeMultiplier = 30f;
        [SerializeField] AudioMixer _mixer;

        [Header("Sound effects clips")]
        [Tooltip("Effetto sonoro quando il player è sotto effetto di avvelenamento")]
        [SerializeField] AudioClip poisoningEffectClip;
        [Tooltip("Effetto sonoro per ogni transazione effettuata al conto del player")]
        [SerializeField] AudioClip cashClip;
        [Tooltip("Effetto sonoro di vittoria per ogni ondata passata (tranne l'ultima)")]
        [SerializeField] AudioClip waveWonClip;
        [Tooltip("Effetto sonoro di vittoria per aver passato l'ultima ondata")]
        [SerializeField] AudioClip gameWonClip;
        [Tooltip("Effetto sonoro di sconfitta del giocatore")]
        [SerializeField] AudioClip gameLostClip;

        [Header("Music clips")]
        [Tooltip("0 -> sottofondo del menu\n" +
            "1 -> sottofondo dei livelli\n" +
            "2 -> sottofondo quando c'è il boss")]
        [SerializeField] AudioClip[] musicClips = new AudioClip[3];

        AudioSource playerAudioSource;

        public static MyAudioManager instance = null;

        public float MasterVolume
        {
            get
            {
                _mixer.GetFloat(_volumeParameters[(int)VolumeType.Master], out float volume);
                return volume;
            }
        }
        public float MusicVolume
        {
            get
            {
                _mixer.GetFloat(_volumeParameters[(int)VolumeType.Music], out float volume);
                return volume;
            }
        }
        public float SfxVolume
        {
            get
            {
                _mixer.GetFloat(_volumeParameters[(int)VolumeType.SFX], out float volume);
                return volume;
            }
        }

        public AudioMixer MainAudioMixer => _mixer;

        private void OnEnable()
        {
            //Verifica se esiste un'altra istanza del MyAudioManager nella scena
            if (instance != null && instance != this)
                Destroy(gameObject);
            else
            {
                instance = this;
                OnInstanceReady.Invoke();
            }
            DontDestroyOnLoad(this);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Start()
        {
            SetVolume(PlayerPrefs.GetFloat(VolumeType.Master.ToString(), .5f), VolumeType.Master);
            SetVolume(PlayerPrefs.GetFloat(VolumeType.Music.ToString(), .5f), VolumeType.Music);
            SetVolume(PlayerPrefs.GetFloat(VolumeType.SFX.ToString(), .5f), VolumeType.SFX);

            //Fai loopare la musica di background
            AudioSourcesManager.instance.MusicAudioSource.loop = true;
            PlayMusicClip(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            switch(scene.buildIndex)
            {
                case 0:
                    break;

                case 1:
                    playerAudioSource = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();

                    EnemyManager.OnWaveKilled += () => PlayAudioSourceWithClip(AudioSourcesManager.instance.SFXAudioSource, waveWonClip, false); ;
                    EnemyManager.OnAllWavesKilled += () => PlayFinishAudio(true);

                    GameManager.GameRestarted += (_) => PlayMusicClip(scene.buildIndex);

                    //Metti la musica di sconfitta nei seguenti due casi
                    Health.OnObjectiveDestroyed += () => PlayFinishAudio(false);
                    Health.OnPlayerDead += () => PlayFinishAudio(false);

                    PoisoningEffect.OnPoisoningEffectStarted += (effectDuration) =>
                    {
                        playerAudioSource.loop = true;
                        PlayAudioSourceWithClip(playerAudioSource, poisoningEffectClip, false);
                    };
                    PoisoningEffect.OnPoisoningEffectFinished += () =>
                    {
                        playerAudioSource.loop = false;
                        playerAudioSource.Stop();
                    };
                    break;
            }           
        }

        private void PlayFinishAudio(bool isWonGame) => PlayAudioSourceWithClip(AudioSourcesManager.instance.MusicAudioSource, isWonGame ? gameWonClip : gameLostClip, false);

        //Esegui la traccia in base alla scena (Menu principale, gioco, boss fight)
        public void PlayMusicClip(int index) => PlayAudioSourceWithClip(AudioSourcesManager.instance.MusicAudioSource, musicClips[index], false);

        public void PlayCashSFX() => PlayAudioSourceWithClip(AudioSourcesManager.instance.SFXAudioSource, cashClip, true);

        //Suona l'effetto sonoro con l'audio source fornito
        public void PlayAudioSourceWithClip(AudioSource audioSource, AudioClip clip, bool isOneShot)
        {
            //Debug.Log("SFX: "+clip.name + " volume: "+ sfxVolume);
            if (isOneShot)
                audioSource.PlayOneShot(clip);
            else
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }

        //Source: https://answers.unity.com/questions/316575/adjust-properties-of-audiosource-created-with-play.html
        public AudioSource PlayClipAt(AudioClip clip, Vector3 pos)
        {
            // get the temp object
            GameObject tempGO = ObjectPooler.SharedInstance.GetPooledObject("Audio", "FxTemporaire");
            // set its position
            tempGO.transform.position = pos;
            // get the audio source
            AudioSource aSource = tempGO.GetComponent<AudioSource>(); 
            // set other aSource properties here, if desired
            tempGO.SetActive(true);
            // start the sound
            PlayAudioSourceWithClip(aSource, clip, true);
            // disable object after clip duration
            StartCoroutine(WaitBeforeDisablingAudio(tempGO, clip.length));
            // return the AudioSource reference
            return aSource; 
        }

        //Imposta uno dei volumi: Master, Music, Effetti sonori
        public void SetVolume(float value, VolumeType volumeType) => _mixer.SetFloat(_volumeParameters[(int)volumeType], Mathf.Log10(value) * _volumeMultiplier);

        private IEnumerator WaitBeforeDisablingAudio(GameObject tempGO, float length)
        {
            yield return new WaitForSeconds(length);
            tempGO.SetActive(false);
        }
    }
}