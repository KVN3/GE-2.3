using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverManager : EnemyManager
{
    protected override void SpawnEnemy()
    {
        base.SpawnEnemy();

        RandomMover moverClass = enemyClasses[Random.Range(0, enemyClasses.Length)] as RandomMover;
        RandomMover mover = Instantiate(moverClass, sp.transform.position, Quaternion.identity);

        mover.xMax = sp.xMax;
        mover.xMin = sp.xMin;
        mover.zMax = sp.zMax;
        mover.zMin = sp.zMin;
        mover.maxSpeed = sp.maxSpeed;

        mover.shooterModule.SetTargets(players);
        mover.SetManager(this);
        enemies.Add(mover);
    }
}
