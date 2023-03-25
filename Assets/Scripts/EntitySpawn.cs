using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawn : MonoBehaviour
{
    public GameObject Enemy;
    public int yPos;
    public int zPos;
    public int enemyCount;

    void Start()
    {
        StartCoroutine(EnemySpawn());
    }

    IEnumerator EnemySpawn()
    {
        for(int i = 0; i < enemyCount; i++)
        {
        Instantiate(Enemy, new Vector3(1,yPos,zPos), Quaternion.identity);
        yield return new WaitForSeconds(0.01f);
        enemyCount += 1;
        }
    }
}
