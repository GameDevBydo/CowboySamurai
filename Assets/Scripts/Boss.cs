using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
public class Boss : MonoBehaviour 
{
    #region variables
    public CharacterController controller;
    private Animator anim;

    float baseLife = 250;
    float rageLife = 150;
    float normalLife = 100;
    private bool rageMode = false;
    private bool canAttack = false;
    private bool canDash = false;
    private bool getHit = false;

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
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }
    private void FixedUpdate() 
    {
        if (distance >= 10f && canDash)StartCoroutine(DashCD());
        if (currentState == BossState.DASHING) controller.Move( transform.forward * Time.fixedDeltaTime * 10);

    }
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

    #region StateMachine
enum BossState
    {
        CINEMATIC,
        STANDING,
        WALKING,
        ATTACKING,
        HITSTUN,
        DASHING
    }

    BossState currentState, pastState;
    string animationName;
    public void ChangeBossState(int id)
    {
        pastState = currentState;
        if(currentState != pastState) return;

        switch(id)
        {
            case 0:
                currentState = BossState.CINEMATIC;
            break;
            case 1:
                currentState = BossState.STANDING;
            break;
            case 2:
                currentState = BossState.WALKING;
            break;
            case 3:
                currentState = BossState.ATTACKING;
            break;
            case 4:
                currentState = BossState.HITSTUN;
            break;
            case 5:
                currentState = BossState.DASHING;
            break;
            default:
                //Debug.Log("Change to state 1");
                currentState = BossState.STANDING;
            break;
        }
        anim.Play(animationName);
        
    }
    #endregion
    
    #region IEnumerator
    IEnumerator DashCD()
        {
            canDash = false;
            getHit = false;
            //rend.material = dashMat;
            ChangeBossState(5);
            gameObject.layer = LayerMask.NameToLayer("Dash");
            yield return new WaitForSeconds(0.35f);
            ChangeBossState(1);
            gameObject.layer = LayerMask.NameToLayer("Boss");
            getHit = true;
            canDash = true;
            //rend.material = baseMat;
        }
    
    #endregion
}