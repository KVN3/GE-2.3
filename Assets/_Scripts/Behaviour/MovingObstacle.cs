using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MovingObstacle : MonoBehaviour
{
    public Direction direction;

    public float delta = 1.5f;
    public float speed = 2.0f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        Vector3 v = startPos;

        if (direction == Direction.Z)
            v.z += delta * Mathf.Sin(Time.time * speed);
        else if (direction == Direction.X)
            v.x += delta * Mathf.Sin(Time.time * speed);
        else if (direction == Direction.Y)
            v.y += delta * Mathf.Sin(Time.time * speed);

        transform.position = v;
    }
}
