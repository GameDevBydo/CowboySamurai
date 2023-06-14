using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{
    public GameObject prefab;
    public float distance;

    private GameObject player;
    private float dist;
    
    // Start is called before the first frame update
    void Start()
    {
       player = Player.instance.gameObject; 
    }

    // Update is called once per frame
    void Update()
    {
        dist = Vector3.Distance(transform.position, Player.instance.transform.position);
        if(dist <= distance)
        {
            Vector3 transformCanva = new Vector3(transform.position.x,transform.position.y +1.5f, transform.position.z);
            prefab.transform.position = transformCanva;
            prefab.SetActive(true);

            if(Input.GetKeyUp(KeyCode.E))
            {
                Controller.instance.shop.SetActive(true);
                Controller.instance.PauseFullGame();
            }
        }
        else
        {
            prefab.SetActive(false);
        }

    }
}

