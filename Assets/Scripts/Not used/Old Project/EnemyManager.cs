using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyManager : MonoBehaviour
{
    public Player player;
    public Enemy[] enemyClasses;
    public int desiredAliveEnemyCount;

    //public Bullet[] bullets;

    public List<Enemy> enemies { get; } = new List<Enemy>();

    private void Start()
    {
        Assert.IsFalse(enemyClasses.Length == 0);
        StartCoroutine(SpawnLoop());
    }


    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (enemies.Count < desiredAliveEnemyCount)
                SpawnEnemy();

            yield return new WaitForSeconds(5);
        }
    }

    private void SpawnEnemy()
    {
        Enemy enemyClass = enemyClasses[Random.Range(0, enemyClasses.Length)];

        Vector3 pos = new Vector3();
        pos.x = Random.Range(-5.0f, 5.0f);
        pos.z = Random.Range(-5.0f, 5.0f);
        pos.y = 5;

        Enemy enemy = Instantiate(enemyClass, pos, Quaternion.identity);
        enemy.player = player;
        //enemy.bullets = bullets;

        enemies.Add(enemy);
    }
}
