using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Data Structs
[System.Serializable]
public struct PlayerRunData
{
    public int currentLap;
    public int maxLaps;
    public TimeSpan raceTime;
    public List<TimeSpan> raceTimes;

    public bool raceFinished;

    // Extra
    public float charges;
    public float score;
}
#endregion

[RequireComponent(typeof(Rigidbody))]
public class PlayerShip : Ship
{
    #region Initialize and Assign Variables
    public PlayerRunData runData;
    
    private AudioSource audioSource;
    public AudioClip alarmClip;

    public override void Start()
    {
        base.Start();

        audioSource = GetComponent<AudioSource>();

        // Set currentlap, maxlaps, timer
        runData.currentLap = 0;
        runData.maxLaps = 3;
        runData.raceTime = TimeSpan.Parse("00:00:00.000");
        runData.raceTimes = new List<TimeSpan>();
        runData.raceFinished = false;
    }
    #endregion

    #region Collisions and Triggers
    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("OuterWall"))
        {
            audioSource.clip = alarmClip;
            audioSource.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
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
                Debug.Log("playerShip Finished");

                runData.raceFinished = true;
            }
            else // Not finished
            {
                Debug.Log($"playerShip Crossed Finish Line");

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
//if (transform.position.x > ShipConfig.rightBound && force.x > 0)
//{
//    rb.velocity = new Vector3(0f, force.y, force.z);
//}

//else if (transform.position.x < ShipConfig.leftBound && force.x < 0)
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

//    Vector3 forward = correctedInput * transform.forward * Time.deltaTime * ShipConfig.movementSpeedFactor;


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