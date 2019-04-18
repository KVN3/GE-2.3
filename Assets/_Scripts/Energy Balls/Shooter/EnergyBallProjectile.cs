using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBallProjectile : MonoBehaviour
{
    public float projSpeed;

    public Vector3 target;
    private Vector3 moveDirection;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitAndDestroy(10));

        Vector3 diff = target - this.transform.position;
        moveDirection = diff.normalized;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(target, Vector3.down);
        transform.Rotate(90, 0, 0);

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
}
