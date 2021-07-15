using System;
using UnityEngine;

namespace ZombieAttack
{
    public class Reward : MonoBehaviour
    {
        [SerializeField] int money;

        void Start()
        {
            gameObject.GetComponent<Health>().OnEnemyDead += GiveMoneyToPlayer;
        }

        private void GiveMoneyToPlayer(Health obj)
        {
            GameManager.instance.playerWallet.UpdateCurrentMoney(money, true);
            UI_Manager.instance.UpdateMoneyText(GameManager.instance.playerWallet.GetCurrentMoney());
        }
    }
}