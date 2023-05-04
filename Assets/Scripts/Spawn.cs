using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public void SpawnEntity(GameObject obj)
    {
        Instantiate(obj, new Vector3(transform.position.x, 0.0f, transform.position.z), Quaternion.identity);
    }
}
