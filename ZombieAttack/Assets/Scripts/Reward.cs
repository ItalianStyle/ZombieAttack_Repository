using UnityEngine;

namespace ZombieAttack
{
    public class Reward : MonoBehaviour
    {
        [SerializeField] int money = 0;

        void Start()
        {
            gameObject.GetComponent<Health>().OnEnemyDead += (_) =>
            {
                Wallet.instance.UpdateCurrentMoney(money, true); //Give money to player
                MyAudioManager.instance.PlayCashSFX();
            };
        }
    }
}