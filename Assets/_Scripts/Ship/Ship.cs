using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ShipConfig
{
    public float movementSpeedFactor;
    public float rotationSpeedFactor;

    public float baseMaxSpeed;
    public float minDrag;
    public float maxDrag;

    public float initialAngleY;
    public float initialSlowDownFactor;

    public LocalSpawnPoint LocalSpawnPoint;
}

[System.Serializable]
public struct ShipFloatConfig
{
    public float floatDiff;
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
public struct ShipSounds
{
    public AudioClip shutDownClip;
    public AudioClip restartClip;

    public AudioClip speedBoostClip;
    public AudioClip alarmClip;
    public AudioClip[] shootingClips;
    public AudioClip[] pickUpClips;
    public AudioClip lapPassedClip;

    public AudioClip[] victoryClips;
    public AudioClip lossClip;
    public AudioClip achievementClip;
}

public class Ship : MonoBehaviour
{
    // Config
    public ShipConfig config;
    public ShipFloatConfig floatConfig;
    public Engines shipEngines;
    public ShipSounds shipSounds;

    // Public run data
    public float currentSpeed;
    public float currentMaxSpeed;

    // Private run data
    private float floatTopBound, floatBottomBound;
    private bool upperBoundReached;
    private bool systemsDown;
    private AudioSource audioSource;

    // Collectables
    public Collectable collectableItemClass;
    public int itemAmount;

    public virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();

        currentMaxSpeed = config.baseMaxSpeed;
        config.initialAngleY = transform.rotation.eulerAngles.y;

        // Set float bounds
        floatTopBound = transform.position.y + floatConfig.floatDiff;
        floatBottomBound = transform.position.y - floatConfig.floatDiff;

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

        if (currentSpeed < currentMaxSpeed)
        {
            rb.AddForce(force);
        }
        else
        {
            Vector3 newLocalVel = localVel;
            newLocalVel.z = currentMaxSpeed;

            if (localVel.z < 0)
                newLocalVel.z *= -1;

            rb.velocity = transform.TransformVector(newLocalVel);

            // Keep floating, but don't increase speed...
            rb.AddForce(0, force.y, 0);
        }

        //Debug.Log("Vehicle speed (" + localVel.z + ") = " + currentSpeed + " MAX = " + currentMaxSpeed);
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

        if (rb.drag > config.minDrag)
            rb.drag -= 0.3f;

        if (rb.angularDrag > 0)
            rb.angularDrag -= 0.2f;

        if (rb.drag < config.minDrag)
            rb.drag = config.minDrag;
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
    public void Rotate(Vector3 acceleration, float horizontalInput, MovementState sideMovementState)
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        // Worldspace Vel -> Local Vel
        Vector3 vel = rb.velocity;
        Vector3 localVel = transform.InverseTransformVector(vel);

        // Rotate
        float x = transform.localEulerAngles.x;
        float y = transform.localEulerAngles.y + acceleration.y;
        float z = GetAngleZ(sideMovementState, acceleration.z);
        transform.localEulerAngles = new Vector3(x, y, z);

        // Local Vel -> Worldspace Vel
        vel = transform.TransformVector(localVel);
        rb.velocity = vel;
    }

    public void ResetAngleZ(float addValue)
    {
        float angleZ = transform.localEulerAngles.z;

        if (angleZ > 3 && angleZ <= 180)
        {
            transform.Rotate(0f, 0f, -addValue);
        }
        else if (angleZ <= 357 && angleZ > 180)
        {
            transform.Rotate(0f, 0f, addValue);
        }
    }

    private float GetAngleZ(MovementState sideMovementState, float addValue)
    {
        float angleZ = transform.localEulerAngles.z;

        float z = transform.localEulerAngles.z;

        if ((angleZ < 20 || angleZ > 300) && sideMovementState.Equals(MovementState.RIGHT))
        {
            z = z + addValue;
        }
        else if ((angleZ > 340 || angleZ <= 50) && sideMovementState.Equals(MovementState.LEFT))
        {
            z = z + addValue;
        }

        return z;
    }

    #endregion

    #region Engines

    private IEnumerator FlickerEngines(int times)
    {
        SetEnginesRestartMode(true);

        int counter = 0;

        while (counter < times)
        {
            TurnOnAllEngines();
            yield return new WaitForSeconds(Random.Range(0.3f, 0.6f));
            TurnOffAllEngines();
            yield return new WaitForSeconds(Random.Range(0.3f, 0.6f));

            counter++;
        }

        SetEnginesRestartMode(false);
        StartUp();
    }

