
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public static SpawnEnemy instance;
    public GameObject plane,player,prefabType1,prefabType2,prefabType3;
    private Vector3 startPlane,center,endPlane,size;


    [Header("Distância mínima do player para spawn dos inimigos")]
    [SerializeField] private float minDistanceToPlayer;

    [Header("Configs Inimigos")]
    [SerializeField] private int beginEnemies;
    [SerializeField] private int enemiesSpawn;
    public GameObject[] enemiesInScene;

    [Header("Tipos de Inimigos na Cena")] 
    [SerializeField] bool type1;
    [SerializeField] bool type2;
    [SerializeField] bool type3;
    
    private List<GameObject> typeEnemy = new List<GameObject>();
    private Vector3 lastSpawn;
    
    void Start()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");

        if(type1)
        {
            typeEnemy.Add(prefabType1);
        }
        else if(type2)
        {
            typeEnemy.Add(prefabType2);
        }
        else if(type3)
        {
            typeEnemy.Add(prefabType3);
        }
        GetSizePlane();
        BeginSpawn();

        
    }

    
    void Update()
    {
        DynamicSpawn();
    }

    

    public void GetSizePlane()
    {
        plane = GameObject.FindGameObjectWithTag("plane");

        size = Vector3.Scale(plane.GetComponent<MeshFilter>().mesh.bounds.size, plane.transform.localScale);
        center = plane.GetComponent<MeshFilter>().mesh.bounds.center;

        startPlane = center - size/2;
        endPlane = center + size/2;
    }

    public void BeginSpawn()
    {
        for (int i = 0; i < beginEnemies; i++)
        {
            Spawn();
        }
    }

    public void DynamicSpawn()
    {
        enemiesInScene = GameObject.FindGameObjectsWithTag("Enemy");

        if(enemiesInScene.Length < beginEnemies && enemiesSpawn > 0)
        {
            Spawn();
            enemiesSpawn --;
        }
    }

    public void Spawn()
    {
        float spwn;
        do
        {
            spwn = Random.Range(startPlane.x, endPlane.x);
        } while (Mathf.Abs(spwn - player.transform.position.x) < minDistanceToPlayer);

        int rndEnemy = Random.Range(0,typeEnemy.Count);
        Instantiate(typeEnemy[rndEnemy], new Vector3(spwn, 0, 0), Quaternion.identity);
        lastSpawn = new Vector3(spwn, 0, 0);
    }
        

    public void SpawnFixedLocation(Transform pos)
    {
        Instantiate(typeEnemy[0], pos.position, Quaternion.identity);
    }
    
}

