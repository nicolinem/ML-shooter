using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera playerCamera;
    public float turnSpeed = 4.0f;
    public float moveSpeed = 2.0f;
    public float minTurnAngle = -90.0f;
    public float maxTurnAngle = 90.0f;
    private float rotX;
    private Rigidbody playerRb;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        // Prevent Rigidbody from rotating
        playerRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (!PauseMenu.GameIsPaused)
        {
            MouseAiming();
        }
    }

    void FixedUpdate()
    {
        if (!PauseMenu.GameIsPaused)
        {
            KeyboardMovement();
        }
    }

    void MouseAiming()
    {
        // Horizontal rotation (player body)
        float y = Input.GetAxis("Mouse X") * turnSpeed;
        transform.Rotate(0, y, 0);

        // Vertical rotation (camera)
        rotX += Input.GetAxis("Mouse Y") * turnSpeed;
        rotX = Mathf.Clamp(rotX, minTurnAngle, maxTurnAngle);
        playerCamera.transform.localEulerAngles = new Vector3(-rotX, 0, 0);
    }

    void KeyboardMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical);
        direction = transform.TransformDirection(direction);

        // Move using Rigidbody
        playerRb.MovePosition(playerRb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }
}