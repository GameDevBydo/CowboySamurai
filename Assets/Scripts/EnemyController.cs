using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController instance;
    GameObject player;

    public List<GameObject> leftEnemies = new List<GameObject>();
    public List<GameObject> rightEnemies = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        player = Player.instance.gameObject;
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        Teleport();
    }

    void Teleport(){
        if(leftEnemies.Count > 2){
            GameObject aux = leftEnemies.Last();
            leftEnemies.RemoveAt(leftEnemies.Count-1);
            if(!rightEnemies.Contains(aux)){
                rightEnemies.Add(aux);
                aux.transform.position = player.transform.position + new Vector3(1.5f, 0, 0);
            }
                
        }
        if(rightEnemies.Count > 2){
            GameObject aux = rightEnemies.Last();
            rightEnemies.RemoveAt(rightEnemies.Count-1);
            if(!leftEnemies.Contains(aux)){
                leftEnemies.Add(aux);
                aux.transform.position = player.transform.position - new Vector3(1.5f, 0, 0);
            }
            
        }
    }
}