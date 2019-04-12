using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Data Structs
[System.Serializable]
public struct PlayerConfig
{
    public float movementSpeedFactor;
    public float rotationSpeedFactor;

    public float maxSpeed;
}

[System.Serializable]
public struct PlayerFloatConfig
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

[System.Serializable]
public struct PlayerRunData
{
    public int currentLap;
    public int maxLaps;
    public TimeSpan raceTime;
    public List<TimeSpan> raceTimes;

    public bool raceFinished;

    // Calculated
    public float initialAngleY;
    public float currentSpeed;

    // Extra
    public float charges;
    public float score;
}
#endregion

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    #region Initialize and Assign Variables
    //public BulletData bulletData;
    public PlayerConfig config;
    public PlayerFloatConfig floatConfig;
    public PlayerRunData runData;
    public Engines shipEngines;

    private AudioSource audioSource;
    public AudioClip alarmSound;

    // Drag values
    private float maxDrag = 5f;
    private float initialSlowDownFactor = 0.01f;

    private bool upperBoundReached = false;
    private bool bottomBoundReached = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        runData.initialAngleY = transform.rotation.eulerAngles.y;

        // Engines off at start
        shipEngines.middleEngine.Deactivate();
        shipEngines.leftEngine.Deactivate();
        shipEngines.rightEngine.Deactivate();

        // Set currentlap, maxlaps, timer
        runData.currentLap = 0;
        runData.maxLaps = 3;
        runData.raceTime = TimeSpan.Parse("00:00:00.000");
        runData.raceTimes = new List<TimeSpan>();
        runData.raceFinished = false;
    }
    #endregion

    #region Movement
    public void Move(Vector3 force, float verticalInput, float horizontalInput)
    {
        Rigidbody rb = this.GetComponent<Rigidbody>();
        Vector3 vel = rb.velocity;
        Vector3 localVel = transform.InverseTransformVector(vel);

        float currSpeed = 0;

        if (localVel.z < 0)
            currSpeed = -localVel.z;
        else
            currSpeed = localVel.z;

        runData.currentSpeed = currSpeed;
        Debug.Log("Vehicle speed = " + runData.currentSpeed);

        if (runData.currentSpeed >= config.maxSpeed)
        {
            force = Vector3.zero;
        }

        // Apply drag
        ManageDrag(verticalInput, horizontalInput, rb);

        // Apply floating
        force.y = ApplyFloating();

        //if (horizontalInput > 0f || horizontalInput < 0f)
        //rb.velocity = (rb.velocity / 1.025f);


        rb.AddForce(force);
    }

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

    public void Break()
    {
        Rigidbody rb = this.GetComponent<Rigidbody>();

        if (rb.drag < maxDrag)
            rb.drag += 0.2f;

        if (rb.angularDrag < maxDrag)
            rb.angularDrag += 0.2f;

        //else
        //{
        //    if (rb.drag > 0)
        //        rb.drag--;

        //    if (rb.angularDrag > 0)
        //        rb.angularDrag--;

        //    if (rb.drag < 0)
        //        rb.drag = 0;

        //    if (rb.angularDrag < 0)
        //        rb.angularDrag = 0;
        //}
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

    private void ManageDrag(float verticalInput, float horizontalInput, Rigidbody rb)
    {
        if (GivingGas(verticalInput))
        {
            shipEngines.middleEngine.Activate();

            if (!Input.GetKey(KeyCode.Space))
            {
                if (rb.drag > 0)
                    rb.drag -= 0.6f;

                if (rb.angularDrag > 0)
                    rb.angularDrag -= 0.2f;

                if (rb.drag < 0)
                    rb.drag = 0;
            }
        }

        // Not giving gas, increase drag
        else
        {
            shipEngines.middleEngine.Deactivate();

            if (!Input.GetKey(KeyCode.Space))
            {
                if (rb.drag < maxDrag)
                {
                    float slowDownFactor = initialSlowDownFactor;
                    if (rb.drag >= 1)
                        slowDownFactor *= rb.drag;

                    rb.drag += slowDownFactor;
                }
            }
        }
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

    private bool GivingGas(float verticalInput)
    {
        if (verticalInput > 0)
            return true;

        return false;
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

        if (floatSpeed == 0)
            Debug.Log("Error... floatSpeed = 0");

        return floatSpeed;
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

        bottomBoundReached = false;
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

        bottomBoundReached = true;
        upperBoundReached = false;
        return false;
    }
    #endregion

    #region Collisions and Triggers
    // Collisions
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("OuterWall"))
        {
            Debug.Log("Hit OuterWall");
        }
        else
        {
            audioSource.clip = alarmSound;
            audioSource.Play();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        //    if (other.gameObject.CompareTag("BoosterPad"))
        //    {
        //        Debug.Log("booted & boosted");

        //        Rigidbody rb = this.GetComponent<Rigidbody>();
        //        Vector3 velocity = rb.velocity * 2;
        //        rb.AddForce(velocity);
        //    }

        if (other.gameObject.CompareTag("FinishLine"))
        {
            // Fixes an error when racetimes = null after restarting on Gianni's level
            if (runData.raceTimes == null)
                runData.raceTimes = new List<TimeSpan>();

            // Save lap and reset time if race has started (lap > 0)
            if (runData.currentLap > 0)
            {
                // Add laptime to racetimes if not finished
                if (!runData.raceFinished)
                    runData.raceTimes.Add(runData.raceTime);
            }
            // If finished
            if (runData.currentLap == runData.maxLaps) // 3/3 laps + finish
            {
                Debug.Log("Player Finished");

                runData.raceFinished = true;
            }
            else // Not finished
            {
                Debug.Log($"Player Crossed Finish Line");

                // Reset Time. Only reset when lap > 0 
                if (runData.currentLap > 0)
                    runData.raceTime = TimeSpan.Parse("00:00:00.000");

                // Add a lap
                runData.currentLap++;

            }
        }
    }
    #endregion
}




