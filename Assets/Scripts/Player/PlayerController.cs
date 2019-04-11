using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Cameras
{
    public GameObject firstPersonCamera;
    public GameObject thirdPersonCamera;
}

public class PlayerController : MonoBehaviour
{
    public Player player;
    public Accelerometer accelerometer;
    public Cameras cameras;

    public bool useAccelerometerControls = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            cameras.firstPersonCamera.SetActive(!cameras.firstPersonCamera.activeSelf);
            cameras.thirdPersonCamera.SetActive(!cameras.thirdPersonCamera.activeSelf);
        }

        // Breaking
        if (Input.GetKey(KeyCode.Space))
            player.PBreak();

        // Controls
        if (Input.GetKeyDown(KeyCode.C))
            useAccelerometerControls = !useAccelerometerControls;
    }

    void FixedUpdate()
    {
        HandleMovement();
    }


    private void HandleMovement()
    {
        float horizontalInput;
        float verticalInput;
        float forwardFactor = 1f;
        float rotationalFactor = 1f;

        // Accelerometer | Keyboard
        if (useAccelerometerControls)
        {
            Vector3 acceleration = accelerometer.GetValue();
            horizontalInput = accelerometer.ParseAccelerationToInput(acceleration.x, Direction.X);
            verticalInput = -accelerometer.ParseAccelerationToInput(acceleration.y, Direction.Y);
            forwardFactor = 2f;
            rotationalFactor = 1f;
        }
        else
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
        }

        ApplyMovement(horizontalInput, verticalInput, forwardFactor, rotationalFactor);
    }

    private void ApplyMovement(float horizontalInput, float verticalInput, float forwardFactor, float rotationalFactor)
    {
        // Rotation
        float y = horizontalInput * player.playerData.rotationSpeed * rotationalFactor;
        player.Rotate(new Vector3(0f, y), horizontalInput);

        // Thrust
        Vector3 forward = -1 * verticalInput * transform.forward * Time.deltaTime * player.playerData.forwardSpeed * forwardFactor;
        player.PMove(forward, verticalInput, horizontalInput);
    }

    //private void HandleShooting()
    //{
    //    if (Input.GetButtonDown("Fire1"))
    //        player.Shoot();
    //}
}


////float movement = Time.deltaTime * player.playerData.sideMovementSpeed;
////float x = Input.GetAxis("Horizontal") * movement;
////float y = Input.GetAxis("Vertical") * movement;

//// Rotation
//float y = Input.GetAxis("Horizontal") * player.playerData.rotationSpeed;
//player.Rotate(new Vector3(0f, y));

//// Movement
//player.PMove(new Vector3(0f, 0f, 0f));

////transform.position += transform.forward * Time.deltaTime * player.playerData.sideMovementSpeed;