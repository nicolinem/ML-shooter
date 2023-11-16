using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab; // Assign this in the editor
    public int numberOfEnemies = 3; // Set the number of enemies you want to spawn
    private List<Enemy> enemies = new List<Enemy>();
    private SimpleMultiAgentGroup m_AgentGroup;

    private int remainingEnemies;

    public Vector2 platformMin = new Vector2(-10f, -10f); // Minimum X and Z coordinates
    public Vector2 platformMax = new Vector2(10f, 10f);   // Maximum X and Z coordinates

    void Start()
    {
        remainingEnemies = numberOfEnemies;
        m_AgentGroup = new SimpleMultiAgentGroup();
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // Instantiate and initialize enemies here
            var enemyGO = Instantiate(enemyPrefab, GetRandomPosition(), Quaternion.identity);
            var enemy = enemyGO.GetComponent<Enemy>();
            enemy.InitializeEnemy(this, m_AgentGroup);

        }
    }

    public Vector3 GetRandomPosition()
    {
        Vector3 randomPosition = Vector3.zero;
        bool positionFound = false;
        int maxAttempts = 100;
        int attempts = 0;

        while (!positionFound && attempts < maxAttempts)
        {
            attempts++;
            float randomX = UnityEngine.Random.Range(platformMin.x, platformMax.x);
            float randomZ = UnityEngine.Random.Range(platformMin.y, platformMax.y);
            randomPosition = new Vector3(randomX, 1, randomZ); // Assuming y=1 is slightly above the ground level


            Collider[] colliders = Physics.OverlapSphere(randomPosition, 0.5f); // 0.5f is the radius of the check, adjust as needed for your game

            if (colliders.Length == 0)
            {
                positionFound = true;
            }
        }

        if (!positionFound)
        {
            Debug.LogError("Failed to find a free position for enemy spawn after " + maxAttempts + " attempts.");
            return Vector3.zero;
        }

        return randomPosition;
    }



    public void RegisterEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
            m_AgentGroup.RegisterAgent(enemy);
        }
    }

    public List<Enemy> GetAllEnemies()
    {
        List<Enemy> activeEnemies = new List<Enemy>();
        foreach (var enemy in enemies)
        {
            if (enemy.gameObject.activeSelf) // Check if the enemy GameObject is active
            {
                activeEnemies.Add(enemy);
            }
        }
        return activeEnemies;
    }

    public void RewardGroup(float reward)
    {

        m_AgentGroup.AddGroupReward(reward);
    }

    // Call this to end the episode for the entire group
    public void EndEpisode()
    {
        m_AgentGroup.EndGroupEpisode();
        ResetScene();

    }

    // Call this to interrupt the episode for the entire group
    public void InterruptGroupEpisode()
    {
        m_AgentGroup.GroupEpisodeInterrupted();
        ResetScene();
    }

    private void ResetScene()
    {
        remainingEnemies = numberOfEnemies;
        foreach (var enemy in enemies)
        {

            enemy.Reactivate();

        }

        // Reset the scene, positions of enemies, etc.
    }




    public void UnregisterEnemy(Enemy enemy)
    {

        if (enemies.Contains(enemy))
        {
            remainingEnemies--;
            m_AgentGroup.UnregisterAgent(enemy);
        }


        if (remainingEnemies == 0)
        {

            EndEpisode();

        }


    }

    public SimpleMultiAgentGroup GetAgentGroup()
    {
        return m_AgentGroup;
    }

    public Enemy GetClosestEnemy(Vector3 position)
    {
        Enemy closestEnemy = null;
        float minDistance = float.MaxValue;

        foreach (var enemy in enemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                float distance = Vector3.Distance(position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        return closestEnemy;
    }
}