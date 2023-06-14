using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{   
    private float gravity = -2f;
    void Update() 
    {
        Vector3 pos = transform.position;
        pos.y += gravity * Time.deltaTime;
        transform.position = pos;
    }
    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Player")
        {
            Controller.instance.money++;
            Destroy(gameObject);
        }
        if(other.tag == "ground")
        {
            gravity = 0;
        }


    }
}
