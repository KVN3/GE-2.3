﻿using System.Collections;
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
    public PlayerShip playerShip;
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
            playerShip.Break();

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
        // Engines & Drag
        if (GivingGas(verticalInput) && !Input.GetKey(KeyCode.Space))
            playerShip.GivingGas();
        else if (!Input.GetKey(KeyCode.Space))
            playerShip.NotGivingGas();

        // Rotation
        float y = horizontalInput * playerShip.config.rotationSpeedFactor * rotationalFactor;
        Debug.Log(y);
        playerShip.Rotate(new Vector3(0f, y), horizontalInput);

        // Thrust
        Vector3 forward = -1 * verticalInput * transform.forward * Time.deltaTime * playerShip.config.movementSpeedFactor * forwardFactor;
        playerShip.Move(forward, verticalInput, horizontalInput);
    }

    private bool GivingGas(float verticalInput)
    {
        if (verticalInput > 0)
            return true;

        return false;
    }

    //private void HandleShooting()
    //{
    //    if (Input.GetButtonDown("Fire1"))
    //        playerShip.Shoot();
    //}
}