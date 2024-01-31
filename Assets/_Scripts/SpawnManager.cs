using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    PlayerSpawn[] spawnPoints;
    public static SpawnManager Instance;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        spawnPoints = FindObjectsOfType<PlayerSpawn>();
    }

    public void RespawnPlayer(Transform obj)
    {
        //when calling this with the player transform it will relocate the player to the furthest spawnpoint
        float dist = 0;
        int index = 0;
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            float tempdist = Vector3.Distance(obj.position, spawnPoints[i].transform.position);
            if (tempdist > dist)
            {
                dist = tempdist;
                index = i;
            }
        }
        obj.position = spawnPoints[index].transform.position;
    }
}
