using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public Transform shootingPoint;

    public PointsManager pointsManager;
    public Camera playerCamera;
    public int maxShotsBeforeCooldown = 5;
    public float cooldownTime = 1f;
    public int damage = 50;
    public int maxAmmo = 20; // Maximum ammo the player can hold

    public int startAmmo = 5;

    private int shotsFired;
    private int currentAmmo;
    private float cooldownTimer;
    private bool isCooldown;

    public Projectile projectile;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start()
    {
        currentAmmo = startAmmo; // Initialize ammo
        pointsManager.UpdateAmmunitionUI(startAmmo);
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
        audioManager.PlaySFX(audioManager.shooting);

        shotsFired++;
        currentAmmo--;
        pointsManager.UpdateAmmunitionUI(currentAmmo);

        // Implement shooting logic here
        var layerMask = 1 << LayerMask.NameToLayer("Player");
        var direction = playerCamera.transform.forward;
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));


        //check if ray hits something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out RaycastHit hitt))
            targetPoint = hitt.point;
        else
            targetPoint = ray.GetPoint(75); //Just a point far away from the player

        Vector3 directionWithoutSpread = targetPoint - shootingPoint.position;

        var spawnedProjectile = Instantiate(projectile, shootingPoint.position, Quaternion.Euler(0f, -90f, 0f));
        spawnedProjectile.SetDirection(directionWithoutSpread);

        Debug.DrawRay(shootingPoint.position, directionWithoutSpread, Color.green, 2f);

        // if (Physics.Raycast(shootingPoint.position, directionWithoutSpread, out var hit, 200f, layerMask))
        // {
        //     Enemy hitEnemy = hit.transform.GetComponent<Enemy>();
        //     if (hitEnemy != null)
        //     {
        //         hitEnemy.GetShot(damage);
        //     }
        // }
        // else
        // {
        //     // Log miss or handle it
        // }

        // Start cooldown if max shots reached
        if (shotsFired >= maxShotsBeforeCooldown)
        {
            StartCooldown();
        }
    }

    public void AddAmmo(int ammoCount)
    {
        currentAmmo = Mathf.Min(currentAmmo + ammoCount, maxAmmo);
        pointsManager.UpdateAmmunitionUI(currentAmmo);
    }
}
