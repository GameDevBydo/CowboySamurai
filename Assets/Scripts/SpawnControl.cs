using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnControl : MonoBehaviour
{
    public Spawn[] spawns;
    public GameObject[] entities;
    bool spawnTimer;
    

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            StartSpawnEntities(0,3,1);
        }
    }


    public void StartSpawnEntities(int entityId, int quantity, int spawnId)
    {
        for(int i = 0; i < quantity; i++)
        {
            spawns[spawnId].SpawnEntity(entities[entityId]);
        }
    }
}
