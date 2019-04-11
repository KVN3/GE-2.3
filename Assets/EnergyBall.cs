using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBall : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {   
        if (other.gameObject.CompareTag("Ship"))
        {
            Debug.Log("pushed by energy ball");
            Rigidbody myRb = this.GetComponent<Rigidbody>();
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            rb.velocity = new Vector3();
        }
    }
}
