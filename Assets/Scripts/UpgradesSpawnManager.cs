using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesSpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class UpgradePrefab
    {
        public GameObject prefab;
        public float spawnProbability = 1.0f;
    }

    public List<UpgradePrefab> upgradePrefabs;
    public int numberOfUpgrades = 5;
    public List<SpawnZone> spawnZones;

    private List<GameObject> spawnedUpgrades = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < numberOfUpgrades; i++)
        {
            SpawnUpgrade();
        }
    }

    private void SpawnUpgrade()
    {
        if (spawnZones.Count == 0)
        {
            Debug.LogError("No spawn zones assigned for upgrades!");
            return;
        }

        SpawnZone selectedSpawnZone = spawnZones[Random.Range(0, spawnZones.Count)];
        Vector3 spawnPoint = selectedSpawnZone.SpawnPoint;

        UpgradePrefab selectedUpgradePrefab = ChooseRandomUpgradePrefab();
        GameObject upgradeGO = Instantiate(selectedUpgradePrefab.prefab, spawnPoint, Quaternion.identity);
        spawnedUpgrades.Add(upgradeGO);
    }

    private UpgradePrefab ChooseRandomUpgradePrefab()
    {
        float totalProbability = 0f;
        foreach (var upgrade in upgradePrefabs)
        {
            totalProbability += upgrade.spawnProbability;
        }

        float randomPoint = Random.Range(0, totalProbability);
        float currentProbability = 0f;

        foreach (var upgrade in upgradePrefabs)
        {
            currentProbability += upgrade.spawnProbability;
            if (currentProbability >= randomPoint)
            {
                return upgrade;
            }
        }

        return upgradePrefabs[upgradePrefabs.Count - 1]; // Fallback in case of rounding errors
    }

    // Add any additional methods you need for the UpgradesSpawnManager here
}