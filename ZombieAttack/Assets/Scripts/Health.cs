using UnityEngine;

namespace ZombieAttack
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float health = 100;

        public void DealDamage(float damage)
        {
            if (health > 0f)
            {
                health -= damage;
                if (health <= 0f)
                {
                    gameObject.SetActive(false);
                    if (gameObject.CompareTag("Finish"))
                    {
                        //Game Over
                        Debug.Log("GAME OVER");
                    }
                        
                }
            }
        }
    }
}