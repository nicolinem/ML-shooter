using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZone : MonoBehaviour
{
   

    public Vector3 startPosition;

    public Vector3 SpawnPoint
    {
        get
        {

            Vector3 randomPosition = GetRandomPosition();

            bool positionFound = false;
            int maxAttempts = 100;
            int attempts = 0;

            while (!positionFound && attempts < maxAttempts)
            {
                attempts++;
                Vector3 spawn = GetRandomPosition();
                randomPosition = new Vector3(spawn.x, 1f, spawn.z); // Assuming y=1 is slightly above the ground level


                Collider[] colliders = Physics.OverlapSphere(randomPosition, 0.5f); // 0.5f is the radius of the check, adjust as needed for your game

                if (colliders.Length == 0)
                {
                    positionFound = true;

                    randomPosition = new Vector3(spawn.x, 0.1f, spawn.z);
                }
            }

            if (!positionFound)
            {
                Debug.LogError("Failed to find a free position for enemy spawn after " + maxAttempts + " attempts.");
                return Vector3.zero;
            }

            return randomPosition;

        }
    }

    private Vector3 GetRandomPosition()
    {

        Vector2 position = Random.insideUnitCircle * 6f;
        var randomPosition = new Vector3(startPosition.x + position.x, 0, startPosition.z + position.y);

        return randomPosition;

    }


    void Start()
    {
        startPosition = transform.position;

    }


    void Update()
    {

    }
}



