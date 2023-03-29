using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public bool canSpawn;
    
    public void SpawnEntity(GameObject obj)
    {
        Instantiate(obj, transform.position, Quaternion.identity);
    }
}
