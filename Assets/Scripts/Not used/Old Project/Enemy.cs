using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    public Player player;
    public float maxForce;
    //public Bullet[] bullets;
    public Vector3 moveDirection;

    public void Move(float force)
    {
        Rigidbody rigidbody = this.GetComponent<Rigidbody>();
        rigidbody.AddForce(moveDirection * force);
    }

    public void Fire(Vector3 target)
    {
        //Select random bullet type
        //int bulletIndex = Random.Range(0, bullets.Length);
        //Bullet bullet = bullets[bulletIndex];
        //bullet.isEnemyBullet = true;
        //bullet.target = target;

        //Spawn the bullet
        //Instantiate(bullet, this.transform.position, this.transform.rotation);
    }
}
