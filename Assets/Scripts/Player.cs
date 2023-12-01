using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

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

    [SerializeField]
    private InputActionReference move, lightAttack, heavyAttack, interagir, jumpAction, dash, supremo, pause, skillTree;


    #region Singleton 
    public static Player instance;
    void Awake()
    {
        //controls.Controls.Start.performed += ctx =>  Awake();
        //controls. Controls.SkillTree.performed += ctx =>  Awake();

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
        SetMaxMeter(0);
        //moneyText = GameObject.Find("MoneyText").GetComponent<TextMeshProUGUI>();
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        rend = transform.GetChild(5).GetChild(0).GetComponent<SkinnedMeshRenderer>(); // Referenciado o meshrenderer do modelo do Lucas (objeto PM_Body)
        ChangePlayerState(0);
        m_Started = true;
        comboCounter = 0;
        getHit = true;
        
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if(!Controller.instance.inputPause)
        {
            /*if(interagir.action.triggered){
                Controller.instance.ToggleShop();
            }     */  
            if(!Controller.instance.playerPause)
            {
                KnockBack();
                Cheats();
                MovementPlayer();
                CallAttack();
                ComboTimer();
                AttackTimer();
                if(dashCD >0) dashCD-= Time.fixedDeltaTime;
                else canDash = true;
                //Input.GetKeyDown(KeyCode.Q) && canDash || Input.GetButtonDown("Dash") &&
                if(dash.action.triggered && canDash) StartCoroutine(DashCD());
                if(currentState == PlayerState.DASHING) controller.Move( transform.forward * Time.fixedDeltaTime * 7);
                if(interagir.action.triggered) Interact();
            }
        }
    }

    void checkDash()
    {
        if(canDash)
        {
            DashCD();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        /*if(hit.collider.CompareTag("Enemy") && getHit)
        {   
            canKnockback = true;
            knockbackDirection = new Vector3(transform.position.x - hit.transform.position.x, 1f, 0f).normalized;
            timerKnockback = 0f;
        }*/
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
                if(currentHitstun == 0)
                    animationName = "Stagger-Light";
                if(currentHitstun == 1)
                    animationName = "Stagger-Downed";
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
    public int currentHitstun;

    public IEnumerator Hitstun()
    {
        ChangePlayerState(5);
        GetComponent<CharacterController>().enabled = false;
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        GetComponent<CharacterController>().enabled = true;
        ChangePlayerState(1);
    }
    #region Movimento do player
    float groundCheck = 0;
    public void MovementPlayer()
    {
        //groundedPlayer = controller.isGrounded;

        if(groundCheck <=0) groundedPlayer = Physics.Raycast(transform.position, Vector3.down, 1.0f);
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
            
            if((currentState != PlayerState.ATTACKING && currentState != PlayerState.SUPER && currentState != PlayerState.DASHING && currentState != PlayerState.HITSTUN) && groundedPlayer) ChangePlayerState(2);
            
        }
        else
        {
            if((currentState != PlayerState.ATTACKING && currentState != PlayerState.SUPER && currentState != PlayerState.DASHING && currentState != PlayerState.HITSTUN) && groundedPlayer) ChangePlayerState(1);
           
        }
        
        
        if (jumpAction.action.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jump * -3.0f * gravity);            
            groundCheck = 1.03f;
            ChangePlayerState(3);
   
        }

        //Aplicação da gravidade e da movimentação 
        playerVelocity.y += gravity * Time.deltaTime;
        if(currentState != PlayerState.SUPER) controller.Move( input* Time.deltaTime * speed);
        if(currentState != PlayerState.SUPER) controller.Move(playerVelocity * Time.deltaTime);

    }

    #endregion

    #region Combate
    
    public int maxHP = 200, hitPoints = 200;

    public float bulletBar = 0, bulletMax = 0;


    public AudioClip takeHit;
    public void TakeDamage(int damage)
    {
        hitPoints-=damage;
        StartCoroutine(HitMaterialEffect());
        Controller.instance.UpdateLifeBar((float)hitPoints/(float)maxHP);
        Controller.instance.audio.PlayEffect(takeHit);
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
        bulletBar = Mathf.Clamp(bulletBar+=(val*20), 0, bulletMax);
        Controller.instance.UpdateBulletBar(bulletBar);
        
    }
    public void SetMaxMeter(int value)
    {
        bulletMax = (value)*20;
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
        public bool previousAttackHit = false;
        string previousAttack = null;


        #region Attacks

        float timeAttack;
        bool buttonPress = false;
        public void CallAttack() // Pega um input e ativa o golpe relacionado a esse input.
        {
            if(groundedPlayer && currentState != PlayerState.DASHING)
            {
                for(int i = 0; i<moveList._attack.Length; i++)
                {
                    moveList._attack[i].hit = false;
                }
                //Input.GetKeyDown(lightAtk[0]) || Input.GetKeyDown(lightAtk[1]) || Input.GetButtonDown("Fire1")
                if(lightAttack.action.triggered)
                {
                    if(comboSequence == "" || comboSequence != "" && previousAttackHit && canInput)
                    {
                        attackTimer = 3;
                        comboSequence += "L";
                        buttonPress = true;
                        canInput = false;
                    }
                } 

                //Input.GetKeyDown(heavyAtk[0]) || Input.GetKeyDown(heavyAtk[1]) || Input.GetButtonDown("Fire2")
                if(heavyAttack.action.triggered)
                {
                    if(comboSequence == "" || comboSequence != "" && previousAttackHit && canInput)
                    {
                        attackTimer = 3;
                        comboSequence += "H";
                        buttonPress = true;
                        canInput = false;
                    }
                } 

                if(supremo.action.triggered)
                {
                    attackTimer = 3;
                    int bullet = Mathf.FloorToInt(bulletBar/(20.0f));
                    if(bullet > 0)
                    {
                        ChangePlayerState(7);
                        ChangeMeter(-1);
                        ///Debug.Log("Super de " + bullet + " balas");
                    }
                } 

                if(buttonPress)
                {
                    attackTimer = 3;
                    Debug.Log(comboSequence);
                    //ChangePlayerState(4);
                    //CheckAttackCollision(comboSequence);
                    CheckAnimation(comboSequence);
                    buttonPress = false;
                }
            }
        }
        
        void LightAttack()
        {
            if(groundedPlayer && currentState != PlayerState.DASHING)
            {
                for(int i = 0; i<moveList._attack.Length; i++)
                {
                    moveList._attack[i].hit = false;
                }

                if(comboSequence == "" || comboSequence != "" && previousAttackHit && canInput)
                {
                    comboSequence += "L";
                    buttonPress = true;
                    canInput = false;
                }
            }
        }
        void HeavyAttack()
        {
            if(groundedPlayer && currentState != PlayerState.DASHING)
            {
                for(int i = 0; i<moveList._attack.Length; i++)
                {
                    moveList._attack[i].hit = false;
                }
                if(comboSequence == "" || comboSequence != "" && previousAttackHit && canInput)
                {
                    comboSequence += "H";
                    buttonPress = true;
                    canInput = false;
                }
            }
        }
        void SpecialAttack()
        {
            if(groundedPlayer && currentState != PlayerState.DASHING)
            {
                for(int i = 0; i<moveList._attack.Length; i++)
                {
                    moveList._attack[i].hit = false;
                }

                int bullet = Mathf.FloorToInt(bulletBar/(20.0f));
                if(bullet > 0)
                {
                    //SuperCollission(bullet-1);
                    ///Debug.Log("Super de " + bullet + " balas");
                }
            }
        }

        public bool getHit, canDash;
        public float dashDuration = 0.2f, dashCD, dashMaxCD = 3;
        IEnumerator DashCD()
        {
            canDash = false;
            getHit = false;
            rend.material = dashMat;
            dashCD = dashMaxCD+dashDuration;
            ChangePlayerState(6);
            gameObject.layer = LayerMask.NameToLayer("Dash");
            yield return new WaitForSeconds(dashDuration);
            ChangePlayerState(1);
            gameObject.layer = LayerMask.NameToLayer("Player");
            getHit = true;
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

        float attackTimer = 3;
        void AttackTimer()
        {
            if(attackTimer>0) attackTimer-=Time.fixedDeltaTime;
            else if(attackTimer<=0 && !canInput) RestartAttack();
        }
        public void IncreaseComboCounter()
        {
            comboTimer = 2;
            comboCounter++;
            UpdateComboCounter();
        }

        public void ResetComboCounter()
        {
            RestartAttack();
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

        public int extraDamage = 0;
        public void IncreaseBaseDamage(int value)
        {
            extraDamage = value;
        }

        #endregion

        #region OverlapBox create

        string previousAnimation = null;
        bool canInput = true;

        void CheckAnimation(string name){
            for(int i= 0; i <moveList._attack.Length; i++)
            {
                if(moveList._attack[i].attackName == name && moveList.attackUnlocked[i])
                {
                    Debug.Log("attack registered.");
                    if(previousAttackHit || comboSequence.Length <= 1)
                    {
                        //anim.Stop();
                        ChangePlayerState(4);
                    }
                }
            }
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
                
                //anim.Play("ResetAttack");
                
                //Debug.Log("Golpe atual: "+ name);

                // Duas listas são criadas, uma para os colisores e outra para os Scripts de Enemy
                // Após as checagens de colisão, a lista de scripts é preenchida e em seguida é inserida num loop que chamará a função de dano
                List<Collider> hitCollider = new List<Collider>();
                List<EnemyB> enemiesHit = new List<EnemyB>();
                List<Boss> bossHit = new List<Boss>();

                for(int i = 0; i<attack.hitboxes.Length; i++)
                {
                    //Debug.Log(attack.attackName);
                    hitCollider.AddRange(Physics.OverlapBox(new Vector3((gameObject.transform.position.x + attack.hitboxes[i].startingPoint.x), gameObject.transform.position.y + attack.hitboxes[i].startingPoint.y, 
                    ((attack.hitboxes[i].extension.z/2.0f)+gameObject.transform.position.z)+attack.hitboxes[i].startingPoint.z), attack.hitboxes[i].extension, gameObject.transform.rotation, m_EnemyLayer));
                }

                foreach (Collider col in hitCollider)
                {
                    if(!enemiesHit.Contains(col.gameObject.GetComponent<EnemyB>())) enemiesHit.Add(col.gameObject.GetComponent<EnemyB>());
                    else if(!bossHit.Contains(col.gameObject.GetComponent<Boss>())) bossHit.Add(col.gameObject.GetComponent<Boss>());
                }

                foreach(EnemyB en in enemiesHit)
                {
                    IncreaseComboCounter();
                    en.TakeDamage(attack.damage + extraDamage, attack.stun, attack.sfx);
                    ChangeMeter(attack.meterGen);
                }
                foreach(Boss boss in bossHit)
                {
                    IncreaseComboCounter();
                    boss.TakeDamage(attack.damage, attack.sfx);
                    ChangeMeter(attack.meterGen);
                }
                recoveryTimer = attack.recovery;
                if(enemiesHit.Count>0) 
                {
                    attack.hit = true;
                    PlayHitSound();
                    canInput = true;
                }
                else if(bossHit.Count>0) 
                {
                    attack.hit = true;
                    PlayHitSound();
                    canInput = true;
                }
                else
                {
                    attack.hit = false;
                }
                previousAttackHit = attack.hit;
                previousAttack = name;
            }
            else
            {
                comboSequence = "";
            }
        }

        // Checar se a string atual é diferente da anterior, e se for, chamar o golpe e truezar o newMove
        public void CallEndAnimation() // É PRA CHAMAR ESSA AQUI LUIS 
        {
            bool newMove = false;

            if(previousAttackHit && comboSequence.Length>1)
            {
                for(int i= 0; i <moveList._attack.Length; i++)
                {
                    if(moveList._attack[i].attackName == comboSequence && moveList.attackUnlocked[i])
                    {
                        Debug.Log("Call new move");
                        ChangePlayerState(4);
                        newMove = true;
                    }
                }
            }
                
            if(!newMove) RestartAttack();
        }

        public void RestartAttack()
        {
            Debug.Log("restartou");
            ChangePlayerState(1);
            comboSequence = "";
            attack = null;
            canAttack = true;
            canInput = true;
            Debug.Log("attack reset.");
        }

        void SuperCollission()
        {
            int bullet = Mathf.FloorToInt(bulletBar/(20.0f));
            attack = superList._attack[bullet];

            if(attack != null)
            {
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
                
                foreach(Enemy en in enemiesHit)
                {
                    IncreaseComboCounter();
                    en.TakeDamage(attack.damage + extraDamage, attack.stun, attack.sfx);
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
                if(Input.GetKey(lightAtk[0]) && !canAttack || Input.GetKey(lightAtk[1]) && !canAttack || Input.GetButtonDown("Jump") && !canAttack)
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

                if(Input.GetKey(heavyAtk[0]) && !canAttack || Input.GetKey(heavyAtk[1]) && !canAttack || Input.GetButtonDown("Fire2") && !canAttack)
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
    public GameObject psTicketCollect, psNpcInteract;
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
                Instantiate(psTicketCollect, collider.transform.position, collider.transform.rotation);
                Destroy(collider.gameObject);
            }
            else if(collider.gameObject.name.Contains("NPC"))
            {
                if (collider.gameObject.GetComponent<Npc>().canInteractAgain)
                {
                    Instantiate(psNpcInteract, collider.transform.position, collider.transform.rotation);
                }
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

    public AudioSource gun;
    public void PlayGunShot()
    {
        gun.Play();
    }
}