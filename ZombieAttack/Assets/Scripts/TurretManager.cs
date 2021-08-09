using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZombieAttack
{
    public class TurretManager : MonoBehaviour
    {
        Turret[] turrets;
        int checkpointActiveTurrets = 0;
        bool canSellTurrets = true;

        public int ActiveTurrets
        {
            get
            {
                int _activeTurrets = 0;
                foreach(Turret turret in turrets)
                {
                    if (turret.enabled)
                        _activeTurrets++;
                }
                return _activeTurrets;
            }
        }

        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

        private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            if(scene.name is "Gioco")
            {
                turrets = FindObjectsOfType<Turret>();
                GameManager.GameRestarted += (waveIndex) =>
                {
                    if(canSellTurrets)
                        SellTurrets();
                };
                EnemyManager.OnAllWavesKilled += () => ResetTurretList();
                WaveBeginnerFlag.OnStartEnemyWave += (timeToEndWave) => SaveActiveTurrets();
            }
        }

        private void SaveActiveTurrets() => checkpointActiveTurrets = ActiveTurrets;

        void ResetTurretList()
        {
            foreach (Turret turret in turrets)
                turret.enabled = false;
        }

        //Sell all turrets saved in checkpointActiveTurrets
        private void SellTurrets()
        {
            for (int i = 0, turretsSold = 0 ; i < turrets.Length && turretsSold <= checkpointActiveTurrets; i++)
            {
                if (turrets[i].enabled)
                {
                    Wallet.instance.UpdateCurrentMoney(turrets[i].sellingCost, true);
                    UI_Manager.instance.UpdateMoneyText();
                    turrets[i].enabled = false;
                    turretsSold++;
                    Debug.Log("Torretta disattivata, guadagnato " + turrets[i].sellingCost + "$");
                }
            }
            checkpointActiveTurrets = 0;
        }
    }
}
