using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SpawnPointManager : MonoBehaviour
{
    public int rowLength;
    public int spacing;

    void Start()
    {
        Assert.IsTrue(rowLength > 0, "rowLength = 0");
        Assert.IsTrue(spacing > 0, "spacing = 0");
    }

    public SpawnPoint[,] GenerateSpawnPoints(Vector3 startPos)
    {
        SpawnPoint[,] tempSpawnPoints = new SpawnPoint[rowLength, rowLength];

        for (int i = 0; i < rowLength; i++)
        {
            for (int j = 0; j < rowLength; j++)
            {
                SpawnPoint sp = ScriptableObject.CreateInstance<SpawnPoint>();
                sp.position = new Vector3(startPos.x + (i * spacing), startPos.y, startPos.z + (j * spacing));
                tempSpawnPoints[i, j] = sp;
            }
        }

        return tempSpawnPoints;
    }
}
