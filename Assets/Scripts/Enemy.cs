using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    public Animator anim;
    private GameObject player;
    #region Movement Variables
    [HideInInspector]
    public float speed;
    public float baseSpeed = 5.0f;
    private float distance;
    [SerializeField]
    private bool canFollow = false;
    #endregion

    #region Combat Variables
    public int hp = 50;
    private float range;
    #endregion

    public ParticleSystem deathPS, hitPS;
    public Material baseMat, hitMat;
    private SkinnedMeshRenderer rend;

    bool m_Started;
    public LayerMask playerMask;
    public Attack[] attackEnemy;
    int currentAttack;

    public static Enemy instance;

    public float recoveryTimer = 5.0f;
    public bool canAttack;
    
    void Awake()
    {
        player = Player.instance.gameObject; //Define o player 
        speed = baseSpeed;
        range = Random.Range( 1.5f, 2.5f);
        rend = transform.GetChild(1).GetChild(1).GetComponent<SkinnedMeshRenderer>();
        m_Started = true;
        instance = this;
        //anim = GetComponent<Animator>();
    }

    void Update()
    {
        if(!Controller.instance.inputPause)
        {
            if(!Controller.instance.playerPause)
            {
                if(canFollow) FollowPlayer();
            }else{
                anim.SetBool("Walking",false);
            }
        }else{
            anim.SetBool("Walking",false);
        }
    }
    void AttackPlayer(){
        currentAttack = Random.Range(1,100);
        
        if(0 < currentAttack && currentAttack < 33){
            //anim.SetBool("Punch", true);
            //anim.SetBool("Kick", false);
            //anim.SetBool("JumpKick", false);
            anim.SetTrigger("Punch");
            Golpe(attackEnemy[0]);
  

            
        }
        if(33 <= currentAttack && currentAttack <= 66){
            //anim.SetBool("Punch", false);
            //anim.SetBool("Kick", true);
            //anim.SetBool("JumpKick", false);
            anim.SetTrigger("Kick");
            Golpe(attackEnemy[1]);
            

            
        }
        if(currentAttack > 66){
            //anim.SetBool("Punch", false);
            //anim.SetBool("Kick", false);
            //anim.SetBool("JumpKick", true);
            anim.SetTrigger("Jumpkick");
            Golpe(attackEnemy[2]);
            
            
        }
    }

    void Timeout(){
        if(recoveryTimer>0 && !canAttack)
        {
            recoveryTimer -= Time.fixedDeltaTime;
        }
        else
        {
            canAttack = true;  
        }
    }

    #region Movement 
    void FollowPlayer() //Segue o player no eixo X, até chegar a uma distancia minima
    {
        distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = player.transform.position - transform.position;
        Vector2 playerPos = new Vector2(player.transform.position.x, 0);

        transform.position = Vector2.MoveTowards(this.transform.position, playerPos, speed * Time.deltaTime);
        if(speed > 0){
            anim.SetBool("Walking",true);        
        }
        if(speed <= 0){
            anim.SetBool("Walking",false);
        }
        if(distance <= range)
        {
            speed = 0;
            Timeout();
            if(canAttack)
                AttackPlayer();
        }
        else
        {
            speed = baseSpeed;
            currentAttack = 0;
        }
        if(player.transform.position.x < gameObject.transform.position.x){
            transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
        }
        if(player.transform.position.x > gameObject.transform.position.x){
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
        }
    }
    #endregion


    #region Combat
    public void TakeDamage(int damage, float stun, AudioClip sfx) // Método para ser chamado ao levar dano
    {
        hp-=damage;
        Instantiate(hitPS, transform.position + Vector3.up, hitPS.transform.rotation);
        //Debug.Log("HP Atual: " + hp);
        GetComponent<AudioSource>().clip = sfx;
        GetComponent<AudioSource>().Play();
        StartCoroutine(HitMaterialEffect());
        CheckDeath();
        recoveryTimer = stun;
        anim.SetTrigger("Stunned");
    }

    public IEnumerator HitMaterialEffect()
    {
        rend.material = hitMat;
        yield return new WaitForSeconds(0.1f);
        rend.material = baseMat;
    }

    void KnockBack()
    {

    }


    void CheckDeath() // Método para checagem de morte
    {
        if(hp <= 0)
        {
            Controller.instance.enemiesInScene--;
            Controller.instance.enemiesDefeated++;
            SkillController.instance.exp += 0.5f;
            Instantiate(deathPS, transform.position + Vector3.up, deathPS.transform.rotation);
            Instantiate(Player.instance.prefabCoin, new Vector3(transform.position.x,2f,transform.position.z), Quaternion.Euler(90f,0,0));
            Destroy(gameObject);
        }
    }

    void Golpe(Attack attack)
    {

        //Use the OverlapBox to detect if there are any other colliders within this box area.
        //Use the GameObject's centre, half the size (as a radius) and rotation. This creates an invisible box around your GameObject.
        Debug.Log(-Mathf.Sign(this.transform.rotation.eulerAngles.y-180));
        Collider[] hitColliders = Physics.OverlapBox(new Vector3(gameObject.transform.position.x + (attack.hitboxes[0].startingPointEnemy.x* -Mathf.Sign(this.transform.rotation.eulerAngles.y-180)), gameObject.transform.position.y + attack.hitboxes[0].startingPointEnemy.y, ((attack.hitboxes[0].extension.z/2.0f)+gameObject.transform.position.z)+attack.hitboxes[0].startingPointEnemy.z), attack.hitboxes[0].extension, gameObject.transform.rotation, playerMask);
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
        recoveryTimer = attack.recovery;

    }

    void OnDrawGizmos()
    {
        if(0 < currentAttack && currentAttack < 33){
            Gizmos.color = Color.red;
            //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
            if (m_Started)
                //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
                Gizmos.DrawWireCube(new Vector3((gameObject.transform.position.x + attackEnemy[0].hitboxes[0].startingPointEnemy.x* -Mathf.Sign(this.transform.rotation.eulerAngles.y-180)), gameObject.transform.position.y + attackEnemy[0].hitboxes[0].startingPointEnemy.y, ((attackEnemy[0].hitboxes[0].extension.z/2.0f)+gameObject.transform.position.z)+attackEnemy[0].hitboxes[0].startingPointEnemy.z), attackEnemy[0].hitboxes[0].extension);
        }
        if(33 <= currentAttack && currentAttack <= 66){
            Gizmos.color = Color.blue;
            //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
            if (m_Started)
                //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
                Gizmos.DrawWireCube(new Vector3((gameObject.transform.position.x + attackEnemy[1].hitboxes[0].startingPointEnemy.x* -Mathf.Sign(this.transform.rotation.eulerAngles.y-180)), gameObject.transform.position.y + attackEnemy[1].hitboxes[0].startingPointEnemy.y, ((attackEnemy[1].hitboxes[0].extension.z/2.0f)+gameObject.transform.position.z)+attackEnemy[1].hitboxes[0].startingPointEnemy.z), attackEnemy[1].hitboxes[0].extension);
        }
        if(currentAttack > 66){
            
            Gizmos.color = Color.green;
            //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
            if (m_Started)
                //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
                Gizmos.DrawWireCube(new Vector3((gameObject.transform.position.x + attackEnemy[2].hitboxes[0].startingPointEnemy.x* -Mathf.Sign(this.transform.rotation.eulerAngles.y-180)), gameObject.transform.position.y + attackEnemy[2].hitboxes[0].startingPointEnemy.y, ((attackEnemy[2].hitboxes[0].extension.z/2.0f)+gameObject.transform.position.z)+attackEnemy[2].hitboxes[0].startingPointEnemy.z), attackEnemy[2].hitboxes[0].extension);
        }
    }

    
    #endregion



}
