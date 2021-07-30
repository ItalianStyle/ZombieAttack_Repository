using UnityEngine;


namespace ZombieAttack
{
    public class Wallet
    {
        public static Wallet instance;

        int currentMoney = 0;

        public static void InitializeWalletInstance(int startValue = 0)
        {
            if(instance is null)          
                instance = new Wallet();
            instance.currentMoney = startValue;
        }

        public int GetCurrentMoney() => currentMoney;
             
        public bool HasEnoughMoneyFor(int cost) => currentMoney >= cost;

        public void ResetWallet() => currentMoney = 0;

        //Aggiorna il conto del giocatore, se true aggiunge i soldi, altrimenti li toglie
        public void UpdateCurrentMoney(int amount, bool isReward)
        {
            if (isReward) currentMoney += amount;
            else
            {
                currentMoney -= amount;
                if (currentMoney < 0)
                    currentMoney = 0;
            }
        }
    }
}