using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody rigidbody;
    private Vector3 direction;
    public int damage = 25;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = direction * 10f;
        Invoke(nameof(SelfDestruct), 1f); // Self destruct after 1 second
    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection;
    }

    void SelfDestruct()
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the projectile hits an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy?.GetShot(damage);
        }
        SelfDestruct();
    }
}