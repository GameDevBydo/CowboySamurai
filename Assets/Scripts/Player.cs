using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator anim;
    private CharacterController controller;
    public float exp = 0;
    private Vector3 playerVelocity;
    private Vector3 input;

    const float gravity= -9.81f;
    public bool groundedPlayer;
    public float speed = 2f, jump = 2f;

    SkinnedMeshRenderer rend;
    public Material baseMat, hitMat, dashMat;

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
        //DontDestroyOnLoad(gameObject);
    }
    #endregion 

    #region State Machine
    enum PlayerState
    {
        CINEMATIC,
        STANDING,
        WALKING,
        AIRBORNE,
        ATTACKING,
        HITSTUN,
        DASHING
    }

    PlayerState currentState, pastState;

    public void ChangePlayerState(int id)
    {
        pastState = currentState;
        switch(id)
        {
            case 0:
                currentState = PlayerState.CINEMATIC;
            break;
            case 1:
                currentState = PlayerState.STANDING;
            break;
            case 2:
                currentState = PlayerState.WALKING;
            break;
            case 3:
                currentState = PlayerState.AIRBORNE;
            break;
            case 4:
                currentState = PlayerState.ATTACKING;
            break;
            case 5:
                currentState = PlayerState.HITSTUN;
            break;
            case 6:
                currentState = PlayerState.DASHING;
            break;
            default:
                Debug.Log("Change to state 1");
                currentState = PlayerState.STANDING;
            break;
        }
    }
    #endregion
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        rend = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
        ChangePlayerState(0);
        m_Started = true;
        comboCounter = 0;
        getHit = true;
        canDash = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AttSpawnPosition();
        if(!Controller.instance.inputPause)
        {
            if(Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) PlayerPause(); // Pause funciona apenas no teclado por enquanto
            
            if(!Controller.instance.playerPause)
            {
                MovimentPlayer();
                Timeout();
                CallAttack();
                ComboTimer();
                if(Input.GetKeyDown(KeyCode.Q) && canDash) StartCoroutine(DashCD());
            }
        }
    }

    public void AttSpawnPosition(){
        Controller.instance.spawns[0].transform.position = new Vector3(transform.position.x -10, 0, 0);
        Controller.instance.spawns[1].transform.position = new Vector3(transform.position.x +10, 0, 0);
    }


    #region Movimento do player
    public void MovimentPlayer()
    {
        groundedPlayer = controller.isGrounded;
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
            anim.SetBool("walking",true);
        }
        else
        {
            anim.SetBool("walking",false);
        }
        
        //Pulo do player
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jump * -3.0f * gravity);
            anim.SetTrigger("jump");
        }

        //Aplicação da gravidade e da movimentação 
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move( input* Time.deltaTime * speed);
        controller.Move(playerVelocity * Time.deltaTime);

        if(!groundedPlayer) ChangePlayerState(3);
        if(input !=Vector3.zero) ChangePlayerState(2);
        else ChangePlayerState(1);
    }
    #endregion

    #region Combate
    
    public int maxHP = 200, hitPoints = 200;
    private float knockbackForce = 300.0f;
    private float knockbackRadius;

    public float bulletBar = 0, bulletMax = 120;


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

    public void CheckDeath()
    {
        if(hitPoints<=0)
        {
            Controller.instance.GameOver();
        }
    }
    // gives the knockback
    private void OnCollisionEnter(Collision collision)
    {
        /*Rigidbody rb = collision.transform.position - transform.position;
        Collider[] colliders = Physics.OverlapSphere(transform.position, knockbackRadius); 

        if(rb != null)
        {
            Vector3 direction = collision.transform.position - transform.position;
            direction.y = 0;

            rb.AddForce(direction.normalized * knockbackForce, ForceMode.Impulse);
        }
        for(int i = 0; i < colliders.Length; i++)
        {
            Rigidbody rb = colliders[i].GetComponent<Rigidbody>();

            if(rb != null)
            {
                rb.AddExplosionForce(knockbackForce, transform.position, knockbackRadius, 0f, ForceMode.Impulse);
                //capable of using ExplosionForce when enemy dies. They "fly" away
            }
        */

    }
    //gives the knockback area
    //send a raytracing to set the hit of the knockback

    #endregion

    void PlayerPause()
    {
        Controller.instance.TogglePlayerPause();
    }

    #region Combo System
        bool m_Started;
        public LayerMask m_EnemyLayer;
        public MoveList moveList, superList;
        public bool canAttack = true;
        public KeyCode[] lightAtk, heavyAtk, specialAtk;

        [HideInInspector]
        public int comboCounter;

        [HideInInspector]
        public string comboSequence = "";

        float recoveryTimer;

        Attack attack = null;
        bool previousAttackHit = false;


        #region Attacks
        public void CallAttack() // Pega um input e ativa o golpe relacionado a esse input.
        {
            bool buttonPress = false;
            ChangePlayerState(4);
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
                int bullet = Mathf.FloorToInt(bulletBar/(bulletMax/6));
                if(bullet > 0)
                {
                    SuperCollission(bullet-1);
                    Debug.Log("Super de " + bullet + "balas");
                }
            } 

            if(buttonPress)
            {
                CheckAttackCollision(comboSequence);
                buttonPress = false;
            }
        }

        public bool getHit, canDash;
        

        IEnumerator DashCD()
        {
            canDash = false;
            getHit = false;
            rend.material = dashMat;
            yield return new WaitForSeconds(2);
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
        void CheckAttackCollision(string name) // Método para checar se o ataque colidiu com inimigos, e se sim, causar dano
        {
            for(int i= 0; i <moveList._attack.Length; i++)
            {
                if(moveList._attack[i].attackName == name && moveList.attackUnlocked[i])
                {
                    if(previousAttackHit) canAttack = true;
                    attack = moveList._attack[i];
                }
            }

            if(attack != null && canAttack)
            {
                //Debug.Log("Golpe atual: "+ name);

                // Duas listas são criadas, uma para os colisores e outra para os Scripts de Enemy
                // Após as checagens de colisão, a lista de scripts é preenchida e em seguida é inserida num loop que chamará a função de dano
                List<Collider> hitCollider = new List<Collider>();
                List<Enemy> enemiesHit = new List<Enemy>();

                for(int i = 0; i<attack.hitboxes.Length; i++)
                {
                    Debug.Log(attack.attackName);
                    hitCollider.AddRange(Physics.OverlapBox(new Vector3((gameObject.transform.position.x + attack.hitboxes[i].startingPoint.x), gameObject.transform.position.y + attack.hitboxes[i].startingPoint.y, 
                    ((attack.hitboxes[i].extension.z/2.0f)+gameObject.transform.position.z)+attack.hitboxes[i].startingPoint.z), attack.hitboxes[i].extension, gameObject.transform.rotation, m_EnemyLayer));
                }

                foreach (Collider col in hitCollider)
                {
                    if(!enemiesHit.Contains(col.gameObject.GetComponent<Enemy>())) enemiesHit.Add(col.gameObject.GetComponent<Enemy>());
                }

                foreach(Enemy en in enemiesHit)
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
            }
            else
            {
                comboSequence = "";
            }
        }

        void SuperCollission(int meter)
        {
            attack = superList._attack[meter];

            if(attack != null && canAttack)
            {
                // Duas listas são criadas, uma para os colisores e outra para os Scripts de Enemy
                // Após as checagens de colisão, a lista de scripts é preenchida e em seguida é inserida num loop que chamará a função de dano

                List<Collider> hitCollider = new List<Collider>();
                List<Enemy> enemiesHit = new List<Enemy>();

                for(int i = 0; i<attack.hitboxes.Length; i++)
                {
                    Debug.Log(attack.attackName);
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
                    en.TakeDamage(attack.damage, attack.stun, attack.sfx);
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
                comboSequence = "";
            }
        }
        #endregion

        #region Cheats 

        void UnlockAllMoves()
        {
            for(int i = 0; i< moveList.attackUnlocked.Length; i++)
            {
                moveList.attackUnlocked[i] = true;
            }
            Debug.Log("<color=green>Unlocked all moves.</color>");
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
}