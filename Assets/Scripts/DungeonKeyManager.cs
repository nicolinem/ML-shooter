using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonKeyManager : MonoBehaviour
{
    public GameObject keyPrefab;
    public int numberOfKeys = 3;
    public List<SpawnZone> spawnZones;

    private List<DungeonKey> keys = new List<DungeonKey>();

    void Start()
    {
        for (int i = 0; i < numberOfKeys; i++)
        {
            SpawnKey();
        }
    }

    private void SpawnKey()
    {
        if (spawnZones.Count == 0)
        {
            Debug.LogError("No spawn zones assigned for keys!");
            return;
        }

        SpawnZone selectedSpawnZone = spawnZones[Random.Range(0, spawnZones.Count)];

        float staticYPosition = 0.5f;

        Vector3 spawnPoint = new Vector3(
            selectedSpawnZone.SpawnPoint.x,
            staticYPosition,
            selectedSpawnZone.SpawnPoint.z
        );

        var keyGO = Instantiate(keyPrefab, spawnPoint, Quaternion.identity);
        var dungeonKey = keyGO.GetComponent<DungeonKey>();
        keys.Add(dungeonKey);
    }

}