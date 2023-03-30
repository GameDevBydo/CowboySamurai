using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator anim;
    private CharacterController controller;

    private Vector3 playerVelocity;
    private Vector3 input;

    const float gravity= -9.81f;
    public bool groundedPlayer;
    public float speed = 2f, jump = 2f;
    public static Player instance;
    
    #region Singleton 
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
        ChangePlayerState(0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!Controller.instance.inputPause)
        {
            if(Input.GetKeyDown(KeyCode.P)) PlayerPause(); // Pause funciona apenas no teclado por enquanto
            
            if(!Controller.instance.playerPause)
            {
                MovimentPlayer();
            }
        }
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

    void PlayerPause()
    {
        Controller.instance.TogglePlayerPause();
    }
}