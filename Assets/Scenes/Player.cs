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



        if (visibleClosestEnemy != null && !isCooldown)
        {

            shotTimer -= Time.fixedDeltaTime;


            if (shotTimer <= 0)
            {

                Shoot();
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
            return hit.collider.gameObject == enemy.gameObject;
        }
        return false;
    }

    private void AimAtEnemy(Enemy enemy)
    {
        Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
        directionToEnemy.y = 0;
        transform.forward = directionToEnemy;
    }

    private void ResetShotTimer()
    {
        shotTimer = Random.Range(1, 6);

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