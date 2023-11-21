using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public void ShowBox()
    {
        transform.GetComponent<BoxCollider>().enabled = true;
    }

    public void HideBox()
    {
        transform.GetComponent<BoxCollider>().enabled = false;
    }

    public void GoAway()
    {
        transform.parent.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Player")
        {
            if(Player.instance.getHit)
            {
                Player.instance.TakeDamage(30);
            }
        }
    }
}
