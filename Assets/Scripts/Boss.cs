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
    private MeshRenderer rend;

    int currentAttack;
    public Attack[] attackEnemy;
    public LayerMask playerMask;
    public float recoveryTimer = 30.0f;

    public ParticleSystem deathPS, hitPS;
    public Material baseMat, hitMat;
    [HideInInspector]public GameObject player;

    public GameObject[] spikes;


    private float distance;

    #endregion


    private void Awake() 
    {
        //player = GameObject.FindGameObjectWithTag("Player");                     VOLTA AQUI TB
        rend = transform.GetChild(0).GetComponent<MeshRenderer>();
        baseMat = rend.material;
        normalLife = baseLife;
    }
    void Start()
    {
        m_Started = true;  
        //anim = GetComponent<Animator>();
        SpikesAttack();
    }
    private void Update() 
    {
        recoveryTimer -= Time.deltaTime;
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
        currentAttack = Random.Range(1,200);
        
        if(0 < currentAttack && currentAttack < 50)
        {
            ChangeBossState(2);
            //WaitSecs();
            Debug.Log("1");
            recoveryTimer = attackEnemy[0].recovery;
        }
        CheckEndAnimation();
    }

    void SpikesAttack()
    {
        StartCoroutine(CallSpikes());
    }

    IEnumerator CallSpikes()
    {
        foreach(GameObject spike in spikes)
        {
            spike.SetActive(true);
            Debug.Log(spike.transform.GetChild(0).GetComponent<Animation>().clip);
            spike.transform.GetChild(0).GetComponent<Animation>().Play();
            yield return new WaitForSeconds(1);
        }
    }

    public void CheckEndAnimation()
    {
        if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
        {
            ChangeBossState(1);
        }
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
    
}