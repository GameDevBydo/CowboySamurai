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
    public float expDropped = 2.0f;
    private bool rageMode = false;
    private bool canAttack = false;
    private bool canDash = false;
    private bool getHit = false;
    int currentAttack;
    public Attack[] attackEnemy;
    public LayerMask playerMask;
    public float recoveryTimer = 5.0f;

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
                currentAttack = 0;
            }
            else
            {
                speed = baseSpeed;
                currentAttack = 0;
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

    void Attack()
    {
        currentAttack = Random.Range(1,100);
        
        if(0 < currentAttack && currentAttack < 50){
            //anim.SetBool("Punch", true);
            //anim.SetBool("Kick", false);
            //anim.SetBool("JumpKick", false);
            anim.SetTrigger("a");
            Hit(attackEnemy[0]);
        }
        if(33 <= currentAttack && currentAttack <= 50){
            //anim.SetBool("Punch", false);
            //anim.SetBool("Kick", true);
            //anim.SetBool("JumpKick", false);
            anim.SetTrigger("b");
            Hit(attackEnemy[1]);
            
        }
    }
     void Hit(Attack attack)
    {

        //Use the OverlapBox to detect if there are any other colliders within this box area.
        //Use the GameObject's centre, half the size (as a radius) and rotation. This creates an invisible box around your GameObject.
        Collider[] hitColliders = Physics.OverlapBox(new Vector3(gameObject.transform.position.x + (attack.hitboxes[0].startingPointEnemy.x* -Mathf.Sign(this.transform.rotation.eulerAngles.y-180)), gameObject.transform.position.y + attack.hitboxes[0].startingPointEnemy.y, 
        ((attack.hitboxes[0].extension.z/2.0f)+gameObject.transform.position.z)+attack.hitboxes[0].startingPointEnemy.z), attack.hitboxes[0].extension, gameObject.transform.rotation, playerMask);
        //Check when there is a new collider coming into contact with the box
        foreach(Collider col in hitColliders)
        {
            if(Player.instance.getHit)
            {
                Player.instance.TakeDamage(attack.damage);
                //
            }
        }
        
        canAttack = false;
        recoveryTimer = Mathf.Max(recoveryTimer,attack.recovery);

    }

    bool isRage (bool rageMode)
    {
        return true;
    }

    void CheckDeath() // Método para checagem de morte
    {
        if(normalLife <= 0)
        {
            Player.instance.exp += expDropped;
            //Instantiate(deathPS, transform.position + Vector3.up, deathPS.transform.rotation);
            //Instantiate(Player.instance.prefabCoin, new Vector3(transform.position.x,2f,transform.position.z), Quaternion.Euler(90f,0,0));
            Destroy(gameObject);
        }
        else if (rageLife <= 0)
        {
            Player.instance.exp += expDropped;
            //Instantiate(deathPS, transform.position + Vector3.up, deathPS.transform.rotation);
            //Instantiate(Player.instance.prefabCoin, new Vector3(transform.position.x,2f,transform.position.z), Quaternion.Euler(90f,0,0));
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage, float stun, AudioClip sfx) // Método para ser chamado ao levar dano
    {   
        if (rageMode)
        {
            rageLife -= damage;
            //Instantiate(hitPS, transform.position + Vector3.up, hitPS.transform.rotation);
            //Debug.Log("HP Atual: " + hp);
            //GetComponent<AudioSource>().clip = sfx;
            //GetComponent<AudioSource>().Play();
            //StartCoroutine(HitMaterialEffect());
            CheckDeath();
            recoveryTimer = Mathf.Max(stun, recoveryTimer);
            anim.SetTrigger("");
        }
        else if (!rageMode)
        {
            normalLife -= damage;
            //Instantiate(hitPS, transform.position + Vector3.up, hitPS.transform.rotation);
            //Debug.Log("HP Atual: " + hp);
            //GetComponent<AudioSource>().clip = sfx;
            //GetComponent<AudioSource>().Play();
            //StartCoroutine(HitMaterialEffect());
            CheckDeath();
            recoveryTimer = Mathf.Max(stun, recoveryTimer);
            anim.SetTrigger("");
        }
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