using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody bulletRigidbody;
    float damage;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            collision.transform.GetComponent<EnemyHealth>().DealDamage(damage);
        }
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
        bulletRigidbody.velocity = Vector3.zero;
    }

    public void Throw(Vector3 force, ForceMode forceMode, float damage)
    {
        bulletRigidbody.AddForce(force, forceMode);
        this.damage = damage;
    }
}