using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;


public class Enemy : Agent
{


    public float speed = 3f;
    public float rotationSpeed = 3f;

    public EnemyManager enemyManager;
    private SimpleMultiAgentGroup agentGroup;

    public Transform ShootingPoint;
    public int minStepsBetweenShots = 50;
    public int damage = 25;

    public int startingHealth = 100;
    private int currentHealth;

    EnvironmentParameters defaultParams;

    public Vector2 platformMin = new Vector2(-10f, -10f); // Minimum X and Z coordinates
    public Vector2 platformMax = new Vector2(10f, 10f);   // Maximum X and Z coordinates

    private bool ShotAvaliable = true;
    private int StepsUntilShotIsAvaliable = 0;

    private Vector3 StartingPosition;
    private Rigidbody RB;



    public void InitializeEnemy(EnemyManager manager, SimpleMultiAgentGroup group)
    {
        enemyManager = manager;
        agentGroup = group;
        StartingPosition = transform.position;
        RB = GetComponent<Rigidbody>();
        currentHealth = startingHealth;
        defaultParams = Academy.Instance.EnvironmentParameters;

        // Register this enemy instance with the group
        agentGroup.RegisterAgent(this);
        enemyManager.RegisterEnemy(this);
    }







    private void Shoot()
    {
        if (!ShotAvaliable)
        {

            return;
        }




        var layerMask = 1 << LayerMask.NameToLayer("Player");
        var direction = transform.forward;

        Debug.DrawRay(ShootingPoint.position, direction * 60f, Color.blue, 1f);

        if (Physics.Raycast(ShootingPoint.position, direction, out var hit, 200f, layerMask))
        {
            Player player = hit.transform.GetComponent<Player>();
            player?.GetShot(damage, this);

        }

        else
        {

            enemyManager.RewardGroup(-0.1f);
        }

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

        AddReward(-1f / MaxStep);

    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Handle Shooting
        if (Mathf.RoundToInt(actionBuffers.DiscreteActions[0]) >= 1)
        {
            Shoot();
        }

        // Handle Movement
        float moveX = actionBuffers.ContinuousActions[0]; // Horizontal movement
        float moveZ = actionBuffers.ContinuousActions[1]; // Vertical movement
        float rotateY = actionBuffers.ContinuousActions[2]; // Rotation

        RB.velocity = new Vector3(moveX * speed, 0f, moveZ * speed);
        transform.Rotate(Vector3.up, rotateY * rotationSpeed);
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2);
        if (hitColliders.Length > 0)
        {
            if (hitColliders[0].tag == "Wall")
            {
                AddReward(-0.04f);
            }

        }
    }






    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        var continuousActionsOut = actionsOut.ContinuousActions;

        discreteActionsOut[0] = Input.GetKey(KeyCode.P) ? 1 : 0;

        // Mapping WASD keys to movement
        continuousActionsOut[0] = Input.GetKey(KeyCode.A) ? -1f : Input.GetKey(KeyCode.D) ? 1f : 0f; // Horizontal
        continuousActionsOut[1] = Input.GetKey(KeyCode.S) ? -1f : Input.GetKey(KeyCode.W) ? 1f : 0f; // Vertical
        continuousActionsOut[2] = Input.GetKey(KeyCode.Q) ? -1f : Input.GetKey(KeyCode.E) ? 1f : 0f; // Rotation
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


    private void Die()
    {
        enemyManager.RewardGroup(-1f);
        gameObject.SetActive(false); // Optionally deactivate the enemy
        enemyManager.UnregisterEnemy(this); // Notify the EnemyManager

    }


}