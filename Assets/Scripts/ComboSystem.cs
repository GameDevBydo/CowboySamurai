using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    bool m_Started;
    public LayerMask m_LayerMask;

    public Attack _attack;
    public bool isValid;

    public KeyCode[] lightAtk, heavyAtk, specialAtk;

    void Start()
    {
        //Use this to ensure that the Gizmos are being drawn when in Play Mode.
        m_Started = true;

    }

    

    void FixedUpdate()
    {
        _attack.hit = false;

        
        if(Input.GetKey(KeyCode.JoystickButton2) && !isValid){
            StartCoroutine(timeout(_attack.startUp));
            for(int i = 0; i<_attack.hitboxes.Length; i++){
                MyCollisions(_attack.hitboxes[i].startingPoint,_attack.hitboxes[i].extension, Quaternion.identity, _attack.damage[0]);
            } 
        }

        if(Input.GetKey(KeyCode.P) && !isValid){
            StartCoroutine(timeout(_attack.startUp));
            for(int i = 0; i<_attack.hitboxes.Length; i++){
                MyCollisions(_attack.hitboxes[i].startingPoint,_attack.hitboxes[i].extension, Quaternion.identity, _attack.damage[0]);
            } 
        }
        
    }

    IEnumerator timeout(float value){
        isValid = true;
        yield return new WaitForSeconds(value);
        isValid = !isValid;
    }
    

    void MyCollisions(Vector3 center,Vector3 halfExtents, Quaternion orientation, int damage)
    {
        //Use the OverlapBox to detect if there are any other colliders within this box area.
        //Use the GameObject's centre, half the size (as a radius) and rotation. This creates an invisible box around your GameObject.
        Collider[] hitColliders = Physics.OverlapBox(new Vector3(gameObject.transform.position.x + center.x, gameObject.transform.position.y + center.y, ((halfExtents.z/2.0f)+gameObject.transform.position.z)+center.z), halfExtents, orientation, m_LayerMask);
        int i = 0;
        
        //Check when there is a new collider coming into contact with the box
        while (i < hitColliders.Length)
        {
            
            _attack.hit = true;
            //Output all of the collider nam
            Debug.Log("Hit : " + hitColliders[i].name + i);
            //Increase the number of Colliders in the array
            i++;
            
        }
    }

    //Draw the Box Overlap as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        if(Input.GetKey(KeyCode.P)){
            for(int i = 0; i<_attack.hitboxes.Length; i++){
                
                Gizmos.color = Color.red;
                //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
                if (m_Started)
                    //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
                    Gizmos.DrawWireCube(new Vector3(gameObject.transform.position.x + _attack.hitboxes[i].startingPoint.x, gameObject.transform.position.y + _attack.hitboxes[i].startingPoint.y, (_attack.hitboxes[i].extension.z/2.0f)+gameObject.transform.position.z+_attack.hitboxes[i].startingPoint.z), _attack.hitboxes[i].extension);
                
            }
           
            
        }
    }
}
