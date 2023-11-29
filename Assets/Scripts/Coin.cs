using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Coin : MonoBehaviour
{   
    public AudioClip coinCollect;
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
            Controller.instance.audio.PlayEffect(coinCollect);
            Destroy(gameObject);
        }
        if(other.tag == "plane")
        {
            gravity = 0;
        }
    }
}
