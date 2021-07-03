using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieAttack
{
    public class PlayerShooting : MonoBehaviour
    {
        Gun currentGun;

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                currentGun.Shoot();
            }
        }
    }
}
