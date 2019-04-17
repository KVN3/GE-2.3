using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserController : MonoBehaviour
{
    public Chaser chaser;

    void Start()
    {
        StartCoroutine(PerformMovement());
        StartCoroutine(PerformFiring());
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float moveForce = Random.Range(1, chaser.maxForce);
        chaser.Move(moveForce);
    }

    //--MOVEMENT--
    IEnumerator PerformMovement()
    {
        while (true)
        {
            Vector3 target = FindTarget();
            yield return MoveToTarget(target);
        }
    }

    IEnumerator MoveToTarget(Vector3 target)
    {
        Vector3 diff = target - this.transform.position;
        chaser.SetMoveDirection(diff.normalized);

        yield return new WaitForFixedUpdate();
    }

    IEnumerator PerformFiring()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);
            Vector3 target = FindTarget();
            chaser.Fire(target);
        }
    }

    // Methods
    private Vector3 FindTarget()
    {
        return chaser.GetClosestTarget().position;
    }
}
