using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : MonoBehaviour 
{
    #region variables
    [Header ("Character Controller")]
    private CharacterController controller;

    [Header ("Animator")]
    public Animator anim;

    [Header ("Boss Stats")]
    float baseLife = 250;
    float rageLife = 150;
    float normalLife = 100;

    [Header ("Boss States")]
    private bool rageMode = false;
    private bool canAttack = false;

    [Header ("Object Pool")]
    public GameObject projectilePrefab;
    public List<Projectile> projectilePool = new List<Projectile>();
    public int poolSize = 6; // number os objs in scene at the same time


    bool m_Started;
    private SkinnedMeshRenderer rend;

    [Header ("Projectile")]
    float limitX = 50.0f;
    float limitY = 50.0f;
    float projectileTimer = 100.0f;

    int currentAttack;
    public Attack[] attackEnemy;
    public LayerMask playerMask;
    public float recoveryTimer = 30.0f;

    public ParticleSystem deathPS, hitPS;
    public Material baseMat, hitMat;

    public GameObject player;


    private float distance;

    #endregion

    #region Move

    private void Awake() 
    {
        projectilePrefab = GameObject.FindGameObjectWithTag("Projectile");
        player = GameObject.FindGameObjectWithTag("Player");

        rend = transform.GetChild(1).GetChild(1).GetComponent<SkinnedMeshRenderer>();
    }
    void Start()
    {
        m_Started = true;  
        controller = GetComponent<CharacterController>();
        //anim = GetComponent<Animator>();
    }
    private void Update() 
    {
        recoveryTimer -= Time.deltaTime;
        CheckDistance();
    }
    private void FixedUpdate() 
    {
    
    }
    void CheckDistance()
    {   
        distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= 10.0f)
        {
            if (recoveryTimer <= 0)
            {
                canAttack = true;
            }
            
            if (canAttack)
            {
                Attack();
            }
        }
        else
        {
            //do rage
        }
        /* later use if boss is not at  the end of the train
        if(player.transform.position.x < gameObject.transform.position.x)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
        }
        if(player.transform.position.x > gameObject.transform.position.x)
        {
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
        }
        */
    }

    #endregion

    #region Atack

    void Attack()
    {
        currentAttack = Random.Range(1,200);
        
        if(0 < currentAttack && currentAttack < 50)
        {
            ChangeBossState(2);
            WaitSecs();
            Hit(attackEnemy[0]);
            Debug.Log("1");
            recoveryTimer = attackEnemy[0].recovery;
        }
        else if(49 < currentAttack && currentAttack < 100)
        {
            ChangeBossState(2);
            WaitSecs();
            Hit(attackEnemy[1]);
            Debug.Log("2");
            recoveryTimer = attackEnemy[1].recovery;
        }
        else if(99 < currentAttack && currentAttack < 150)
        {
            ChangeBossState(2);
            WaitSecs();
            Hit(attackEnemy[2]);
            Debug.Log("3");
            recoveryTimer = attackEnemy[2].recovery;
        }
        else if (149 < currentAttack && currentAttack < 200)
        {
            if (canAttack)
            {
        
            }
        }
        CheckEndAnimation();
    }


     void Hit(Attack attack)
    {
        if (canAttack)
        {
            Collider[] hitColliders = Physics.OverlapBox(new Vector3(gameObject.transform.position.x + (attack.hitboxes[0].startingPointEnemy.x* -Mathf.Sign(this.transform.rotation.eulerAngles.y-180)), gameObject.transform.position.y + attack.hitboxes[0].startingPointEnemy.y, 
            ((attack.hitboxes[0].extension.z/2.0f)+gameObject.transform.position.z)+attack.hitboxes[0].startingPointEnemy.z), attack.hitboxes[0].extension, gameObject.transform.rotation, playerMask);
            //Check when there is a new collider coming into contact with the box
            foreach(Collider col in hitColliders)
            {
                if(Player.instance.getHit)
                {
                    Player.instance.TakeDamage(attack.damage);
                }
            }  
            canAttack = false;
            recoveryTimer = Mathf.Max(recoveryTimer,attack.recovery);
        }
    }
    
    void OnDrawGizmos()
    {
        if(canAttack)
        {
            Gizmos.color = Color.red;        
            if (m_Started)
            {
                if (0 < currentAttack && currentAttack < 50)
                {
                    //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
                Gizmos.DrawWireCube(new Vector3(gameObject.transform.position.x + (attackEnemy[0].hitboxes[0].startingPointEnemy.x* -Mathf.Sign(this.transform.rotation.eulerAngles.y-180)), gameObject.transform.position.y + attackEnemy[0].hitboxes[0].startingPointEnemy.y, 
                ((attackEnemy[0].hitboxes[0].extension.z/2.0f)+gameObject.transform.position.z)+attackEnemy[0].hitboxes[0].startingPointEnemy.z), attackEnemy[0].hitboxes[0].extension);
                }
                else if (49 < currentAttack && currentAttack < 100)
                {
                    //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
                Gizmos.DrawWireCube(new Vector3(gameObject.transform.position.x + (attackEnemy[1].hitboxes[0].startingPointEnemy.x* -Mathf.Sign(this.transform.rotation.eulerAngles.y-180)), gameObject.transform.position.y + attackEnemy[1].hitboxes[0].startingPointEnemy.y, 
                ((attackEnemy[1].hitboxes[0].extension.z/2.0f)+gameObject.transform.position.z)+attackEnemy[1].hitboxes[0].startingPointEnemy.z), attackEnemy[1].hitboxes[0].extension);
                }
                else if (99 < currentAttack && currentAttack < 150)
                {
                    //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
                Gizmos.DrawWireCube(new Vector3(gameObject.transform.position.x + (attackEnemy[2].hitboxes[0].startingPointEnemy.x* -Mathf.Sign(this.transform.rotation.eulerAngles.y-180)), gameObject.transform.position.y + attackEnemy[2].hitboxes[0].startingPointEnemy.y, 
                ((attackEnemy[2].hitboxes[0].extension.z/2.0f)+gameObject.transform.position.z)+attackEnemy[2].hitboxes[0].startingPointEnemy.z), attackEnemy[2].hitboxes[0].extension);
                }
            }
        }

    }
    public void CheckEndAnimation()
    {
        if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
        {
            ChangeBossState(1);
        }
    }
    

    bool isRage (bool rageMode)
    {
        if (normalLife <= 0)
        {
            baseLife = rageLife;
            rageMode = true;
            return true;
        }
        else return false;
    }

    void CheckDeath() // Método para checagem de morte
    {
        if (baseLife <= 0)
        {
            Destroy(gameObject);
        }
        else if (rageLife <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage, float stun, AudioClip sfx) // Método para ser chamado ao levar dano
    {   
        if (rageMode)
        {
            rageLife -= damage;
            //Instantiate(hitPS, transform.position + Vector3.up, hitPS.transform.rotation);
            GetComponent<AudioSource>().clip = sfx;
            GetComponent<AudioSource>().Play();
            StartCoroutine(HitMaterialEffect());
            CheckDeath();
            recoveryTimer = Mathf.Max(stun, recoveryTimer);
        }
        else if (!rageMode)
        {
            normalLife -= damage;
            //Instantiate(hitPS, transform.position + Vector3.up, hitPS.transform.rotation);
            GetComponent<AudioSource>().clip = sfx;
            GetComponent<AudioSource>().Play();
            StartCoroutine(HitMaterialEffect());
            CheckDeath();
            recoveryTimer = Mathf.Max(stun, recoveryTimer);
        }
    }
    public IEnumerator HitMaterialEffect()
    {
        rend.material = hitMat;
        yield return new WaitForSeconds(0.1f);
        rend.material = baseMat;
    }
    #endregion

    #region StateMachine
enum BossState
    {
        CINEMATIC,
        STANDING,
        ATTACKING
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
                animationName = "Idle";
            break;
            case 2:
                currentState = BossState.ATTACKING;
                animationName = "Hit1";
                //CheckEndAnimation();
            break;
            default:
                //Debug.Log("Change to state 1");
                currentState = BossState.STANDING;
            break;
        }
        anim.Play(animationName);
    }
    #endregion
    public IEnumerator WaitSecs()
    {
        yield return new WaitForSeconds(1.0f);
    }
}