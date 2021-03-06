using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ZombieAttack
{
    public class UI_Manager : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] CanvasGroup finishScreen = null;
        [SerializeField] CanvasGroup playerPanel = null;

        [SerializeField] CanvasGroup finalObjectiveHPBarPanel = null;
        [SerializeField] CanvasGroup mainMenuPanel = null;
        [Space]
        [SerializeField] Text waveText = null;
        [SerializeField] Animator waveTextAnimator = null;
        [SerializeField] Animator poisonIconAnimator = null;

        [SerializeField] Text pickupTimerText;
        [SerializeField] Text timerText = null;
        [SerializeField] Text moneyText = null;

        [Header("End screen stats")]
        [SerializeField] FinishScreen[] finishScreens = null;

        [Header("Buttons")]
        [SerializeField] GameObject playButton = null;        
        [SerializeField] GameObject resumeButton = null;
        
        Text titleFinishScreen = null;

        Camera cam;
       
        public static UI_Manager instance;

        private void OnEnable()
        {
            //Verifica se esiste un'altra istanza dello UI_Manager nella scena
            if (instance != null && instance != this)
                Destroy(gameObject);
            else
                instance = this;

            DontDestroyOnLoad(this);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            PoisoningEffect.OnPoisoningEffectStarted += (duration) => SetPoisoningIcon_animation(true);
            PoisoningEffect.OnPoisoningEffectFinished += () => SetPoisoningIcon_animation(false);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            GetReferences(scene.buildIndex);

            switch (scene.buildIndex)
            {
                case 0:
                    //Setto la UI
                    SetCanvasGroup(mainMenuPanel, true);
                    /*SetCanvasGroup(settingsPanel, false);
                    SetCanvasGroup(creditsPanel, false);

                    //Aggiungo la funzionalità ai bottoni
                    settingsButton.onClick.AddListener(delegate { SetCamera(MenuType.Settings); });
                    settingsButton.onClick.AddListener(delegate { UpdateVolumeSettings(); });
                    creditsButton.onClick.AddListener(delegate { SetCamera(MenuType.Credits); });
                    creditsButton.onClick.AddListener(delegate { SetScrollingCredits(true); });

                    returnButtonFromSettings.onClick.AddListener(delegate { SetCamera(MenuType.MainMenu); });
                    returnButtonFromCredits.onClick.AddListener( delegate { SetCamera(MenuType.MainMenu); });
                    returnButtonFromCredits.onClick.AddListener(delegate { SetScrollingCredits(false); });

                    
                    //Setto gli slider per i volumi nelle opzioni
                    masterVolumeSlider.onValueChanged.AddListener(delegate { MyAudioManager.instance.SetVolume(masterVolumeSlider.value, MyAudioManager.VolumeType.Master); });
                    backgroundVolumeSlider.onValueChanged.AddListener(delegate { MyAudioManager.instance.SetVolume(backgroundVolumeSlider.value, MyAudioManager.VolumeType.Background); });
                    sfxVolumeSlider.onValueChanged.AddListener(delegate { MyAudioManager.instance.SetVolume(sfxVolumeSlider.value, MyAudioManager.VolumeType.SFX); });
                    */

                    //SetCanvasGroup(tutorialPanel, true);
                    break;

                case 1:
                    SetCanvasGroup(finishScreen, false);
                    titleFinishScreen.enabled = false;

                    SetCanvasGroup(finalObjectiveHPBarPanel, true);
                    SetCanvasGroup(playerPanel, true);
                    timerText.enabled = false;

                    UpdateMoneyText();
                    //SetCanvasGroup(tutorialPanel, true);

                    PoisoningEffect.OnPoisoningEffectStarted += (duration) => SetPoisoningIcon_animation(true);
                    PoisoningEffect.OnPoisoningEffectFinished += () => SetPoisoningIcon_animation(false);

                    break;

                default:
                    Debug.LogError("Indice di scena non riconosciuto");
                    break;
            }
        }

        //Trova i riferimenti alla UI in ogni scena
        private void GetReferences(int sceneIndex)
        {
            playButton = GameObject.FindGameObjectWithTag("PlayButton");
            cam = Camera.main;
            switch (sceneIndex)
            {
                case 0:
                    mainMenuPanel = GameObject.Find("UI/MainMenuPanel").GetComponent<CanvasGroup>();
                    /*settingsPanel = GameObject.FindGameObjectWithTag("Settings_Panel").GetComponent<CanvasGroup>();
                    creditsPanel = GameObject.FindGameObjectWithTag("Credits_Panel").GetComponent<CanvasGroup>();
                    
                    scrollingCredits = creditsPanel.transform.GetChild(1).GetChild(0).GetComponent<Animator>();

                    settingsButton = GameObject.Find("UI_Canvas/MainMenuPanel/SettingsBtn").GetComponent<Button>();
                    creditsButton = GameObject.Find("UI_Canvas/MainMenuPanel/CreditsBtn").GetComponent<Button>();
                    returnButtonFromSettings = GameObject.Find("UI_Canvas/SettingsPanel/ReturnBtn").GetComponent<Button>();
                    returnButtonFromCredits = GameObject.Find("UI_Canvas/CreditsPanel/ReturnBtn").GetComponent<Button>();

                    masterVolumeSlider = GameObject.Find("UI_Canvas/SettingsPanel/masterVolumeSlider").GetComponent<Slider>();
                    backgroundVolumeSlider = GameObject.Find("UI_Canvas/SettingsPanel/backgroundVolumeSlider").GetComponent<Slider>();
                    sfxVolumeSlider = GameObject.Find("UI_Canvas/SettingsPanel/sfxVolumeSlider").GetComponent<Slider>();
                    */

                    //Trova il riferimento ai bottoni

                    break;

                case 1:
                    //Trova equipaggiamento player
                    //playerEquipment = GameObject.FindGameObjectWithTag("Player").GetComponent<Equipment>();

                    //Trova riferimento agli screen
                    finishScreen = GameObject.FindGameObjectWithTag("FinishPanel").GetComponent<CanvasGroup>();
                    titleFinishScreen = finishScreen.transform.GetChild(0).GetComponent<Text>();

                    //Trova HP bar della cassaforte
                    finalObjectiveHPBarPanel = GameObject.FindGameObjectWithTag("FinalObjectivePanel").GetComponent<CanvasGroup>();

                    //Trova HP bar del player
                    playerPanel = GameObject.FindGameObjectWithTag("PlayerPanel").GetComponent<CanvasGroup>();

                    //pickupTimerPanel = playerPanel.transform.Find("PickupTimer").GetComponent<CanvasGroup>();
                    waveText = playerPanel.transform.Find("WaveText").GetComponent<Text>();
                    waveTextAnimator = waveText.GetComponent<Animator>();

                    poisonIconAnimator = playerPanel.transform.Find("PoisoningIcon_Background").GetComponent<Animator>();

                    timerText = playerPanel.transform.Find("TimerText").GetComponent<Text>();
                    moneyText = playerPanel.transform.Find("MoneyText").GetComponent<Text>();

                    //Trova il riferimento al tutorial
                    //tutorialPanel = GameObject.FindGameObjectWithTag("TutorialPanel").GetComponent<CanvasGroup>();

                    //Trova il riferimento ai bottoni
                    resumeButton = GameObject.FindGameObjectWithTag("ResumeButton");
                    break;

                default:
                    Debug.LogError("Indice di scena non riconosciuto");
                    break;
            }
        }

        public void SetFinishScreen(GameManager.GameState gameState)
        {
            if ((int)gameState < 3)
            {
                //Metti in pausa
                Time.timeScale = 0f;

                //Disabilita UI dell'equipaggiamento del giocatore

                //Disabilita la UI delle barre HP
                SetHPBar(GameManager.EntityType.Player, false);
                SetHPBar(GameManager.EntityType.FinalObjective, false);

                //Abilita l'end screen
                //0 -> Win
                //1 -> Lost
                //2 -> Paused

                //Attiva il testo
                titleFinishScreen.enabled = true;


                titleFinishScreen.text = finishScreens[(int)gameState].titleText;
                titleFinishScreen.color = finishScreens[(int)gameState].titleColor;


                //Attiva i bottoni dell'endscreen
                playButton.gameObject.SetActive(gameState != GameManager.GameState.Paused);
                resumeButton.gameObject.SetActive(gameState is GameManager.GameState.Paused);

                SetCanvasGroup(finishScreen, true);
            }
            else if (gameState is GameManager.GameState.WaveWon)
            {
                //Stampa Ondata passata
                PlayWaveText(isVictoryText: true);
            }
            else
                Debug.LogError("Lo stato di gioco non può essere usato come indice per il titolo di endScreen (" + gameState + ")");
        }

        public static void SetCanvasGroup(CanvasGroup canvasGroup, bool canShow)
        {
            canvasGroup.alpha = canShow ? 1f : 0f;
            canvasGroup.interactable = canShow;
            canvasGroup.blocksRaycasts = canShow;
        }
        
        public void SetHPBar(GameManager.EntityType characterType, bool canActive)
        {
            //Controlla quale barra degli HP deve essere attivata
            switch (characterType)
            {
                case GameManager.EntityType.Player:
                    SetCanvasGroup(playerPanel, canActive);
                    break;

                //Se è il finalObjective attiva la barra degli HP, altrimenti non fare niente
                case GameManager.EntityType.FinalObjective:
                    SetCanvasGroup(finalObjectiveHPBarPanel, canActive);
                    break;

                default:
                    Debug.LogError("Barra HP non riconosciuta");
                    break;
            }
        }

        public void SetHUD(bool canShow)
        {
            /*
            //Se il player ha una spada
            if (playerEquipment.currentWeaponType != Equipment.WeaponType.None)
                SetHPBar(GameManager.EntityType.Player, canShow);

            //Se il boss è presente, riattiva la sua barra HP
            if (GameManager.instance.isBossActive)
                SetHPBar(GameManager.EntityType.FinalObjective, true);

            //Verifica che equipaggiamento ha il giocatore
            if (playerEquipment.currentWeaponType is Equipment.WeaponType.Sword)
                SetReloadIcon(canShow, Pickup.PickupType.Sword);

            else if (playerEquipment.currentWeaponType is Equipment.WeaponType.FinalSword)
                SetReloadIcon(canShow, Pickup.PickupType.FinalSword);

            if (playerEquipment.HasShield)
                SetReloadIcon(canShow, Pickup.PickupType.Shield);
            */
            SetCanvasGroup(finishScreen, !canShow);
            SetHPBar(GameManager.EntityType.Player, canShow);
            SetHPBar(GameManager.EntityType.FinalObjective, canShow);
        }

        //Se non si passa alcun parametro, si setta il testo per la vittoria
        public void PlayWaveText(bool isVictoryText)
        {
            waveText.text = isVictoryText ? "Ondata passata!" : "Ondata " + (EnemyManager.instance.currentWave + 1).ToString();
            waveTextAnimator.SetTrigger(isVictoryText ? "PlayWonWaveText" : "PlayWaveText");
        }

        public void UpdateTimeText(int time) => timerText.text = time.ToString();
        public void UpdateMoneyText()
        {
            if (Wallet.instance is null)
                Wallet.InitializeWalletInstance();                   
            moneyText.text = Wallet.instance.GetCurrentMoney().ToString() + " $";
        }

        public void SetTimerText(bool startOrStopTimerText) => timerText.GetComponent<Animator>().SetBool("CanPlayTimerText", startOrStopTimerText);

        private void SetPoisoningIcon_animation(bool canActive)
        {
            poisonIconAnimator.SetBool("CanActivePoisoningIconBackground", canActive); //Background
            poisonIconAnimator.transform.GetChild(0).GetComponent<Animator>().SetBool("CanActiveTimeBar", canActive); //Timebar
            poisonIconAnimator.transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("PoisonIsActive", canActive); //PoisonIcon
        }
        /*
        public enum MenuType { MainMenu, Settings, Credits }
        Animator scrollingCredits = null;
        */
        /*
        [Header("Settings Sliders")]
        [SerializeField] Slider masterVolumeSlider = null;
        [SerializeField] Slider backgroundVolumeSlider = null;
        [SerializeField] Slider sfxVolumeSlider = null;
        */
        /*[SerializeField] Button settingsButton = null;
        [SerializeField] Button creditsButton = null;
        [SerializeField] Button returnButtonFromSettings = null;
        [SerializeField] Button returnButtonFromCredits = null;
        */
        /*
        [SerializeField] CanvasGroup wavePanel = null;
        [SerializeField] CanvasGroup tutorialPanel = null;


        /*[SerializeField] CanvasGroup settingsPanel = null;
        [SerializeField] CanvasGroup creditsPanel = null;
        //[SerializeField] CanvasGroup[] enemiesHPBars = null; 
        */
        /*
       Equipment playerEquipment = null;
       SimpleHealthBar bossHPBar = null;
       SimpleHealthBar playerHPBar = null;

       //Usato da ObjectThrower per aggiornare la ricarica dell'arma
       public Image GetCurrentWeaponReloadIcon
       {
           get
           {
               if (playerEquipment.currentWeaponType is Equipment.WeaponType.Sword)
                   return swordIcon.transform.GetChild(0).GetComponent<Image>();

               else if (playerEquipment.currentWeaponType is Equipment.WeaponType.FinalSword)
                   return finalSwordIcon.transform.GetChild(0).GetComponent<Image>();

               else return null;
           }
       }
       */
        /*
        private void UpdateVolumeSettings()
        {
           masterVolumeSlider.value = AudioListener.volume;
           backgroundVolumeSlider.value = MyAudioManager.instance.backgroundVolume;
           sfxVolumeSlider.value = MyAudioManager.instance.sfxVolume;

        }

        private void SetScrollingCredits(bool canScroll)
        {
           scrollingCredits.SetBool("CanScrollCredits", canScroll);
        }
        */

        /*
        //Faccio cambiare la posizione della camera in base al menu attivo
        public void SetCamera(MenuType menuType)
        {
            cameraPlacer.SetPosition((int)menuType);
        }
        */

        /*
        public void UpdateEquipmentIcon(EquipmentIcon.IconType equipmentIcon, float currentValue, float maxValue)
        {
            switch (equipmentIcon)
            {
                case EquipmentIcon.IconType.Shield:
                    shieldIcon.equipmentIcon.fillAmount = currentValue / maxValue;
                    break;

                case EquipmentIcon.IconType.Sword:
                    swordIcon.equipmentIcon.fillAmount = currentValue / maxValue;
                    break;

                case EquipmentIcon.IconType.FinalSword:
                    finalSwordIcon.equipmentIcon.fillAmount = currentValue / maxValue;
                    break;

                default:
                    Debug.LogError("Tipo di icona non definito!");
                    break;
            }
        }

        public void UpdateHPBar(GameObject gameObject, float currentValue, float maxValue)
        {
            //Controlla a chi si deve aggiornare la barra degli HP
            if (gameObject.CompareTag("Player"))
                playerHPBar.UpdateBar(currentValue, maxValue);

            else if (gameObject.CompareTag("Enemy"))
            {
                //Se il nemico è il boss aggiorna la barra degli HP, altrimenti non fare niente
                if (gameObject.GetComponent<Enemy>().enemyType is EnemyType.Boss)
                    bossHPBar.UpdateBar(currentValue, maxValue);
            }

            else
                Debug.LogError("Barra HP non riconosciuta");
        }
        */
    }
}