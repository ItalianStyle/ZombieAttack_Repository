using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZombieAttack
{
    public class TurretManager : MonoBehaviour
    {
        TurretActivation[] turretActivations;
        int checkpointActiveTurrets = 0;
        bool CanSellTurrets
        {
            get;
            set;
        }

        public int ActiveTurrets
        {
            get
            {
                int _activeTurrets = 0;
                foreach(TurretActivation turretActivation in turretActivations)
                {
                    if (turretActivation.Turret.enabled)
                        _activeTurrets++;
                }
                return _activeTurrets;
            }
        }

        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

        private void OnSceneLoaded(Scene scene, LoadSceneMode _)
        {
            if(scene.name is "Gioco")
            {
                turretActivations = FindObjectsOfType<TurretActivation>();

                GameManager.GameRestarted += (waveIndex) => DisableTurrets(CanSellTurrets);
                EnemyManager.OnAllWavesKilled += () => CanSellTurrets = false;
                //Save all current active turrets when starting the wave
                WaveBeginnerFlag.OnStartEnemyWave += (waveIndex) => checkpointActiveTurrets = ActiveTurrets; 
            }
        }

        //Sell all turrets saved in checkpointActiveTurrets
        private void DisableTurrets(bool canSellTurret)
        {
            int turretsSold = 0;
            for (int i = 0; i < turretActivations.Length; i++)
            {
                if (turretActivations[i].Turret.enabled)
                {
                    turretActivations[i].Turret.enabled = false;
                    string log = turretActivations[i].name + " disattivata";

                    if (canSellTurret && turretsSold <= checkpointActiveTurrets)
                    {
                        Wallet.instance.UpdateCurrentMoney(turretActivations[i].Turret.SellingCost, true);
                        turretsSold++;
                        log += ", guadagnato " + turretActivations[i].Turret.SellingCost + "$";
                    }
                    Debug.Log(log);
                }
            }
            checkpointActiveTurrets = 0;
        }
    }
}