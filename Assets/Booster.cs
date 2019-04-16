using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Booster : MonoBehaviour
{
    public float boostFactor = 2;
    public float maxSpeedIncrease = 10;
    public float boostDuration = 3;

    private AudioSource audioSource;
    public AudioClip boostClip;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(boostClip);

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator ApplySpeedBoost(Ship ship, Rigidbody rb)
    {
        // Increase max speed
        ship.currentMaxSpeed += maxSpeedIncrease;

        Vector3 newVelocity = new Vector3(rb.velocity.x * boostFactor, rb.velocity.y, rb.velocity.z * boostFactor);
        rb.velocity = newVelocity;

        yield return new WaitForSeconds(boostDuration);

        // Restore max speed
        ship.currentMaxSpeed -= maxSpeedIncrease;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ship"))
        {
            PlayerShip playerShip = other.GetComponent<PlayerShip>();
            Rigidbody rb = other.GetComponent<Rigidbody>();

            playerShip.PlaySound(SoundType.SPEEDBOOST);
            StartCoroutine(ApplySpeedBoost(playerShip, rb));
        }
    }
}

// TO DO: increase moveSpeedFactor in ship instead of using rb in booster