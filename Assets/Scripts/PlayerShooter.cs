using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public Transform shootingPoint;
    public Camera playerCamera;
    public int maxShotsBeforeCooldown = 5;
    public float cooldownTime = 1f;
    public int damage = 50;
    public int maxAmmo = 20; // Maximum ammo the player can hold

    private int shotsFired;
    private int currentAmmo;
    private float cooldownTimer;
    private bool isCooldown;



    private void Start()
    {
        currentAmmo = maxAmmo; // Initialize ammo
    }

    private void Update()
    {
        HandleShooting();
    }

    private void HandleShooting()
    {
        if (isCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                EndCooldown();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space) && currentAmmo > 0)
        {
            Shoot();
        }
    }

    private void StartCooldown()
    {
        isCooldown = true;
        cooldownTimer = cooldownTime;
    }

    private void EndCooldown()
    {
        isCooldown = false;
        shotsFired = 0;
    }

    private void Shoot()
    {
        shotsFired++;
        currentAmmo--;

        // Implement shooting logic here
        var layerMask = 1 << LayerMask.NameToLayer("Player");
        var direction = playerCamera.transform.forward;

        Debug.DrawRay(shootingPoint.position, direction * 100f, Color.green, 2f);

        if (Physics.Raycast(shootingPoint.position, direction, out var hit, 200f, layerMask))
        {
            Enemy hitEnemy = hit.transform.GetComponent<Enemy>();
            if (hitEnemy != null)
            {
                hitEnemy.GetShot(damage);
            }
        }
        else
        {
            // Log miss or handle it
        }

        // Start cooldown if max shots reached
        if (shotsFired >= maxShotsBeforeCooldown)
        {
            StartCooldown();
        }
    }

    public void AddAmmo(int ammoCount)
    {
        currentAmmo = Mathf.Min(currentAmmo + ammoCount, maxAmmo);
    }
}
