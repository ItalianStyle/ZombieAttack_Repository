using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float currentHealth;

    public void DealDamage(float damage)
    {
        if (currentHealth > 0f)
        {
            currentHealth -= damage;
            if (currentHealth <= 0f)
            {
                //Game lost
                gameObject.SetActive(false);
            }
        }
    }
}
