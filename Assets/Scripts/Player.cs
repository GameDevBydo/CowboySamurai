using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator anim;
    public CharacterController controller;
    public float exp = 0;
    private Vector3 playerVelocity;
    private Vector3 input;

    public GameObject prefabCoin;

    const float gravity= -9.81f;
    public bool groundedPlayer;
    public float speed = 2f, jump = 2f;

    SkinnedMeshRenderer rend;
    public Material baseMat, hitMat, dashMat;
    
    private float timerKnockback; 
    private bool canKnockback = false; 
    private Vector3 knockbackDirection;

    [HideInInspector]
    public bool canCheat = false;


    #region Singleton 
    public static Player instance;
    void Awake()
    {
        //Singleton básico, para evitar multiplos controllers na cena
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
    #endregion 

    void Start()
    {
        //moneyText = GameObject.Find("MoneyText").GetComponent<TextMeshProUGUI>();
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        rend = transform.GetChild(5).GetChild(0).GetComponent<SkinnedMeshRenderer>(); // Referenciado o meshrenderer do modelo do Lucas (objeto PM_Body)
        ChangePlayerState(0);
        m_Started = true;
        comboCounter = 0;
        getHit = true;
        canDash = true;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if(!Controller.instance.inputPause)
        {
            if(Input.GetKeyDown(KeyCode.Joystick1Button6) || Input.GetKeyDown(KeyCode.Z)){
                Controller.instance.ToggleShop();
            }       
            if(!Controller.instance.playerPause)
            {
                KnockBack();
                Cheats();
                MovementPlayer();
                if(attack != null) Timeout();
                CallAttack();
                ComboTimer();
                if(Input.GetKeyDown(KeyCode.Q) && canDash) StartCoroutine(DashCD());
                if(currentState == PlayerState.DASHING) controller.Move( transform.forward * Time.fixedDeltaTime * 10);
                if(Input.GetKeyDown(interactKey[0])||Input.GetKeyDown(interactKey[1])) Interact();
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if(hit.collider.CompareTag("Enemy") && getHit)
        {   
            canKnockback = true;
            knockbackDirection = new Vector3(transform.position.x - hit.transform.position.x, 1f, 0f).normalized;
            timerKnockback = 0f;
        }
    }

    #region State Machine
    enum PlayerState
    {
        CINEMATIC,
        STANDING,
        WALKING,
        AIRBORNE,
        ATTACKING,
        HITSTUN,
        SUPER,
        DASHING
    }

    PlayerState currentState, pastState;
    string animationName;
    public void ChangePlayerState(int id)
    {
        pastState = currentState;
        //if(id != 4)
        //{
            if(currentState != pastState) return;
        //}
        switch(id)
        {
            case 0:
                currentState = PlayerState.CINEMATIC;
                animationName = "Idle";
            break;
            case 1:
                currentState = PlayerState.STANDING;
                animationName = "Idle";
            break;
            case 2:
                currentState = PlayerState.WALKING;
                animationName = "Walk";
            break;
            case 3:
                currentState = PlayerState.AIRBORNE;
                animationName = "Jump";
            break;
            case 4:
                currentState = PlayerState.ATTACKING;
                animationName = comboSequence;
            break;
            case 5:
                currentState = PlayerState.HITSTUN;
                animationName = "Idle";
            break;
            case 6:
                currentState = PlayerState.DASHING;
                animationName = "Dash";
            break;
            case 7:
                currentState = PlayerState.SUPER;
                animationName = "S";
            break;
            default:
                //Debug.Log("Change to state 1");
                currentState = PlayerState.STANDING;
            break;
        }
        anim.Play(animationName);
        
    }
    #endregion

    public void KnockBack()
    {
        if(canKnockback)
        {
            timerKnockback += Time.deltaTime;
            if(timerKnockback <= 0.5f)
            {
                Vector3 knockbackForce = knockbackDirection * 5f * Time.fixedDeltaTime; 
                controller.Move(knockbackForce);
                //Debug.Log(knockbackForce);
                
            }
            else 
            {   
                TakeDamage(15);
                canKnockback = false;
            }
        
        }
    }
    #region Movimento do player
    float groundCheck = 0;
    public void MovementPlayer()
    {
        //groundedPlayer = controller.isGrounded;

        if(groundCheck <=0) groundedPlayer = Physics.Raycast(transform.position, Vector3.down, 0.5f);
        else
        {
            groundedPlayer = false;
            groundCheck-= Time.deltaTime;
        }

        
        input.Set(Input.GetAxisRaw("Horizontal"),0,0);
        
        //Verifica se o player está tocando no chão
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = -1f;
        }

        //Condição para verificar se o player está em movimento, se sim o corpo do player fica voltado para a direção do input e toca a animação de andar  
        if(input != Vector3.zero)
        {
            transform.forward = input;
            
            if((currentState != PlayerState.ATTACKING && currentState != PlayerState.SUPER && currentState != PlayerState.DASHING) && groundedPlayer) ChangePlayerState(2);
            //anim.SetBool("walking",true);
        }
        else
        {
            if((currentState != PlayerState.ATTACKING && currentState != PlayerState.SUPER && currentState != PlayerState.DASHING) && groundedPlayer) ChangePlayerState(1);
            //anim.SetBool("walking",false);
        }
        
        //Pulo do player
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            //playerVelocity.y += Mathf.Sqrt(jump * -3.0f * gravity);
            //anim.SetTrigger("jump");
            groundCheck = 1.03f;
            ChangePlayerState(3);

            // BERNARDOOOOO FAZ UM COOLDOWN PRO IS GROUND FICAR FALSOOO
        }

        //Aplicação da gravidade e da movimentação 
        playerVelocity.y += gravity * Time.deltaTime;
        if(currentState != PlayerState.SUPER) controller.Move( input* Time.deltaTime * speed);
        if(currentState != PlayerState.SUPER) controller.Move(playerVelocity * Time.deltaTime);

        //if(!groundedPlayer) ChangePlayerState(3);
        //if(input !=Vector3.zero) ChangePlayerState(2);
        //else ChangePlayerState(1);
    }
    #endregion

    #region Combate
    
    public int maxHP = 200, hitPoints = 200;

    public float bulletBar = 0, bulletMax = 0;


    public void TakeDamage(int damage)
    {
        hitPoints-=damage;
        StartCoroutine(HitMaterialEffect());
        Controller.instance.UpdateLifeBar((float)hitPoints/(float)maxHP);
        //Tocar o som de dano q esta no combo system
        CheckDeath();
    }

    public IEnumerator HitMaterialEffect()
    {
        rend.material = hitMat;
        yield return new WaitForSeconds(0.1f);
        rend.material = baseMat;
    }

    public void ChangeMeter(float val)
    {
        bulletBar = Mathf.Clamp(bulletBar+=val, 0, bulletMax);
        Controller.instance.UpdateBulletBar(bulletBar);
    }
    public void SetMaxMeter(int value)
    {
        bulletMax = (value+1)*20;
    }

    public void CheckDeath()
    {
        if(hitPoints<=0)
        {
            Controller.instance.GameOver();
            //ChangePlayerState(0);
        }
    }
    #endregion

    #region Combo System
        bool m_Started;
        public LayerMask m_EnemyLayer;
        public MoveList moveList, superList;
        public bool canAttack = true;
        public KeyCode[] lightAtk, heavyAtk, specialAtk, interactKey;

        [HideInInspector]
        public int comboCounter;

        [HideInInspector]
        public string comboSequence = "";

        float recoveryTimer;

        Attack attack = null;
        bool previousAttackHit = false;
        string previousAttack = null;


        #region Attacks

        float timeAttack;
        public void CallAttack() // Pega um input e ativa o golpe relacionado a esse input.
        {
            if(groundedPlayer && currentState != PlayerState.DASHING)
            {
                bool buttonPress = false;
                for(int i = 0; i<moveList._attack.Length; i++)
                {
                    moveList._attack[i].hit = false;
                }

                if(Input.GetKeyDown(lightAtk[0]) || Input.GetKeyDown(lightAtk[1]))
                {
                    comboSequence += "L";
                    buttonPress = true;
                } 
                
                
                if(Input.GetKeyDown(heavyAtk[0]) || Input.GetKeyDown(heavyAtk[1]))
                {
                    comboSequence += "H";
                    buttonPress = true;
                } 

                if(Input.GetKeyDown(specialAtk[0]) || Input.GetKeyDown(specialAtk[1]))
                {
                    int bullet = Mathf.FloorToInt(bulletBar/(20.0f));
                    if(bullet > 0)
                    {
                        SuperCollission(bullet-1);
                        ///Debug.Log("Super de " + bullet + " balas");
                    }
                } 

                if(buttonPress)
                {
                    //if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f)
                        CheckAttackCollision(comboSequence);
                    
                    buttonPress = false;
                }
            }
        }

        public bool getHit, canDash;
        
        IEnumerator DashCD()
        {
            canDash = false;
            getHit = false;
            rend.material = dashMat;
            ChangePlayerState(6);
            gameObject.layer = LayerMask.NameToLayer("Dash");
            yield return new WaitForSeconds(0.35f);
            ChangePlayerState(1);
            gameObject.layer = LayerMask.NameToLayer("Player");
            getHit = true;
            canDash = true;
            rend.material = baseMat;
        }
        #endregion

        #region Combo Counter

        float comboTimer;
        void ComboTimer()
        {
            if(comboTimer>0) comboTimer-=Time.fixedDeltaTime;
            else if(comboTimer<=0 && comboCounter>0) ResetComboCounter();    
        }
        public void IncreaseComboCounter()
        {
            comboTimer = 3;
            comboCounter++;
            UpdateComboCounter();
        }

        public void ResetComboCounter()
        {
            comboCounter = 0;
            UpdateComboCounter();
        }

        public void UpdateComboCounter()
        {
            if(comboCounter >0) 
            {
                Controller.instance.comboCounter.text = "Combo: " + comboCounter;
                if(comboCounter >= 10 && comboCounter<20)
                {
                    Controller.instance.comboComment.text = Controller.instance.comments.comments[0];
                }
                else if(comboCounter >= 20 && comboCounter<30)
                {
                    Controller.instance.comboComment.text = Controller.instance.comments.comments[1];
                }
                else if(comboCounter >= 30 && comboCounter<40)
                {
                    Controller.instance.comboComment.text = Controller.instance.comments.comments[2];
                }
                else if(comboCounter >= 40 && comboCounter<50)
                {
                    Controller.instance.comboComment.text = Controller.instance.comments.comments[3];
                }
                else if(comboCounter >= 50 && comboCounter<60)
                {
                    Controller.instance.comboComment.text = Controller.instance.comments.comments[4];
                }
                else if(comboCounter >= 60 && comboCounter<70)
                {
                    Controller.instance.comboComment.text = Controller.instance.comments.comments[5];
                }
            }
            else 
            {
                Controller.instance.comboCounter.text = "";
                Controller.instance.comboComment.text = "";
            }
        }

        #endregion

        #region  Timeout golpe
        void Timeout(){
            if(recoveryTimer>0)
            {
                canAttack = false;
                recoveryTimer -= Time.fixedDeltaTime;
            }
            else
            {
                canAttack = true;
                attack = null;
                comboSequence ="";
                ChangePlayerState(1);
            }
        }
        #endregion

        #region Efeitos

        public AudioSource hitSound, hurtSound;
        public void PlayHurtSound()
        {
            hurtSound.Stop();
            hurtSound.Play();
        }
        void PlayHitSound()
        {
            hitSound.Stop();
            hitSound.Play();
        }

        #endregion

        #region OverlapBox create

        public void CheckAttack(){
            
        }
        void CheckAttackCollision(string name) // Método para checar se o ataque colidiu com inimigos, e se sim, causar dano
        {
            for(int i= 0; i <moveList._attack.Length; i++)
            {
                if(moveList._attack[i].attackName == name && moveList.attackUnlocked[i])
                {
                    if(previousAttackHit && previousAttack != name) canAttack = true;
                    attack = moveList._attack[i];
                }
            }

            if(attack != null && canAttack)
            {
                anim.Play("ResetAttack");
                ChangePlayerState(4);
                //Debug.Log("Golpe atual: "+ name);

                // Duas listas são criadas, uma para os colisores e outra para os Scripts de Enemy
                // Após as checagens de colisão, a lista de scripts é preenchida e em seguida é inserida num loop que chamará a função de dano
                List<Collider> hitCollider = new List<Collider>();
                List<EnemyB> enemiesHit = new List<EnemyB>();

                for(int i = 0; i<attack.hitboxes.Length; i++)
                {
                    //Debug.Log(attack.attackName);
                    hitCollider.AddRange(Physics.OverlapBox(new Vector3((gameObject.transform.position.x + attack.hitboxes[i].startingPoint.x), gameObject.transform.position.y + attack.hitboxes[i].startingPoint.y, 
                    ((attack.hitboxes[i].extension.z/2.0f)+gameObject.transform.position.z)+attack.hitboxes[i].startingPoint.z), attack.hitboxes[i].extension, gameObject.transform.rotation, m_EnemyLayer));
                }

                foreach (Collider col in hitCollider)
                {
                    if(!enemiesHit.Contains(col.gameObject.GetComponent<EnemyB>())) enemiesHit.Add(col.gameObject.GetComponent<EnemyB>());
                }

                foreach(EnemyB en in enemiesHit)
                {
                    IncreaseComboCounter();
                    en.TakeDamage(attack.damage, attack.stun, attack.sfx);
                    ChangeMeter(attack.meterGen);
                }
                recoveryTimer = attack.recovery;
                if(enemiesHit.Count>0) 
                {
                    attack.hit = true;
                    PlayHitSound();
                }
                previousAttackHit = attack.hit;
                previousAttack = name;
            }
            else
            {
                if(currentState != PlayerState.ATTACKING || currentState != PlayerState.SUPER)
                    comboSequence = "";
            }
        }

        void SuperCollission(int meter)
        {
            
            attack = superList._attack[meter];

            if(attack != null)
            {
                anim.Play("ResetAttack");
                ChangePlayerState(7);
                // Duas listas são criadas, uma para os colisores e outra para os Scripts de Enemy
                // Após as checagens de colisão, a lista de scripts é preenchida e em seguida é inserida num loop que chamará a função de dano

                List<Collider> hitCollider = new List<Collider>();
                List<Enemy> enemiesHit = new List<Enemy>();

                for(int i = 0; i<attack.hitboxes.Length; i++)
                {
                    //Debug.Log(attack.attackName);
                    Vector3 p = gameObject.transform.position + attack.hitboxes[i].startingPoint;
                    p.z += attack.hitboxes[i].extension.z * 0.5f;
                    hitCollider.AddRange(Physics.OverlapBox(p, attack.hitboxes[i].extension, Quaternion.identity, m_EnemyLayer));
                }

                foreach (Collider col in hitCollider)
                {
                    if(!enemiesHit.Contains(col.gameObject.GetComponent<Enemy>())) enemiesHit.Add(col.gameObject.GetComponent<Enemy>());
                }
                float dis = 0, minDis = 100;
                Enemy closestEnemy = null;
                for(int i = 0; i< meter+1; i++)
                {
                    foreach(Enemy en in enemiesHit)
                    {
                        dis = Vector3.Distance(en.transform.position, transform.position);
                        if(dis < minDis) closestEnemy = en;
                        
                    }
                    if(closestEnemy != null)
                    {
                    IncreaseComboCounter();
                    enemiesHit.Remove(closestEnemy);
                    closestEnemy.TakeDamage(attack.damage, attack.stun, attack.sfx);
                    }
                }

                recoveryTimer = attack.recovery;
                if(enemiesHit.Count>0) 
                {
                    attack.hit = true;
                    PlayHitSound();
                }
                ChangeMeter(attack.meterGen);
                previousAttackHit = attack.hit;
            }
            else
            {
                if(currentState != PlayerState.ATTACKING || currentState != PlayerState.SUPER)
                    comboSequence = "";
            }
        }
        #endregion

        #region Gizmos
        //Desenhe o Box Overlap como um gizmo para mostrar onde ele está testando no momento
        void OnDrawGizmos()
        {
            if(attack != null)
            {
                if(Input.GetKey(lightAtk[0]) && !canAttack || Input.GetKey(lightAtk[1]) && !canAttack)
                {
                    for(int i = 0; i<attack.hitboxes.Length; i++){
                        
                        Gizmos.color = Color.blue;
                        //Verifica se está rodando no modo Play , para não tentar desenhar isso no modo Editor
                        if (m_Started){
                            Vector3 p = gameObject.transform.position + attack.hitboxes[i].startingPoint;
                            Vector3 e = attack.hitboxes[i].extension*2;
                            p.z += attack.hitboxes[i].extension.z * 0.5f;
                            Gizmos.DrawWireCube(p , new Vector3(e.z, e.y, e.x));
                        }
                    }
                }

                if(Input.GetKey(heavyAtk[0]) && !canAttack || Input.GetKey(heavyAtk[1]) && !canAttack)
                {
                    for(int i = 0; i<attack.hitboxes.Length; i++){
                        
                        Gizmos.color = Color.red;
                        if (m_Started){
                            Vector3 p = gameObject.transform.position + attack.hitboxes[i].startingPoint;
                            Vector3 e = attack.hitboxes[i].extension*2;
                            p.z += attack.hitboxes[i].extension.z * 0.5f;
                            Gizmos.DrawWireCube(p , new Vector3(e.z, e.y, e.x));
                        }
                    }
                }

                if(Input.GetKeyDown(specialAtk[0]) || Input.GetKeyDown(specialAtk[1]))
                {
                    for(int i = 0; i<attack.hitboxes.Length; i++){
                        
                        Gizmos.color = Color.yellow;
                        if (m_Started){
                            Vector3 p = gameObject.transform.position + attack.hitboxes[i].startingPoint;
                            Vector3 e = attack.hitboxes[i].extension*2;
                            p.z += attack.hitboxes[i].extension.z * 0.5f;
                            Gizmos.DrawWireCube(p , new Vector3(e.z, e.y, e.x));
                        }
                    }
                } 
            }
        }
        #endregion
    #endregion

    #region Cheats 

    void Cheats()
    {
        //if(canCheat)
        //{
            if(Input.GetKeyDown(KeyCode.F1)) GainLifeCheat();
            if(Input.GetKeyDown(KeyCode.F2)) GainMoneyCheat();
            if(Input.GetKeyDown(KeyCode.F3)) GainEXPCheat();
            if(Input.GetKeyDown(KeyCode.F8)) ClearLevel();
            if(Input.GetKeyDown(KeyCode.F9)) MaximizeSkillTree();
        //}
    }
    void MaximizeSkillTree()
    {
        for(int i = 0; i< SkillController.instance.skills.Length; i++)
        {
            SkillController.instance.UnlockSKill(i);
        }
        for(int i = 0; i< SkillController.instance.superSkills.Length; i++)
        {
            SkillController.instance.UnlockSuper(i);
        }
        ///Debug.Log("<color=green>Desbloqueou todos os golpes.</color>");
    }

    void GainLifeCheat()
    {
        hitPoints+= 50;
        hitPoints = Mathf.Clamp(hitPoints, 0, maxHP);
        Controller.instance.UpdateLifeBar((float)hitPoints/(float)maxHP);
    }
    
    void GainMoneyCheat()
    {
        Controller.instance.money += 20;
    }
    void GainEXPCheat()
    {
        exp += 15;
    }

    void ClearLevel()
    {
        Controller.instance.enemiesDefeated = 99;
    }
    #endregion
    

    public Vector3 interactSize;

    void Interact()
    {   
        List<TicketSO> tickets = new List<TicketSO>();
        Collider[] colliders = Physics.OverlapBox(transform.position, interactSize);
        //Meter um physics box aqui que pega os objeto com tag ticket e filtra pelo nome pra puxar o ticket da lista.

        foreach(Collider collider in colliders)
        {
            if(collider.gameObject.name.Contains("Tic"))
            {
                tickets.Add(collider.gameObject.GetComponent<Ticket>().ticketSO);
                Destroy(collider.gameObject);
            }
            else if(collider.gameObject.name.Contains("NPC"))
            {
                collider.gameObject.GetComponent<Npc>().beginInteraction = true;
            }
        }

        if(tickets.Count>0)
        {
            foreach(TicketSO tic in tickets)
            {
                Controller.instance.CollectTicket(tic);
            }
        }
    }
}