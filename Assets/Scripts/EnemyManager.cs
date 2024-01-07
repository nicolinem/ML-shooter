using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;


public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int numberOfEnemies = 3;
    private List<Enemy> enemies = new List<Enemy>();
    private SimpleMultiAgentGroup m_AgentGroup;

    private int remainingEnemies;

    public List<SpawnZone> spawnZones;


    void Start()
    {
        remainingEnemies = numberOfEnemies;
        m_AgentGroup = new SimpleMultiAgentGroup();


        for (int i = 0; i < numberOfEnemies; i++)
        {
            Vector3 position = GetRandomPosition();
            Vector3 newPosition = new Vector3(position.x, 0.5f, position.z);
            var enemyGO = Instantiate(enemyPrefab, newPosition, Quaternion.identity);
            var enemy = enemyGO.GetComponent<Enemy>();
            enemy.InitializeEnemy(this, m_AgentGroup);
        }
    }

    public Vector3 GetRandomPosition()
    {
        if (spawnZones.Count == 0)
        {
            Debug.LogError("No spawn zones assigned!");
            return Vector3.zero;
        }

        SpawnZone selectedSpawnZone = spawnZones[Random.Range(0, spawnZones.Count)];

        return selectedSpawnZone.SpawnPoint;
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
            if (enemy.gameObject.activeSelf)
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