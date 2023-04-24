using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    bool m_Started;
    public LayerMask m_EnemyLayer;
    public MoveList moveList;
    public bool canAttack = true;
    public KeyCode[] lightAtk, heavyAtk, specialAtk;

    [HideInInspector]
    public int comboCounter;

    [HideInInspector]
    public string comboSequence = "";

    float recoveryTimer;

    Attack attack = null, previousAttack = null;
    void Start()
    {
        //Use this to ensure that the Gizmos are being drawn when in Play Mode.
        m_Started = true;
        comboCounter = 0;
    }

    void FixedUpdate()
    {
        Timeout();
        CallAttack();
        ComboTimer();
    }

    #region Attacks
    public void CallAttack() // Pega um input e ativa o golpe relacionado a esse input.
    {
        bool buttonPress = false;
        Player.instance.ChangePlayerState(4);
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

        if(buttonPress)
        {
            CheckAttackCollision(comboSequence);
            buttonPress = false;
        }
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
        if(comboCounter >0) Controller.instance.comboCounter.text = "Combo: " + comboCounter;
        else Controller.instance.comboCounter.text = "";
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
            Player.instance.ChangePlayerState(1);   
        }
    }
    #endregion

    #region OverlapBox create
    void CheckAttackCollision(string name) // Método para checar se o ataque colidiu com inimigos, e se sim, causar dano
    {
        for(int i= 0; i <moveList._attack.Length; i++)
        {
            if(moveList._attack[i].attackName == name && moveList.attackUnlocked[i])
            {
                if(previousAttack != null) Debug.Log("Golpe Anterior: " + previousAttack.hit);
                if(previousAttack != null && previousAttack.hit == true) canAttack = true;
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
                hitCollider.AddRange(Physics.OverlapBox(new Vector3((gameObject.transform.position.x + attack.hitboxes[i].startingPoint.x), gameObject.transform.position.y + attack.hitboxes[i].startingPoint.y, ((attack.hitboxes[i].extension.z/2.0f)+gameObject.transform.position.z)+attack.hitboxes[i].startingPoint.z), attack.hitboxes[i].extension, gameObject.transform.rotation, m_EnemyLayer));
            }

            foreach (Collider col in hitCollider)
            {
                if(!enemiesHit.Contains(col.gameObject.GetComponent<Enemy>())) enemiesHit.Add(col.gameObject.GetComponent<Enemy>());
            }

            foreach(Enemy en in enemiesHit)
            {
                IncreaseComboCounter();
                en.TakeDamage(attack.damage);
            }
            recoveryTimer = attack.recovery;
            if(enemiesHit.Count>0) attack.hit = true;
            previousAttack = attack;
            Debug.Log(previousAttack.hit);
        }
        else
        {
            comboSequence = "";
        }
    }
    #endregion

// aaaaaaa roque 

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
                    if (m_Started)
                    Gizmos.DrawWireCube(new Vector3(gameObject.transform.position.x + attack.hitboxes[i].startingPoint.x, gameObject.transform.position.y + attack.hitboxes[i].startingPoint.y, (attack.hitboxes[i].extension.z/2.0f)+gameObject.transform.position.z+attack.hitboxes[i].startingPoint.z), attack.hitboxes[i].extension*2);
                    
                }
            }

            if(Input.GetKey(heavyAtk[0]) && !canAttack || Input.GetKey(heavyAtk[1]) && !canAttack)
            {
                for(int i = 0; i<attack.hitboxes.Length; i++){
                    
                    Gizmos.color = Color.red;
                    if (m_Started)
                    Gizmos.DrawWireCube(new Vector3(gameObject.transform.position.x + attack.hitboxes[i].startingPoint.x, gameObject.transform.position.y + attack.hitboxes[i].startingPoint.y, (attack.hitboxes[i].extension.z/2.0f)+gameObject.transform.position.z+attack.hitboxes[i].startingPoint.z), attack.hitboxes[i].extension*2);
                    
                }
            }
        }
    }
    #endregion
}
