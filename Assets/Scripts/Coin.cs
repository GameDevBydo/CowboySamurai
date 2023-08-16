using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{   
    int coinValue;
    private float gravity = -3.5f;
    
    void Start()
    {
        coinValue = Random.Range(1, 5);
    }
    
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
            Controller.instance.money+=coinValue;
            Destroy(gameObject);
        }
        if(other.tag == "ground")
        {
            gravity = 0;
        }
    }
}
