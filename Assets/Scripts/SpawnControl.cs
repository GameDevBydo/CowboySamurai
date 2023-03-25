using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnControl : MonoBehaviour
{
    public GameObject Spawner;
    public GameObject Player;
    public int yPos;
    public int zPos;
    float distance = Vector3.Distance(GameObject.FindWithTag("Player").transform.position, (GameObject.FindWithTag("Spawner")).transform.position); 

    void Start()
    {
        Instantiate(Spawner, new Vector3(distance, yPos, zPos), Quaternion.identity);
    }
}
