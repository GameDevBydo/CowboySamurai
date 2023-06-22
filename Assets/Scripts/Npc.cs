using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Npc : MonoBehaviour
{
    public GameObject prefab;
    public float distance;

    private GameObject player;
    private float dist;

    public int npcType; // 0 para vendedor (abre uma loja), 1 para texto

    [TextArea(2,4)]
    public string[] quotes;

    public UnityEvent[] endEvent;
    
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

            if(Input.GetKeyDown(Player.instance.interactKey[0]) || Input.GetKeyDown(Player.instance.interactKey[1]))
            {
                if(npcType == 0)
                {
                    Controller.instance.shop.SetActive(true);
                    Controller.instance.PauseFullGame();
                }
                if(npcType == 1)
                {
                    Controller.instance.StartWriting(quotes, endEvent);
                }
            }
        }
        else
        {
            prefab.SetActive(false);
        }

    }
}

