using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Chaser : MonoBehaviour
{
    public EnergyBallProjectile[] energyBallProjectileClasses;
    public PlayerShip[] targets;

    public float maxForce;
    public float minDistance = 400000;

    private Vector3 moveDirection;
    private bool isCloseEnough = false;

    public void Move(float force)
    {
        if (isCloseEnough)
        {
            Rigidbody rigidbody = this.GetComponent<Rigidbody>();
            rigidbody.AddForce(moveDirection * force);
        }
    }

    public void Fire(Vector3 target)
    {
        if (isCloseEnough)
        {
            EnergyBallProjectile projectile = energyBallProjectileClasses[Random.Range(0, energyBallProjectileClasses.Length)];
            projectile.SetTarget(target);

            Instantiate(projectile, this.transform.position, this.transform.rotation);
        }
    }

    public Transform GetClosestTarget()
    {
        bool isCloseEnough = false;

        Transform closestTarget = this.transform;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (PlayerShip target in targets)
        {
            Transform potentialTarget = target.transform;

            Vector3 directionToTarget = potentialTarget.position - currentPosition;

            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;

                if (closestDistanceSqr < minDistance)
                {
                    isCloseEnough = true;
                    closestTarget = potentialTarget;
                }
            }
        }

        if (isCloseEnough)
            this.isCloseEnough = true;
        else
            this.isCloseEnough = false;

        return closestTarget;
    }

    public void SetMoveDirection(Vector3 moveDirection)
    {
        this.moveDirection = moveDirection;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ship"))
        {
            Destroy(gameObject);
        }
    }
}