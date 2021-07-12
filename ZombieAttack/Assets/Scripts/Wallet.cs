using UnityEngine;


namespace ZombieAttack
{
    [CreateAssetMenu(fileName = "Wallet", menuName = "New Wallet")]
    public class Wallet : ScriptableObject
    {
        [SerializeField] int currentMoney = 0;

        public void ResetWallet()
        {
            currentMoney = 0;
        }

        public void UpdateCurrentMoney(int amount, bool isDetraction)
        {
            if (isDetraction)
            {
                currentMoney -= amount;
                if (currentMoney < 0)
                    currentMoney = 0;
            }
            else
                currentMoney += amount;
        }
    }
}