// Apply boundaries
//if (transform.position.x > PlayerConfig.rightBound && force.x > 0)
//{
//    rb.velocity = new Vector3(0f, force.y, force.z);
//}

//else if (transform.position.x < PlayerConfig.leftBound && force.x < 0)
//{
//    rb.velocity = new Vector3(0f, force.y, force.z);
//}
//else
//    rb.AddForce(force);

//float verticalInput = Input.GetAxis("Vertical");

//if (verticalInput < 0)
//{
//    //rb.drag = verticalInput * -10;
//    //rb.angularDrag = verticalInput * 10;
//}
//else
//{
//    rb.drag = 0;
//    rb.angularDrag = 0;
//    float correctedInput = verticalInput;
//    float currentAngleY = transform.rotation.eulerAngles.y;

//    if (currentAngleY >= 0 && currentAngleY < 90 || currentAngleY >= 270 && currentAngleY <= 360)
//    {
//        correctedInput = verticalInput * -1;
//    }

//    Vector3 forward = correctedInput * transform.forward * Time.deltaTime * PlayerConfig.movementSpeedFactor;


//    rb.AddRelativeForce(forward);
//}

//public void Shoot()
//{
//    //Select random bullet type
//    //int bulletIndex = Random.Range(0, bulletData.bullets.Length);
//    //Bullet bullet = bulletData.bullets[bulletIndex];
//    //bullet.isEnemyBullet = false;

//    //Spawn the bullet
//    //Instantiate(bullet, this.transform.position, this.transform.rotation);
//}

// Force andere kant drag implementeren 
// Voor de rotatie toepassing velocity oohale n als vfactor inverse transform je hem door de roatie van de tras=ns
// Krijg local snelheid, recht naar voren beweegt z (LOOKAAL) heel groot z y bijna 0
// Pas rotatie toe en vervolgens trans je velocity door nieuwe rotatie heen
// Krijg nieuwe vector3 worldspace, apply rb
// Lokaal altijd z