using UnityEngine;


namespace ZombieAttack
{
    [CreateAssetMenu(fileName = "Wallet", menuName = "New Wallet")]
    public class Wallet : ScriptableObject
    {
        [SerializeField] int currentMoney = 0;

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