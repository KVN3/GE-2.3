using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBallProjectile : MonoBehaviour
{
    public float projSpeed;

    private Vector3 target;
    private Vector3 moveDirection;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 diff = target - this.transform.position;
        moveDirection = diff.normalized;

        StartCoroutine(WaitAndDestroy(10));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Rigidbody rigidbody = this.GetComponent<Rigidbody>();
        rigidbody.AddForce(moveDirection * projSpeed);
    }

    void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }

    IEnumerator WaitAndDestroy(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    public void SetTarget(Vector3 target)
    {
        this.target = target;
    }
}
