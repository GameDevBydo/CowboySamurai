using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEvent : MonoBehaviour
{
    public UnityEvent Collide;
    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            Collide.Invoke();
        }
    }
}
