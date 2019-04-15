using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class EnergyBall : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip chargesZappedClip;

    public void Start()
    {
        Assert.IsNotNull(chargesZappedClip);

        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ship"))
        {
            audioSource.clip = chargesZappedClip;
            audioSource.Play();

            Rigidbody rb = other.GetComponent<Rigidbody>();
            rb.velocity = new Vector3();
        }
    }
}
