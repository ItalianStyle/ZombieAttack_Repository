using System;
using UnityEngine;

namespace ZombieAttack
{
    public class Reward : MonoBehaviour
    {
        [SerializeField] int money = 0;

        void Start()
        {
            gameObject.GetComponent<Health>().OnEnemyDead += GiveMoneyToPlayer;
        }

        private void GiveMoneyToPlayer(Health obj)
        {
            Wallet.instance.UpdateCurrentMoney(money, true);
            UI_Manager.instance.UpdateMoneyText();
        }
    }
}