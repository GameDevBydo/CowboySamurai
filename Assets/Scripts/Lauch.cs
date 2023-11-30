using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lauch : MonoBehaviour
{
    public GameObject bottlePrefab; 
    public Transform player; 
    public float force; 
    public float forceY;
    public float minDelay = 1f; 
    public float maxDelay = 3f; 
    public float targetRange = 2f; 

    private float nextThrowTime;

    void Update()
    {
        if (Time.time > nextThrowTime)
        {
            ThrowBottle();
            nextThrowTime = Time.time + Random.Range(minDelay, maxDelay);
        }
    }

    void ThrowBottle()
    {
        Vector3 initialPosition = transform.position; 
        Vector3 playerPosition = player.position; 

        Vector3 targetPosition = playerPosition + Random.insideUnitSphere * targetRange;

        GameObject bottle = Instantiate(bottlePrefab, initialPosition, Quaternion.identity);

        
        Vector3 direction = (targetPosition - initialPosition).normalized;

        Rigidbody rb = bottle.GetComponent<Rigidbody>();
        rb.velocity = new Vector3(direction.x * force, forceY, 1f);
    }

   
}
