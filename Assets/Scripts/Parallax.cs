using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Transform startWagon,endWagon;
    private Vector3 startP,endP;
    public float parallax;
 
    void Start()
    {
        

       
    }

    float Size()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Bounds bounds = spriteRenderer.bounds;
        return bounds.size.x/2f;
    }
    void Update()
    {

        Vector3 pos = transform.position;

        startP = new Vector3(pos.x + Size(), pos.y, pos.z);
        endP = new Vector3(pos.x - Size(), pos.y, pos.z);

        transform.Translate(Time.deltaTime*-parallax, 0, 0);

        if(startP.x < endWagon.position.x )
        {
            float p = startWagon.position.x + Size();
            transform.position = new Vector3(p,transform.position.y,transform.position.z);
        }
        
    }
}
