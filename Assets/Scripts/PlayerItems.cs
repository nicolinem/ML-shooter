using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItems : MonoBehaviour
{
    public delegate void KeyCollectedAction();
    public static event KeyCollectedAction OnKeyCollected;

    private int keysCollected = 0;
    private PlayerShooter playerShooter;
    public GameObject wallToDelete;
    private int totalKeysRequired = 3;
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

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

            // Check if the player has found all keys
            if (keysCollected >= totalKeysRequired)
            {
                audioManager.PlaySFX(audioManager.unlock);
                DeleteWall();
            }
            else
            {
                audioManager.PlaySFX(audioManager.key);
            }
        }
        else if (other.gameObject.CompareTag("Ammo"))
        {
            audioManager.PlaySFX(audioManager.ammo);
            int ammoCount = 5; // Can change to make different ammo amounts
            playerShooter.AddAmmo(ammoCount);
            other.gameObject.SetActive(false); // Deactivate the ammo item
        }
    }

    private void DeleteWall()
    {
        if (wallToDelete != null)
        {
            Destroy(wallToDelete);
        }
        else
        {
            Debug.LogWarning("Wall reference is null. Make sure to assign the wall GameObject in the Inspector.");
        }
    }
}
