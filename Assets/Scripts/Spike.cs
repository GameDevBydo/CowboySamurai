using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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

    public void PlaySound()
    {
        GetComponent<AudioSource>().Play();
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
