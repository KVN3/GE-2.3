using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ShipConfig
{
    public float movementSpeedFactor;
    public float rotationSpeedFactor;

    public float maxSpeed;
    public float maxDrag;

    public float initialAngleY;
    public float initialSlowDownFactor;
}

[System.Serializable]
public struct ShipFloatConfig
{
    public float topBound, bottomBound;
    public float floatSpeed;
}

[System.Serializable]
public struct Engines
{
    public Engine middleEngine;
    public Engine leftEngine;
    public Engine rightEngine;
}

public class Ship : MonoBehaviour
{
    public ShipConfig config;
    public ShipFloatConfig floatConfig;
    public Engines shipEngines;

    // Public run data
    public float currentSpeed;

    // Private run data
    private bool upperBoundReached = false;

    public virtual void Start()
    {
        config.initialAngleY = transform.rotation.eulerAngles.y;

        // Engines off at start
        TurnOffAllEngines();
    }

    #region Movement
    public void Move(Vector3 force, float verticalInput, float horizontalInput)
    {
        Rigidbody rb = this.GetComponent<Rigidbody>();
        Vector3 vel = rb.velocity;
        Vector3 localVel = transform.InverseTransformVector(vel);


        // Get current speed
        currentSpeed = GetCurrentSpeed(vel);

        // Apply floating
        force.y = ApplyFloating();

        if (currentSpeed < config.maxSpeed)
        {
            rb.AddForce(force);
        }
        else
        {
            Vector3 newLocalVel = localVel;
            newLocalVel.z = config.maxSpeed;

            if (localVel.z < 0)
                newLocalVel.z *= -1;

            rb.velocity = transform.TransformVector(newLocalVel);

            // Keep floating, but don't increase...
            rb.AddForce(0, force.y, 0);
        }

        //Debug.Log("Vehicle speed (" + localVel.z + ") = " + currentSpeed);
    }

    public float GetCurrentSpeed(Vector3 vel)
    {
        Vector3 localVel = transform.InverseTransformVector(vel);
        float currSpeed = localVel.z;

        if (localVel.z < 0)
            currSpeed *= -1;

        return currSpeed;
    }

    public void GivingGas()
    {
        Rigidbody rb = this.GetComponent<Rigidbody>();

        shipEngines.middleEngine.Activate();

        if (rb.drag > 0)
            rb.drag -= 0.6f;

        if (rb.angularDrag > 0)
            rb.angularDrag -= 0.2f;

        if (rb.drag < 0)
            rb.drag = 0;
    }

    public void NotGivingGas()
    {
        Rigidbody rb = this.GetComponent<Rigidbody>();

        shipEngines.middleEngine.Deactivate();

        if (rb.drag < config.maxDrag)
        {
            float slowDownFactor = config.initialSlowDownFactor;
            if (rb.drag >= 1)
                slowDownFactor *= rb.drag;

            rb.drag += slowDownFactor;
        }
    }

    public void Break()
    {
        Rigidbody rb = this.GetComponent<Rigidbody>();

        if (rb.drag < config.maxDrag)
            rb.drag += 0.2f;

        if (rb.angularDrag < config.maxDrag)
            rb.angularDrag += 0.2f;
    }
    #endregion

    #region Rotation
    public void Rotate(Vector3 acceleration, float horizontalInput)
    {
        ManageEngines(horizontalInput);
        Rigidbody b = GetComponent<Rigidbody>();
        Vector3 vel = b.velocity;
        Vector3 vel2 = b.velocity;
        Vector3 localVel = transform.InverseTransformVector(vel);

        Vector3 localVel2 = transform.InverseTransformVector(vel2);   // Transform worldspace vel naar local space

        transform.Rotate(acceleration);

        localVel2.x = Mathf.Lerp(localVel2.x, 0.0f, Time.deltaTime * 10);

        localVel2.y = Mathf.Lerp(localVel2.y, 0.0f, Time.deltaTime * 10);

        vel = transform.TransformVector(localVel);  // Transform terug naar worldspace

        b.velocity = vel;

        //Rigidbody rb = GetComponent<Rigidbody>();

        //Vector3 vel = rb.velocity;
        //Vector3 localVel = transform.InverseTransformVector(vel);     // Transform worldspace vel naar local space

        //// Je local vel zou nu vooral op de Z as hoog moeten zijn
        //transform.Rotate(acceleration);

        //vel = transform.TransformVector(localVel);      // Transform terug naar worldspace
        //rb.velocity = vel;    
    }

    public void SetAngle(Rigidbody rb)
    {
        float feloX = rb.velocity.x;

        if (feloX > 0)
        {
            if (!(transform.rotation.eulerAngles == new Vector3(0, 0, 25)))
            {
                transform.Rotate(0, 0, 25);
            }

        }
        else if (feloX < 0)
        {
            if (!(transform.rotation.eulerAngles == new Vector3(0, 0, -25)))
            {
                transform.Rotate(0, 0, -25);
            }
        }
        else if (feloX == 0)
        {
            transform.Rotate(0, 0, -25);
        }

        if (feloX == 0)
        {

        }
    }
    #endregion

    #region Engines
    private void TurnOffAllEngines()
    {
        shipEngines.middleEngine.Deactivate();
        shipEngines.leftEngine.Deactivate();
        shipEngines.rightEngine.Deactivate();
    }

    private void ManageEngines(float horizontalInput)
    {
        if (horizontalInput > 0)
        {
            shipEngines.leftEngine.Activate();
            shipEngines.rightEngine.Deactivate();
        }
        else if (horizontalInput < 0)
        {
            shipEngines.rightEngine.Activate();
            shipEngines.leftEngine.Deactivate();
        }
        else
        {
            shipEngines.leftEngine.Deactivate();
            shipEngines.rightEngine.Deactivate();
        }
    }
    #endregion

    #region Floating
    private float ApplyFloating()
    {
        float floatSpeed = 0;

        if (ShouldFloatUp())
            floatSpeed = floatConfig.floatSpeed;         
        else if (ShouldFloatDown())
            floatSpeed = -floatConfig.floatSpeed;

        ApplyFloatingBounds();

        if (floatSpeed == 0)
            Debug.Log("Error... floatSpeed = 0");

        return floatSpeed;
    }

    private void ApplyFloatingBounds()
    {
        float diff = Mathf.Round((floatConfig.topBound - floatConfig.bottomBound) * 10) / 10;

        if (transform.position.y < (floatConfig.bottomBound - diff))
        {
            transform.position = new Vector3(transform.position.x, floatConfig.bottomBound, transform.position.z);
        }
        else if (transform.position.y > (floatConfig.topBound + diff))
        {
            transform.position = new Vector3(transform.position.x, floatConfig.topBound, transform.position.z);
        }
    }

    private float GetHeightMiddle()
    {
        float diff = floatConfig.topBound - floatConfig.bottomBound;
        float middleHeight = floatConfig.topBound - (diff / 2);
        return middleHeight;
    }

    private bool ShouldFloatUp()
    {
        if (transform.position.y < floatConfig.topBound)
        {
            if (!upperBoundReached)
            {
                return true;
            }

        }

        upperBoundReached = true;
        return false;
    }
    private bool ShouldFloatDown()
    {
        if (transform.position.y > floatConfig.bottomBound)
        {
            if (upperBoundReached)
            {
                return true;
            }
        }

        upperBoundReached = false;
        return false;
    }
    #endregion

    #region Collisions and Triggers

    #endregion
}
