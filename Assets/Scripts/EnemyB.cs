using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public bool canAttack;

    [Header("Moving")]
    public GameObject player;
    public float distance;
    public  float speed;
    public Vector3 playerPos;
    public bool canFollow;
    public float target, targetStop;
    public bool foundPlayer;

    [Header("Stats Enemy")]
    public int hp = 50;
    public float expDropped = 0.5f;
    private float range;

    public ParticleSystem deathPS, hitPS;
    public Material baseMat, hitMat;
    private SkinnedMeshRenderer rend;

    [Header("Hitstun")]
    float _timerHitstun;

    [Header("Ataque")]
    float timerAttack;
    float startUp;
    float timeoutAttack;


    public enum State{
        Idle,
        Moving,
        Attacking,
        Hitstun,
        Death
    }
    // Start is called before the first frame update
    void Awake()
    {
        player = Player.instance.gameObject;
        //rend = transform.GetComponent<SkinnedMeshRenderer>();
    }
    void Start()
    {
        m_Started = true;
        if(SceneManager.GetActiveScene().name == "TutorialScene"){
            hp = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        EnemyState();
        if(currentState != State.Death){
            recoveryTime -= Time.deltaTime;
            if(currentState != State.Hitstun)
                StartFollowPlayer();
        }
        
    }

    void EnemyState(){
        switch(currentState){
            case State.Idle:
                anim.Play("Idle");
            break;
            case State.Moving:
                FollowPlayer();
            break;
            case State.Attacking:
                ChoiceHit();
                CheckEndAnimation();
            break;
            case State.Hitstun:
                anim.Play("hitstun");
                HitStun();
            break;
            case State.Death:
                anim.Play("Death");
                DestroyEnemy();
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

    public void CreateHit(int index){
        if(canAttack){
            Collider [] hitColliders = Physics.OverlapBox(new Vector3(gameObject.transform.position.x + (attackEnemy[index].hitboxes[0].startingPointEnemy.x* -Mathf.Sign(this.transform.rotation.eulerAngles.y-180)), gameObject.transform.position.y + attackEnemy[index].hitboxes[0].startingPointEnemy.y, 
            ((attackEnemy[index].hitboxes[0].extension.z/2.0f)+gameObject.transform.position.z)+attackEnemy[index].hitboxes[0].startingPointEnemy.z), attackEnemy[index].hitboxes[0].extension, Quaternion.identity, m_LayerMask);
            foreach(Collider col in hitColliders)
            {
                if(Player.instance.getHit)
                {
                    Player.instance.TakeDamage(attackEnemy[index].damage);
                }
            }
            canAttack = false;
        }
    }

    public void CheckEndAnimation(){
        if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f){
            ChangeState(State.Idle);
        }
    }

    public void ChoiceHit(){
        if(pickHit < 0 && pickHit < 33){
            anim.Play("Hit1");
            recoveryTime = attackEnemy[0].recovery;
        }else if(pickHit >= 33 && pickHit <= 66){
            anim.Play("Hit2");
            recoveryTime = attackEnemy[1].recovery;
        }else if(pickHit>66){
            anim.Play("Hit3");
            recoveryTime = attackEnemy[2].recovery;
        }
    }

    void OnDrawGizmos()
    {
        if(canAttack){
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
        distance = Vector3.Distance(transform.position, player.transform.position);
        if(distance<=target && canFollow){
            ChangeState(State.Moving);
        }
        
        
        if(distance<=targetStop && distance>=-targetStop){
            canFollow = false;
            if(transform.position.x < player.transform.position.x)
                if(!EnemyController.instance.leftEnemies.Contains(gameObject))
                    EnemyController.instance.leftEnemies.Add(gameObject);

            if(transform.position.x > player.transform.position.x)
                if(!EnemyController.instance.rightEnemies.Contains(gameObject))
                    EnemyController.instance.rightEnemies.Add(gameObject);

            if(recoveryTime<=0){
                ChangeState(State.Attacking);
                pickHit = Random.Range(0, 100);
                canAttack = true;
            }
        }else{
            AttPositionList();
            canFollow = true;
        }
    }

    void AttPositionList(){
        if(EnemyController.instance.rightEnemies.Contains(gameObject)){
            EnemyController.instance.rightEnemies.Remove(gameObject);
        }
    
        if(EnemyController.instance.leftEnemies.Contains(gameObject)){
            EnemyController.instance.leftEnemies.Remove(gameObject);
        }
    }

    void CheckDeath(){
        if(hp <= 0)
        {
            
            Player.instance.exp += expDropped;
            Controller.instance.enemiesDefeated++;
            Controller.instance.CheckClearCondition();
            if(transform.position.x < player.transform.position.x) 
                EnemyController.instance.leftEnemies.Remove(gameObject);
            if(transform.position.x > player.transform.position.x)
                EnemyController.instance.rightEnemies.Remove(gameObject);
            Instantiate(deathPS, transform.position + Vector3.up, deathPS.transform.rotation);
            Instantiate(Player.instance.prefabCoin, new Vector3(transform.position.x,2f,transform.position.z), Quaternion.Euler(90f,0,0));
            ChangeState(State.Death);
            
        }
    }
    void DestroyEnemy(){
        if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime>=1.0f)
            Destroy(gameObject);
    }


    public void TakeDamage(int damage, float knockback, AudioClip sfx){
        ChangeState(State.Hitstun);
        hp-=damage;
        Instantiate(hitPS, transform.position + Vector3.up, hitPS.transform.rotation);
        //Debug.Log("HP Atual: " + hp);
        GetComponent<AudioSource>().clip = sfx;
        GetComponent<AudioSource>().Play();
        //StartCoroutine(HitMaterialEffect());
        CheckDeath();
    }

    public IEnumerator HitMaterialEffect()
    {
        rend.material = hitMat;
        yield return new WaitForSeconds(0.1f);
        rend.material = baseMat;
    }

    void HitStun(){
        GetComponent<CharacterController>().enabled = false;
        if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime>1.0f){
            GetComponent<CharacterController>().enabled = true;
            ChangeState(State.Idle);
        }
    }
}
