using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ChaserManager : MonoBehaviour
{
    public Chaser[] chaserClasses;
    public int desiredAliveChaserCount;

    private PlayerShip[] players;

    private List<LocalSpawnPoint> spawnPoints;
    private List<LocalSpawnPoint> usedSpawnPoints;

    private List<Chaser> chasers = new List<Chaser>();

    private void Start()
    {
        Assert.IsFalse(chaserClasses.Length == 0);

        usedSpawnPoints = new List<LocalSpawnPoint>();

        StartCoroutine(SpawnLoop());
    }


    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (chasers.Count < desiredAliveChaserCount)
                SpawnChaser();

            yield return new WaitForSeconds(3);
        }
    }

    private void SpawnChaser()
    {
        Chaser chaserClass = chaserClasses[Random.Range(0, chaserClasses.Length)];

        if (spawnPoints.Count == 0)
        {
            foreach(LocalSpawnPoint spawnPoint in usedSpawnPoints)
                spawnPoints.Add(spawnPoint);

            usedSpawnPoints = new List<LocalSpawnPoint>();
        }

        //LocalSpawnPoint[] sps = new LocalSpawnPoint[spawnPoints.Count];

        LocalSpawnPoint sp = spawnPoints[Random.Range(0, spawnPoints.Count)];
        spawnPoints.Remove(sp);
        usedSpawnPoints.Add(sp);

        Chaser chaser = Instantiate(chaserClass, sp.transform.position, Quaternion.identity);
        chaser.SetTargets(players);
        chaser.SetManager(this);
            
        chasers.Add(chaser);
    }

    public void SetPlayers(PlayerShip[] players)
    {
        this.players = players;
    }

    public void SetSpawnPoints(List<LocalSpawnPoint> spawnPoints)
    {
        this.spawnPoints = spawnPoints;
    }

    public void RemoveFromAliveChasers(Chaser chaser)
    {
        chasers.Remove(chaser);
    }
}
