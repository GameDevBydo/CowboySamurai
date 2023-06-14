using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    Rigidbody rb;
    MeshCollider mesh;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "ground")
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            mesh.isTrigger = true;
        }
    }
    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Player")
        {
            Controller.instance.money++;
            Destroy(gameObject);
        }

    }
}
