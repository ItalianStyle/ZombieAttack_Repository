using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float health = 100;
    [SerializeField] float collisionDamage = 10f;

    public void DealDamage(float damage)
    {
        if(health > 0f)
        {
            health -= damage;
            if (health <= 0f) gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.transform.GetComponent<PlayerHealth>().DealDamage(collisionDamage);
        }
    }
}