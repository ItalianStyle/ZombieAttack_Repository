using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZombieAttack
{
    public class KillCounter : MonoBehaviour
    {
        [SerializeField] EnemyManager enemyManager = null;

        private void Update()
        {
            GetComponent<Text>().text = enemyManager.killedEnemies.ToString();
        }
    }
}
