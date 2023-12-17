using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItems : MonoBehaviour
{
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

            //Possible to tell the key manager that the key has been picked up, or the game manager
            other.gameObject.SetActive(false);
        }

        else if (other.gameObject.CompareTag("Ammo"))
        {
            int ammoCount = 10; // Can change to make different ammoamounts
            playerShooter.AddAmmo(ammoCount);
            other.gameObject.SetActive(false); // Deactivate the ammo item
        }
    }
}
