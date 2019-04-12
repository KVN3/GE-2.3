using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Enemy enemy;

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
        float moveForce = Random.Range(1, enemy.maxForce);
        enemy.Move(moveForce);
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
        while (true)
        {
            Vector3 diff = target - this.transform.position;
            enemy.moveDirection = diff.normalized;

            if (diff.sqrMagnitude < (3 * 3))
                break;

            yield return new WaitForFixedUpdate();
        }
    }
    //--MOVEMENT--

    //--FIRING--
    IEnumerator PerformFiring()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);
            Vector3 target = FindTarget();
            enemy.Fire(target);
        }
    }
    //--FIRING--

    // Methods
    private Vector3 FindTarget()
    {
        return enemy.player.transform.position;
    }
}
