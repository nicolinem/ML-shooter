using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public int startingHealth = 100;
    public float speed = 1f;

    private Enemy currentTarget;


    public Transform shootingPoint;
    public int maxShotsBeforeCooldown = 5;
    public float minTimeBetweenShots = 7;
    public float maxTimeBetweenShots = 3;
    public float cooldownTime = 1f;
    public int damage = 50;

    private int CurrentHealth;
    private int shotsFired;
    private float shotTimer;
    private float cooldownTimer;
    private bool isCooldown;

    private Vector3 startPosition;

    public float moveSpeed = 2f;
    public float stoppingDistance = 100f;
    private Rigidbody playerRb;




    public EnemyManager enemyManager;


    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        CurrentHealth = startingHealth;
        ResetShotTimer();

    }

    private void FixedUpdate()
    {
        List<Enemy> allEnemies = enemyManager.GetAllEnemies();

        Enemy visibleClosestEnemy = null;
        float minVisibleDistance = float.MaxValue;

        // Check for visible enemies
        foreach (var enemy in allEnemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < minVisibleDistance && IsEnemyVisible(enemy))
            {
                visibleClosestEnemy = enemy;
                minVisibleDistance = distanceToEnemy;
            }
        }

        // Continue with your existing logic for movement
        if (visibleClosestEnemy != null && !isCooldown)
        {
            currentTarget = visibleClosestEnemy;
            AimAtEnemy(visibleClosestEnemy);

            // Only move if not within stopping distance
            float distanceToEnemy = Vector3.Distance(transform.position, visibleClosestEnemy.transform.position);
            if (distanceToEnemy > stoppingDistance)
            {
                Vector3 directionToEnemy = (visibleClosestEnemy.transform.position - transform.position).normalized;
                playerRb.MovePosition(transform.position + directionToEnemy * moveSpeed * Time.fixedDeltaTime);
            }
        }

        // Shooting logic



        if (visibleClosestEnemy != null && !isCooldown)
        {

            shotTimer -= Time.fixedDeltaTime;

            // Debug.Log(shotTimer);
            if (shotTimer <= 0)
            {
                Debug.Log("SHOOTING");
                Shoot(); // Shoot only if there is a visible enemy
                if (shotsFired >= maxShotsBeforeCooldown)
                {
                    isCooldown = true;
                    cooldownTimer = cooldownTime;
                }
                else
                {
                    ResetShotTimer();
                }
            }
        }

        // Cooldown logic
        if (isCooldown)
        {
            cooldownTimer -= Time.fixedDeltaTime;
            if (cooldownTimer <= 0)
            {
                isCooldown = false;
                shotsFired = 0;
                ResetShotTimer();
            }
        }
    }



    private bool IsEnemyVisible(Enemy enemy)
    {
        var layerMask = 1 << LayerMask.NameToLayer("Player");
        Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, directionToEnemy, out RaycastHit hit, 200f, layerMask))
        {
            // Check if the raycast hit the enemy and not a wall
            return hit.collider.gameObject == enemy.gameObject;
        }
        return false;
    }

    private void AimAtEnemy(Enemy enemy)
    {
        Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
        directionToEnemy.y = 0; // This ensures there is no tilt on the x-axis
        transform.forward = directionToEnemy; // Rotate player to face the enemy
    }

    private void ResetShotTimer()
    {
        shotTimer = Random.Range(1, 6);
        Debug.Log(shotTimer);
    }

    private void Shoot()
    {
        if (currentTarget == null)
        {
            return; // No target to shoot at
        }

        shotsFired++;

        var layerMask = 1 << LayerMask.NameToLayer("Player");
        var direction = (currentTarget.transform.position - shootingPoint.position).normalized;

        Debug.DrawRay(shootingPoint.position, direction * 100f, Color.green, 2f);

        if (Physics.Raycast(shootingPoint.position, direction, out var hit, 200f, layerMask))
        {
            Enemy hitEnemy = hit.transform.GetComponent<Enemy>();
            if (hitEnemy != null)
            {
                hitEnemy.GetShot(damage);
            }
        }
        else
        {
            // Log miss or handle it
        }
    }


    public void GetShot(int damage, Enemy shooter)
    {

        ApplyDamage(damage, shooter);
    }

    private void ApplyDamage(int damage, Enemy shooter)
    {
        CurrentHealth -= damage;


        shooter.PlayerHit();

        if (CurrentHealth <= 0)
        {
            Die(shooter);
        }
    }

    private void Die(Enemy shooter)
    {
        Respawn();
        shooter.RegisterKill();

    }



    public void Respawn()
    {
        CurrentHealth = startingHealth;
        var randomPosition = enemyManager.GetRandomPosition();
        transform.position = new Vector3(randomPosition.x, 1f, randomPosition.z);

    }
}