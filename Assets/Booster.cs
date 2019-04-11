using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour
{
    public float boostFactor = 2;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("booted & boosted");

        Rigidbody rb = other.GetComponent<Rigidbody>();
        Vector3 newVelocity = new Vector3(rb.velocity.x * boostFactor, rb.velocity.y, rb.velocity.z * boostFactor);
        rb.velocity = newVelocity;
    }
}
