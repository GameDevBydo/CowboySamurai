using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    bool m_Started;
    public LayerMask m_LayerMask;

    public Attack _attack;
    public bool isValidBlow;
    public KeyCode[] lightAtk, heavyAtk, specialAtk;


    void Start()
    {
        //Use this to ensure that the Gizmos are being drawn when in Play Mode.
        m_Started = true;

    }

    

    void FixedUpdate()
    {
        buttonsAttacks();
    }
    #region Buttons Attacks
    public void buttonsAttacks(){
        //Coloca a variavel hit como false
        _attack.hit = false;

        //if para botao de attack
        if(Input.GetKey(lightAtk[0]) && !isValidBlow){
            //chama cooutine para o freezetime do golpe
            StartCoroutine(timeout(_attack.freezeTime));
            //for para passar em todo vetor de hitbox
            for(int i = 0; i<_attack.hitboxes.Length; i++){
                //cria as caixas para fazer o desenho do golpe
                MyCollisions(_attack.hitboxes[i].startingPoint,_attack.hitboxes[i].extension, Quaternion.identity, _attack.damage[0]);
            } 
        }
        if(Input.GetKey(lightAtk[1]) && !isValidBlow){
            StartCoroutine(timeout(_attack.freezeTime));
            for(int i = 0; i<_attack.hitboxes.Length; i++){
                MyCollisions(_attack.hitboxes[i].startingPoint,_attack.hitboxes[i].extension, Quaternion.identity, _attack.damage[0]);
            } 
        }
        if(Input.GetKey(heavyAtk[0]) && !isValidBlow){
            StartCoroutine(timeout(_attack.freezeTime));
            for(int i = 0; i<_attack.hitboxes.Length; i++){
                MyCollisions(_attack.hitboxes[i].startingPoint,_attack.hitboxes[i].extension, Quaternion.identity, _attack.damage[0]);
            } 
        }
        if(Input.GetKey(heavyAtk[1]) && !isValidBlow){
            StartCoroutine(timeout(_attack.freezeTime));
            for(int i = 0; i<_attack.hitboxes.Length; i++){
                MyCollisions(_attack.hitboxes[i].startingPoint,_attack.hitboxes[i].extension, Quaternion.identity, _attack.damage[0]);
            } 
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
    void MyCollisions(Vector3 center,Vector3 halfExtents, Quaternion orientation, int damage)
    {
        //Isso cria uma caixa invisível ao redor do seu GameObject .
        Collider[] hitColliders = Physics.OverlapBox(new Vector3(gameObject.transform.position.x + center.x, gameObject.transform.position.y + center.y, ((halfExtents.z/2.0f)+gameObject.transform.position.z)+center.z), halfExtents, orientation, m_LayerMask);
        int i = 0;
        
        //Verifica quando há um novo colisor entrando em contato com a caixa
        while (i < hitColliders.Length)
        {
            //Coloca o hit do attack como verdadeiro
            _attack.hit = true;
            // Imprime todos os nomes dos colisores
            //Debug.Log("Hit : " + hitColliders[i].name + i);
            //Aumenta o número de Colliders no array
            i++;
            
        }
    }
    #endregion

    #region Gizmos
    //Desenhe o Box Overlap como um gizmo para mostrar onde ele está testando no momento
    void OnDrawGizmos()
    {
        if(Input.GetKey(lightAtk[0])){
            for(int i = 0; i<_attack.hitboxes.Length; i++){
                
                Gizmos.color = Color.red;
                //Verifica se está rodando no modo Play , para não tentar desenhar isso no modo Editor
                if (m_Started)
                    Gizmos.DrawWireCube(new Vector3(gameObject.transform.position.x + _attack.hitboxes[i].startingPoint.x, gameObject.transform.position.y + _attack.hitboxes[i].startingPoint.y, (_attack.hitboxes[i].extension.z/2.0f)+gameObject.transform.position.z+_attack.hitboxes[i].startingPoint.z), _attack.hitboxes[i].extension);
                
            }
        }
        if(Input.GetKey(lightAtk[1])){
            for(int i = 0; i<_attack.hitboxes.Length; i++){
                
                Gizmos.color = Color.red;
                if (m_Started)
                    Gizmos.DrawWireCube(new Vector3(gameObject.transform.position.x + _attack.hitboxes[i].startingPoint.x, gameObject.transform.position.y + _attack.hitboxes[i].startingPoint.y, (_attack.hitboxes[i].extension.z/2.0f)+gameObject.transform.position.z+_attack.hitboxes[i].startingPoint.z), _attack.hitboxes[i].extension);
                
            }
        }
        if(Input.GetKey(heavyAtk[0])){
            for(int i = 0; i<_attack.hitboxes.Length; i++){
                
                Gizmos.color = Color.red;
                if (m_Started)
                    Gizmos.DrawWireCube(new Vector3(gameObject.transform.position.x + _attack.hitboxes[i].startingPoint.x, gameObject.transform.position.y + _attack.hitboxes[i].startingPoint.y, (_attack.hitboxes[i].extension.z/2.0f)+gameObject.transform.position.z+_attack.hitboxes[i].startingPoint.z), _attack.hitboxes[i].extension);
                
            }
        }
        if(Input.GetKey(heavyAtk[1])){
            for(int i = 0; i<_attack.hitboxes.Length; i++){
                
                Gizmos.color = Color.red;
                if (m_Started)
                    Gizmos.DrawWireCube(new Vector3(gameObject.transform.position.x + _attack.hitboxes[i].startingPoint.x, gameObject.transform.position.y + _attack.hitboxes[i].startingPoint.y, (_attack.hitboxes[i].extension.z/2.0f)+gameObject.transform.position.z+_attack.hitboxes[i].startingPoint.z), _attack.hitboxes[i].extension);
                
            }
        }
    }
    #endregion
}
