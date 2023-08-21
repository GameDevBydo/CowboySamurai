using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour 
{
    #region variables

    float baseLife = 250;
    float rageLife = 150;
    float normalLife = 100;
    private bool rageMode = false;
    private bool canAttack = false;

    public GameObject bossPrefab;
    public GameObject player;

    public float speed;
    public float baseSpeed = 5.0f;
    public float rageSpeed = 7.0f;
    private float distance;

    #endregion

    #region Move
    //TODO: 
    //1: Dash(?)
    void Follow()
    {
        distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 dir = player.transform.position - transform.position;
        Vector2 playerPos = new Vector2(player.transform.position.x, 0);

        transform.position = Vector2.MoveTowards(this.transform.position, playerPos, speed * Time.deltaTime);
        
        if (distance <= 1.5f)
        {
            speed = 0;
            if (canAttack)
            {
                Attack();
            }
        }
        else
        {
            if (rageMode)
            {
                speed = rageSpeed;
            }
            else
            {
                speed = baseSpeed;
            }
        }
        if(player.transform.position.x < gameObject.transform.position.x)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
        }
        if(player.transform.position.x > gameObject.transform.position.x)
        {
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
        }
    }

    #endregion

    #region Atack

    int Attack()
    {
        int damage = 0;
        //cria um collider(?) em sua frente para o ataque 
        return damage;
    }

    bool isRage (bool rageMode)
    {
        return true;
    }
    #endregion
}