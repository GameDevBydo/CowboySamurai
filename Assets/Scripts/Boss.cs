using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : MonoBehaviour 
{
    #region Variables

    [Header ("Animator")]
    public Animator anim;

    [Header ("Boss Stats")]
    float baseLife = 250;
    float rageLife = 100;
    float normalLife = 100;

    [Header ("Boss States")]
    private bool rageMode = false;
    private bool canAttack = false;

    bool m_Started;
    private SkinnedMeshRenderer rend;

    int currentAttack;
    public Attack[] attackEnemy;
    public LayerMask playerMask;
    public float recoveryTimer = 30.0f;

    public ParticleSystem deathPS, hitPS;
    public Material baseMat, hitMat;
    [HideInInspector]public GameObject player;

    public GameObject[] spikes, rageSpikes;


    private float distance;

    #endregion


    private void Awake() 
    {
        //player = GameObject.FindGameObjectWithTag("Player");                     VOLTA AQUI TB
        rend = transform.GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>();
        baseMat = rend.material;
        normalLife = baseLife;
    }
    void Start()
    {
        m_Started = true;  
        anim = transform.GetComponent<Animator>();
    }
    private void Update() 
    {
        recoveryTimer -= Time.deltaTime;
        if(recoveryTimer <=0)
        {
            Attack();
        }

        CheckDistance();
    }

    #region Move
    void CheckDistance()
    {   
        //distance = Vector3.Distance(transform.position, player.transform.position);
        //if(recoveryTimer <= 0)
        //{
        //    canAttack = true;
        //}                                                                                                         VOLTA ISSO TUDO
        //if (distance <= 10.0f && canAttack)
        //{
        //    Attack();
        //}
    }

    #endregion

    #region Attack

    void Attack()
    {
        currentAttack = Random.Range(0,1);
        
        if(currentAttack == 0)
        {
            Debug.Log("0");
            anim.Play("Wendigo_Attack1");
            
        }
        else if(currentAttack == 1)
        {
            Debug.Log("1");
            anim.Play("Wendigo_Attack2");
            
        }
        else Debug.Log("trouxa");
        recoveryTimer = 7;
    }

    void SpikesAttack()
    {
        StartCoroutine(CallSpikes());
    }

    IEnumerator CallSpikes()
    {
        if(rageMode)
        {
            foreach(GameObject spike in rageSpikes)
            {
                spike.SetActive(true);
                spike.transform.GetChild(0).GetComponent<Animation>().Play();
                yield return new WaitForSeconds(0.2f);
            }
        }
        else
        foreach(GameObject spike in spikes)
        {
            spike.SetActive(true);
            spike.transform.GetChild(0).GetComponent<Animation>().Play();
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void ReturnToIdle()
    {
        anim.Play("Armature|Idle");
    }

    void EnterRage()
    {
        //play rage anim, change shader
        normalLife += rageLife;
        rageMode = true;
    }

    void CheckDeath() // Método para checagem de morte
    {
        if (normalLife <= 0)
        {
            Destroy(gameObject);
            // play video of ending
        }
    }

    public void TakeDamage(int damage, float stun, AudioClip sfx) // Método para ser chamado ao levar dano
    {   
        normalLife -= damage;
        //Instantiate(hitPS, transform.position + Vector3.up, hitPS.transform.rotation);
        GetComponent<AudioSource>().clip = sfx;
        GetComponent<AudioSource>().Play();
        StartCoroutine(HitMaterialEffect());
        CheckDeath();
        recoveryTimer = Mathf.Max(stun, recoveryTimer);
        if(normalLife <= 50) EnterRage();
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
    
    public void ShowBox(int id)
    {
        transform.GetChild(id).gameObject.SetActive(true);
    }

    public void HideBox(int id)
    {
        transform.GetChild(id).gameObject.SetActive(false);
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
            break;
            default:
                currentState = BossState.STANDING;
            break;
        }
        anim.Play(animationName);
    }
    #endregion
    

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Player")
        {
            if(Player.instance.getHit)
            {
                Player.instance.TakeDamage(50);
            }
        }
    }
}