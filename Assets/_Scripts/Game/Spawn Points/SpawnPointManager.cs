using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public struct SpawnPointManagerSettings
{
    public int rowLength;
    public int spacing;
}

public class SpawnPointManager : MonoBehaviour
{
    public SpawnPointManagerSettings settings;

    void Start()
    {
        Assert.IsTrue(settings.rowLength > 0, "rowLength = 0");
        Assert.IsTrue(settings.spacing > 0, "spacing = 0");
    }

    public SpawnPoint[,] GenerateSpawnPoints(Vector3 startPos)
    {
        SpawnPoint[,] tempSpawnPoints = new SpawnPoint[settings.rowLength, settings.rowLength];

        for (int i = 0; i < settings.rowLength; i++)
        {
            for (int j = 0; j < settings.rowLength; j++)
            {
                SpawnPoint sp = ScriptableObject.CreateInstance<SpawnPoint>();
                sp.position = new Vector3(startPos.x + (i * settings.spacing), startPos.y, startPos.z + (j * settings.spacing));
                tempSpawnPoints[i, j] = sp;
            }
        }

        return tempSpawnPoints;
    }
}
