using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class Boss : MonoBehaviour 
{
    #region Variables

    [Header ("Animator")]
    public Animator anim;

    [Header ("Boss Stats")]
    float baseLife = 500;
    float rageLife = 250;
    float normalLife = 500;

    [Header ("Boss States")]
    private bool rageMode = false;
    private bool canAttack = false;

    private SkinnedMeshRenderer rend;

    int currentAttack;
    public Attack[] attackEnemy;
    public LayerMask playerMask;
    public float recoveryTimer = 30.0f;

    public Material baseMat, hitMat, rageMat, rageScytheMat;

    public GameObject[] spikes, rageSpikes;


    private float distance;

    #endregion


    private void Awake() 
    {
        rend = transform.GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>();
        baseMat = rend.material;
        normalLife = baseLife;
        source = GetComponent<AudioSource>();
    }
    void Start()
    {  
        anim = transform.GetComponent<Animator>();
    }
    private void Update() 
    {
        recoveryTimer -= Time.deltaTime;
        if(recoveryTimer <=0)
        {
            Attack();
        }
    }

    #region Attack
    void Attack()
    {
        currentAttack = Random.Range(0,3);
        
        if(currentAttack == 0)
        {
            anim.Play("Wendigo_Attack1");
        }
        else if(currentAttack == 1)
        {
            anim.Play("Wendigo_Attack2");   
        }
        else if(currentAttack == 2)
        {
            if(rageMode) anim.Play("Wendigo_Attack3");
            else anim.Play("Wendigo_Attack2");
        }
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
        anim.Play("Wendigo_Idle");
    }

    void EnterRage()
    {
        normalLife += rageLife;
        rageMode = true;
        rend.material = rageMat;
        baseMat = rageMat;
        transform.GetChild(0).GetChild(2).GetComponent<SkinnedMeshRenderer>().material = rageScytheMat;
    }

    void CheckDeath() // Método para checagem de morte
    {
        if (normalLife <= 0)
        {
            Destroy(gameObject);
            Controller.instance.PlayEndingVideo();
            Controller.instance.ChangeScreen(Controller.instance.endGameScreen);
        }
    }

    public void TakeDamage(int damage, AudioClip sfx) // Método para ser chamado ao levar dano
    {   
        normalLife -= damage;
        GetComponent<AudioSource>().clip = sfx;
        GetComponent<AudioSource>().Play();
        StartCoroutine(HitMaterialEffect());
        CheckDeath();
        if(normalLife <= 200 && !rageMode)
        {
            anim.Play("Wendigo_Rage");
            recoveryTimer = 6;
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
    
    AudioSource source;

    public void PlaySound(AudioClip sound)
    {
        source.clip = sound;
        source.Play();
    }




}