using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class EnergyBall : MonoBehaviour
{
    public int shutDownDuration = 4;

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ship"))
        {
            PlayerShip playerShip = other.GetComponent<PlayerShip>();
            Rigidbody rb = other.GetComponent<Rigidbody>();

            playerShip.GetHitByEmp(shutDownDuration);
        }
    }
}
