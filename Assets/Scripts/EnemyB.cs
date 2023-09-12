using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyB : MonoBehaviour
{
    [Header("State")]
    public State currentState;

    [Header("Animator")]
    public Animator anim;

    [Header("Overlapbox")]
    public LayerMask m_LayerMask;
    bool m_Started;
    public int pickHit;
    public float recoveryTime = 5.0f;
    public Attack[] attackEnemy;

    [Header("Moving")]
    public GameObject player;
    public  float speed;
    public Vector3 playerPos;
    public bool canFollow;
    public float target, targetStop;

    [Header("Stats Enemy")]
    public int hp = 50;
    public float expDropped = 0.5f;
    private float range;

    public ParticleSystem deathPS, hitPS;
    public Material baseMat, hitMat;
    private SkinnedMeshRenderer rend;

    public enum State{
        Idle,
        Moving,
        Attacking,
        Hitstun
    }
    // Start is called before the first frame update
    void Awake()
    {
        player = Player.instance.gameObject;
    }
    void Start()
    {
        m_Started = true;
    }

    // Update is called once per frame
    void Update()
    {
        EnemyState();
        recoveryTime -= Time.deltaTime;
        StartFollowPlayer();
        /*if(canAttack){
            ChangeState(State.Attacking);
            pickHit = Random.Range(0, 100);
            recoveryTime = 5.0f;
        }*/
    }

    void EnemyState(){
        switch(currentState){
            case State.Idle:
                //Debug.Log("Idle");
                anim.Play("Idle");
            break;
            case State.Moving:
                FollowPlayer();
            break;
            case State.Attacking:
                //Debug.Log("Attacking");
                ChoiceHit();
                CheckEndAnimation();
            break;
            case State.Hitstun:
                anim.Play("Hitstun");
            break;
            default:
                ChangeState(State.Idle);
                anim.Play("idle");
            break;
        }
    }

    public void ChangeState(State newState){
        currentState = newState;
    }

    public void CreateHit(string nameAnimation, Attack attack){
        Collider [] hitColliders = Physics.OverlapBox(new Vector3(gameObject.transform.position.x + (attack.hitboxes[0].startingPointEnemy.x* -Mathf.Sign(this.transform.rotation.eulerAngles.y-180)), gameObject.transform.position.y + attack.hitboxes[0].startingPointEnemy.y, 
        ((attack.hitboxes[0].extension.z/2.0f)+gameObject.transform.position.z)+attack.hitboxes[0].startingPointEnemy.z), attack.hitboxes[0].extension, Quaternion.identity, m_LayerMask);
        int i = 0;
        while(i<hitColliders.Length){
            i++;
        }
        anim.Play(nameAnimation);
    }

    public void CheckEndAnimation(){
        if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f){
            ChangeState(State.Idle);
        }
    }

    public void ChoiceHit(){
        if(pickHit < 0 && pickHit < 33){
            CreateHit("Hit1", attackEnemy[0]);
        }else if(pickHit >= 33 && pickHit <= 66){
            CreateHit("Hit2", attackEnemy[1]);
        }else if(pickHit>66){
            CreateHit("Hit3", attackEnemy[2]);
        }
    }

    void OnDrawGizmos()
    {
        if(pickHit < 0 && pickHit < 33){
            Gizmos.color = Color.red;        
            if (m_Started)
                //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
                Gizmos.DrawWireCube(new Vector3(gameObject.transform.position.x + (attackEnemy[0].hitboxes[0].startingPointEnemy.x* -Mathf.Sign(this.transform.rotation.eulerAngles.y-180)), gameObject.transform.position.y + attackEnemy[0].hitboxes[0].startingPointEnemy.y, 
        ((attackEnemy[0].hitboxes[0].extension.z/2.0f)+gameObject.transform.position.z)+attackEnemy[0].hitboxes[0].startingPointEnemy.z), attackEnemy[0].hitboxes[0].extension);
        }else if(pickHit >= 33 && pickHit <= 66){
            Gizmos.color = Color.blue;        
            if (m_Started)
                //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
                Gizmos.DrawWireCube(new Vector3(gameObject.transform.position.x + (attackEnemy[1].hitboxes[0].startingPointEnemy.x* -Mathf.Sign(this.transform.rotation.eulerAngles.y-180)), gameObject.transform.position.y + attackEnemy[1].hitboxes[0].startingPointEnemy.y, 
        ((attackEnemy[1].hitboxes[0].extension.z/2.0f)+gameObject.transform.position.z)+attackEnemy[1].hitboxes[0].startingPointEnemy.z), attackEnemy[1].hitboxes[0].extension);
        }else if(pickHit>66){
            Gizmos.color = Color.green;        
            if (m_Started)
                //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
                Gizmos.DrawWireCube(new Vector3(gameObject.transform.position.x + (attackEnemy[2].hitboxes[0].startingPointEnemy.x* -Mathf.Sign(this.transform.rotation.eulerAngles.y-180)), gameObject.transform.position.y + attackEnemy[2].hitboxes[0].startingPointEnemy.y, 
        ((attackEnemy[2].hitboxes[0].extension.z/2.0f)+gameObject.transform.position.z)+attackEnemy[2].hitboxes[0].startingPointEnemy.z), attackEnemy[2].hitboxes[0].extension);
        }
    }

    void FollowPlayer() 
    {
        playerPos = new Vector2(player.transform.position.x, 0);
        transform.position = Vector2.MoveTowards(this.transform.position, playerPos, speed * Time.deltaTime);

        anim.Play("Walking");

        if(player.transform.position.x < gameObject.transform.position.x){
            transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
        }
        if(player.transform.position.x > gameObject.transform.position.x){
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
        }
    }

    void StartFollowPlayer(){
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if(distance<=target && canFollow){
            ChangeState(State.Moving);
        }
        if(distance<=targetStop && distance>=-targetStop){
            canFollow = false;
            if(recoveryTime<=0){
                ChangeState(State.Attacking);
                pickHit = Random.Range(0, 100);
                recoveryTime = 2.0f;
            }
        }else{
            canFollow = true;
        }
    }

    void CheckDeath(){
        if(hp <= 0)
        {
            Player.instance.exp += expDropped;
            Instantiate(deathPS, transform.position + Vector3.up, deathPS.transform.rotation);
            Instantiate(Player.instance.prefabCoin, new Vector3(transform.position.x,2f,transform.position.z), Quaternion.Euler(90f,0,0));
            Destroy(gameObject);
        }
    }

    void TakeDamage(int damage, float stun, AudioClip sfx){
        hp-=damage;
        Instantiate(hitPS, transform.position + Vector3.up, hitPS.transform.rotation);
        //Debug.Log("HP Atual: " + hp);
        GetComponent<AudioSource>().clip = sfx;
        GetComponent<AudioSource>().Play();
        StartCoroutine(HitMaterialEffect());
        CheckDeath();
    }

    public IEnumerator HitMaterialEffect()
    {
        rend.material = hitMat;
        yield return new WaitForSeconds(0.1f);
        rend.material = baseMat;
    }

}
