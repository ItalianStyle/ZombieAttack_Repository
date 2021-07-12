using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieAttack
{
    public class Wallet : MonoBehaviour
    {
        [SerializeField] int currentMoney = 0;

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
