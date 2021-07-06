using UnityEngine;

namespace ZombieAttack
{
    public class PlayerShooting : MonoBehaviour
    {
        Gun currentGun;
        [SerializeField] SimpleHealthBar gunBar = null;

        private void Awake()
        {
            currentGun = transform.Find("Gun").GetComponent<Gun>();    
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                currentGun.Shoot(gunBar);
            }
        }
    }
}
