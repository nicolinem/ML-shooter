using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Rigidbody Rigidbody;
    private Vector3 Direction;

    private bool hasCollided = false;
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
        if (hasCollided) return; // Exit if already collided

        if (collision.gameObject.CompareTag("Player"))
        {
            hasCollided = true; // Set flag to true on first collision

            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player?.GetShot(damage, Enemy);
        }

        SelfDestruct();
    }
}