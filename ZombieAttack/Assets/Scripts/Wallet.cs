using System;
using UnityEngine;


namespace ZombieAttack
{
    public class Wallet
    {
        public static Wallet instance;
        public static event Action OnCurrentMoneyChanged = delegate { };

        int _currentMoney = 0;
        int checkpointCurrentMoney = 0;

        public int CurrentMoney
        {
            get => _currentMoney;

            set
            {
                _currentMoney = value;
                if (_currentMoney < 0)
                    _currentMoney = 0;

                OnCurrentMoneyChanged.Invoke();
            }
        }

        public static void InitializeWalletInstance(int startValue = 0)
        {
            if(instance is null)          
                instance = new Wallet();
            instance.CurrentMoney = startValue;
        }

        Wallet()
        {
            CurrentMoney = checkpointCurrentMoney = 0;

            EnemyManager.OnWaveKilled += SaveCurrentMoney;
            EnemyManager.OnAllWavesKilled += () => InitializeWalletInstance();

            GameManager.GameRestarted += LoadCheckpointMoney;
        }

        private void LoadCheckpointMoney(int waveIndex) => CurrentMoney = checkpointCurrentMoney;

        private void SaveCurrentMoney() => checkpointCurrentMoney = CurrentMoney;

        public int GetCurrentMoney() => CurrentMoney;
             
        public bool HasEnoughMoneyFor(int cost) => CurrentMoney >= cost;

        public void ResetWallet() => CurrentMoney = 0;

        //Aggiorna il conto del giocatore, se true aggiunge i soldi, altrimenti li toglie
        public void UpdateCurrentMoney(int amount, bool isReward)
        {
            if (isReward) 
                CurrentMoney += amount;
            else
                CurrentMoney -= amount;
        }
    }
}