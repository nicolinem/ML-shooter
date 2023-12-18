using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItems : MonoBehaviour
{
    public delegate void KeyCollectedAction();
    public static event KeyCollectedAction OnKeyCollected;

    private int keysCollected = 0;
    private PlayerShooter playerShooter;

    private void Start()
    {
        playerShooter = GetComponent<PlayerShooter>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
            keysCollected++;
            other.gameObject.SetActive(false);

            // Trigger the event when a key is collected
            OnKeyCollected?.Invoke();
        }
        else if (other.gameObject.CompareTag("Ammo"))
        {
            int ammoCount = 10; // Can change to make different ammo amounts
            playerShooter.AddAmmo(ammoCount);
            other.gameObject.SetActive(false); // Deactivate the ammo item
        }
    }
}
