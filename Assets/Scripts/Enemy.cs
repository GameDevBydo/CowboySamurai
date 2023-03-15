using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameObject player;
    #region Movement Variables
    [HideInInspector]
    public float speed = 5.0f;
    public float baseSpeed = 5.0f;
    private float distance;
    [SerializeField]
    private bool canFollow = false;
    #endregion

    #region Combat Variables
    private int hp = 50;
    #endregion
    void Awake()
    {
        player = Player.instance.gameObject; //Define o player 
    }

    void Update()
    {
        if(canFollow) FollowPlayer();
    }

    #region Movement 
    void FollowPlayer() //Segue o player no eixo X, até chegar a uma distancia minima
    {
        distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = player.transform.position - transform.position;
        Vector2 playerPos = new Vector2(player.transform.position.x, 0);

        transform.position = Vector2.MoveTowards(this.transform.position, playerPos, speed * Time.deltaTime);
        if(distance <= 2)
        {
            speed = 0;
        }
        else
        {
            speed = baseSpeed;
        }
    }
    #endregion



    public void TakeDamage(int damage) // Método para ser chamado ao levar dano
    {
        hp-=damage;
        Debug.Log("HP Atual: " + hp);
        CheckDeath();
    }

    void CheckDeath() // Método para checagem de morte
    {
        if(hp <= 0)
        {
            Debug.Log(gameObject.name + "morreu");
            Destroy(gameObject);
        }
    }
}
