using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Rigidbody Rigidbody;
    private Vector3 Direction;
    public int damage = 25;

    private Enemy Enemy;

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.velocity = Direction * 10f;
        Invoke(nameof(SelfDestruct), 1f); // Self destruct after 1 second
    }

    public void SetDirection(Vector3 direction, Enemy enemy)
    {

        Direction = direction;
        Enemy = enemy;
    }

    void SelfDestruct()
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the projectile hits the player
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.GetShot(damage, Enemy);
            }

        }
        SelfDestruct();
    }
}