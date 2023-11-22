using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Vector3 startP,endP;
    public float parallax;
 
    void Start()
    {
       
    }


    void Update()
    {
        transform.Translate(Time.deltaTime*parallax, 0, 0);

        if(transform.position.x > endP.x+5)
        {
            transform.position = new Vector3(startP.x-5,transform.position.y,transform.position.z);
        }
        if(transform.position.x < startP.x-5)
        {
            transform.position = new Vector3(endP.x+5,transform.position.y,transform.position.z);
        }
    }
}