    private void SetEnginesRestartMode(bool isOn)
    {
        float startSpeed = .5f;
        float lifeTime = 2f;

        if (isOn)
        {
            startSpeed = 20f;
            lifeTime = .2f;
        }

        // Start speed
        shipEngines.leftEngine.SetStartSpeed(startSpeed);
        shipEngines.rightEngine.SetStartSpeed(startSpeed);
        shipEngines.middleEngine.SetStartSpeed(startSpeed);

        // Life time
        shipEngines.leftEngine.SetLifeTime(lifeTime);
        shipEngines.rightEngine.SetLifeTime(lifeTime);
        shipEngines.middleEngine.SetLifeTime(lifeTime);

    }

    private void TurnOnAllEngines()
    {
        shipEngines.middleEngine.Activate();
        shipEngines.leftEngine.Activate();
        shipEngines.rightEngine.Activate();
    }

    private void TurnOffAllEngines()
    {
        shipEngines.middleEngine.Deactivate();
        shipEngines.leftEngine.Deactivate();
        shipEngines.rightEngine.Deactivate();
    }

    public void ManageEngines(float horizontalInput)
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

        //if (floatSpeed == 0)
        //    Debug.Log("Error... floatSpeed = 0");

        return floatSpeed;
    }

    private void ApplyFloatingBounds()
    {
        float diff = Mathf.Round((floatTopBound - floatBottomBound) * 10) / 10;

        if (transform.position.y < (floatBottomBound - diff))
        {
            transform.position = new Vector3(transform.position.x, floatBottomBound, transform.position.z);
        }
        else if (transform.position.y > (floatTopBound + diff))
        {
            transform.position = new Vector3(transform.position.x, floatTopBound, transform.position.z);
        }
    }

    private float GetHeightMiddle()
    {
        float diff = floatTopBound - floatBottomBound;
        float middleHeight = floatTopBound - (diff / 2);
        return middleHeight;
    }

    private bool ShouldFloatUp()
    {
        if (transform.position.y < floatTopBound)
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
        if (transform.position.y > floatBottomBound)
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

    #region Shooting
    public void Shoot()
    {
        if (itemAmount > 0)
        {
            if (collectableItemClass is JammerProjectile)
            {
                // This object's rotation + 180y
                Vector3 rot = transform.rotation.eulerAngles;
                rot = new Vector3(rot.x, rot.y + 180, rot.z);
                Quaternion rotation = Quaternion.Euler(rot);

                JammerProjectile projectile = (JammerProjectile)Instantiate(collectableItemClass, config.LocalSpawnPoint.transform.position, rotation);
                projectile.owner = this;

                PlaySound(SoundType.SHOOTING);
                itemAmount--;
            }
        }
    }
    #endregion

    #region Sounds
    public void PlaySound(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.SPEEDBOOST:
                audioSource.clip = shipSounds.speedBoostClip;
                break;
            case SoundType.PICKUP:
                audioSource.clip = shipSounds.pickUpClips[Random.Range(0, shipSounds.pickUpClips.Length)];
                break;
            case SoundType.ALARM:
                audioSource.clip = shipSounds.alarmClip;
                break;
            case SoundType.SHOOTING:
                audioSource.clip = shipSounds.shootingClips[Random.Range(0, shipSounds.shootingClips.Length)];
                break;

            case SoundType.SHUTDOWN:
                audioSource.clip = shipSounds.shutDownClip;
                break;
            case SoundType.RESTART:
                audioSource.clip = shipSounds.restartClip;
                break;

            case SoundType.VICTORY:
                audioSource.clip = shipSounds.victoryClips[Random.Range(0, shipSounds.victoryClips.Length)];
                break;
            case SoundType.LOSS:
                audioSource.clip = shipSounds.lossClip;
                break;
            case SoundType.LAPPASSED:
                audioSource.clip = shipSounds.lapPassedClip;
                break;
        }


        audioSource.Play();
    }
    #endregion

    #region Collisions and Triggers
    public void GetHitByEmp(int duration)
    {
        if (!IsSystemDown())
        {
            ShutDown();
            RestoreSystem();
        }
    }

    private IEnumerator TemporaryShutDown(int duration)
    {
        if (!IsSystemDown())
        {
            ShutDown();
            yield return new WaitForSeconds(duration);
            RestoreSystem();
        }
    }

    public bool IsSystemDown()
    {
        return systemsDown;
    }

    private void ShutDown()
    {
        PlaySound(SoundType.SHUTDOWN);
        systemsDown = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = true;

        TurnOffAllEngines();
    }

    private void RestoreSystem()
    {
        StartCoroutine(FlickerEngines(3));
    }

    private void StartUp()
    {
        PlaySound(SoundType.RESTART);
        systemsDown = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }
    #endregion
}

// TO DO: camera straight while turning
// TO DO: temporary increase of air drag while turning (big question mark)