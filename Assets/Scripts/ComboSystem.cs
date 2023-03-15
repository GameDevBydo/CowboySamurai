using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    bool m_Started;
    public LayerMask m_EnemyLayer;
    public MoveList moveList;
    public bool isValidBlow;
    public KeyCode[] lightAtk, heavyAtk, specialAtk;


    void Start()
    {
        //Use this to ensure that the Gizmos are being drawn when in Play Mode.
        m_Started = true;
    }

    void FixedUpdate()
    {
        CallAttack();
    }

    #region Attacks
    public void CallAttack() // Pega um input e ativa o golpe relacionado a esse input.
    {
        for(int i = 0; i<moveList._attack.Length; i++)
        {
            moveList._attack[i].hit = false;
        }

        if(Input.GetKey(lightAtk[0]) && !isValidBlow || Input.GetKey(lightAtk[1]) && !isValidBlow){
            CheckAttackCollision(0);
            StartCoroutine(timeout(moveList._attack[0].freezeTime));
        }
        
        if(Input.GetKey(heavyAtk[0]) && !isValidBlow || Input.GetKey(heavyAtk[1]) && !isValidBlow){
            CheckAttackCollision(1);
            StartCoroutine(timeout(moveList._attack[1].freezeTime));
        }
    }
    #endregion

    #region  Timeout golpe
    IEnumerator timeout(float value){
        //Coloca como verdadeiro a variavel que controla se o player pode usar o golpe
        isValidBlow = true;
        //time para que a variavel volte para falso e o player volte a poder usar o golpe
        yield return new WaitForSeconds(value);
        //variavel volta para falso para poder usar o golpe
        isValidBlow = !isValidBlow;
    }
    #endregion

    #region OverlapBox create
    void CheckAttackCollision(int moveId) // Método para checar se o ataque colidiu com inimigos, e se sim, causar dano
    {
        // Duas listas são criadas, uma para os colisores e outra para os Scripts de Enemy
        // Após as checagens de colisão, a lista de scripts é preenchida e em seguida é inserida num loop que chamará a função de dano
        List<Collider> hitCollider = new List<Collider>();
        List<Enemy> enemiesHit = new List<Enemy>();
 
        for(int i = 0; i<moveList._attack[moveId].hitboxes.Length; i++)
        {
            hitCollider.AddRange(Physics.OverlapBox(new Vector3(gameObject.transform.position.x + moveList._attack[moveId].hitboxes[i].startingPoint.x, gameObject.transform.position.y + moveList._attack[moveId].hitboxes[i].startingPoint.y, ((moveList._attack[0].hitboxes[i].extension.z/2.0f)+gameObject.transform.position.z)+moveList._attack[moveId].hitboxes[i].startingPoint.z), moveList._attack[0].hitboxes[i].extension, gameObject.transform.rotation, m_EnemyLayer));
        }

        foreach (Collider col in hitCollider)
        {
            if(!enemiesHit.Contains(col.gameObject.GetComponent<Enemy>())) enemiesHit.Add(col.gameObject.GetComponent<Enemy>());
        }

        foreach(Enemy en in enemiesHit)
        {
            en.TakeDamage(moveList._attack[moveId].damage);
        }

    }
    #endregion

    #region Gizmos
    //Desenhe o Box Overlap como um gizmo para mostrar onde ele está testando no momento
    void OnDrawGizmos()
    {
        if(Input.GetKey(lightAtk[0]) && !isValidBlow || Input.GetKey(lightAtk[1]) && !isValidBlow){
            for(int i = 0; i<moveList._attack[0].hitboxes.Length; i++){
                
                Gizmos.color = Color.blue;
                //Verifica se está rodando no modo Play , para não tentar desenhar isso no modo Editor
                if (m_Started)
                    Gizmos.DrawWireCube(new Vector3(gameObject.transform.position.x + moveList._attack[0].hitboxes[i].startingPoint.x, gameObject.transform.position.y + moveList._attack[0].hitboxes[i].startingPoint.y, (moveList._attack[0].hitboxes[i].extension.z/2.0f)+gameObject.transform.position.z+moveList._attack[0].hitboxes[i].startingPoint.z), moveList._attack[0].hitboxes[i].extension);
                
            }
        }

        if(Input.GetKey(heavyAtk[0]) && !isValidBlow || Input.GetKey(heavyAtk[1]) && !isValidBlow){
            for(int i = 0; i<moveList._attack[1].hitboxes.Length; i++){
                
                Gizmos.color = Color.red;
                if (m_Started)
                    Gizmos.DrawWireCube(new Vector3(gameObject.transform.position.x + moveList._attack[1].hitboxes[i].startingPoint.x, gameObject.transform.position.y + moveList._attack[1].hitboxes[i].startingPoint.y, (moveList._attack[1].hitboxes[i].extension.z/2.0f)+gameObject.transform.position.z+moveList._attack[1].hitboxes[i].startingPoint.z), moveList._attack[1].hitboxes[i].extension);
                
            }
        }
        
    }
    #endregion
}
