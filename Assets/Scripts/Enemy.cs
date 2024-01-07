using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using TMPro.Examples;


public class Enemy : Agent
{



    public EnemyProjectile enemyProjectile;
    public float speed = 3f;
    public float rotationSpeed = 3f;

    private static readonly int AttackState = Animator.StringToHash("Base Layer.attack_shift");

    public EnemyManager enemyManager;
    private SimpleMultiAgentGroup agentGroup;

    public Transform ShootingPoint;
    public int minStepsBetweenShots = 300;
    public int damage = 25;

    public int startingHealth = 100;
    private int currentHealth;

    EnvironmentParameters defaultParams;

    public Vector2 platformMin = new Vector2(-10f, -10f); // Minimum X and Z coordinates
    public Vector2 platformMax = new Vector2(10f, 10f);   // Maximum X and Z coordinates

    private bool ShotAvaliable = true;
    private int StepsUntilShotIsAvaliable = 0;

    private Vector3 StartingPosition;
    public Rigidbody RB;

    public Animator Anim;



    public void InitializeEnemy(EnemyManager manager, SimpleMultiAgentGroup group)
    {
        enemyManager = manager;
        agentGroup = group;
        StartingPosition = new Vector3(transform.position.x, 0.0001f, transform.position.z);




        currentHealth = startingHealth;
        defaultParams = Academy.Instance.EnvironmentParameters;

        // Register this enemy instance with the group

        enemyManager.RegisterEnemy(this);

        RB = GetComponent<Rigidbody>();
        Anim = GetComponent<Animator>();
    }







    private void Shoot()
    {
        if (!ShotAvaliable)
        {

            return;
        }




        var layerMask = 1 << LayerMask.NameToLayer("Player");
        var direction = transform.forward;


        var newProjectile = Instantiate(enemyProjectile, ShootingPoint.position, Quaternion.Euler(0f, -90f, 0f));
        newProjectile.SetDirection(direction, this);

        Debug.DrawRay(ShootingPoint.position, direction * 60f, Color.blue, 1f);

        // if (Physics.Raycast(ShootingPoint.position, direction, out var hit, 200f, layerMask))
        // {
        //     PlayerController player = hit.transform.GetComponent<PlayerController>();
        //     player?.GetShot(damage, this);

        // }

        // else
        // {

        //     enemyManager.RewardGroup(-0.1f);
        // }

        ShotAvaliable = false;
        StepsUntilShotIsAvaliable = minStepsBetweenShots;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(ShotAvaliable);
        // sensor.AddObservation(target.transform.position);

        //Add Angle Y
    }


    private void FixedUpdate()
    {


        if (!ShotAvaliable)
        {
            StepsUntilShotIsAvaliable--;

            if (StepsUntilShotIsAvaliable <= 0)
            {

                ShotAvaliable = true;
            }
        }

        enemyManager.RewardGroup(-1f / MaxStep);

    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Handle Shooting
        if (Mathf.RoundToInt(actionBuffers.DiscreteActions[0]) >= 1)
        {
            Shoot();
            Anim.CrossFade(AttackState, 0.1f, 0, 0);

        }
        else
        {

        }

        // Handle Movement
        float moveX = actionBuffers.ContinuousActions[0]; // Horizontal movement
        float moveZ = actionBuffers.ContinuousActions[1]; // Vertical movement
        float rotateY = actionBuffers.ContinuousActions[2]; // Rotation







        RB.velocity = new Vector3(moveX * speed, 0f, moveZ * speed);
        transform.Rotate(Vector3.up, rotateY * rotationSpeed);

    }


    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Wall")
        {

            enemyManager.RewardGroup(-0.4f);
        }
    }



    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        var continuousActionsOut = actionsOut.ContinuousActions;

        discreteActionsOut[0] = Input.GetKey(KeyCode.P) ? 1 : 0;

        // Mapping WASD keys to movement
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        continuousActionsOut[0] = Input.GetKey(KeyCode.A) ? -1f : Input.GetKey(KeyCode.D) ? 1f : 0f; // Horizontal

    }



    public void RegisterKill()
    {

        enemyManager.RewardGroup(1.0f);
        enemyManager.EndEpisode();
    }

    public void Reactivate()
    {

        currentHealth = startingHealth;
        transform.position = enemyManager.GetRandomPosition(); // Assuming you have a method to get a random start position
        gameObject.SetActive(true); // Reactivate the GameObject
        enemyManager.RegisterEnemy(this); // Re-register with the group
    }

    public void GetShot(int damage)
    {
        currentHealth -= damage;
        enemyManager.RewardGroup(-0.3f);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void PlayerHit()
    {
        enemyManager.RewardGroup(0.5f);
    }


    private void Die()
    {
        enemyManager.RewardGroup(-1f);
        gameObject.SetActive(false);
        enemyManager.UnregisterEnemy(this); // Notify the EnemyManager

    }


}