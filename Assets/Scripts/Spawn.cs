using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    
    bool canSpawn;
    float timer = 1.0f;
    void Update()
    {
        Deadzone();
    }
    public void SpawnEntity(GameObject obj)
    {
        if(canSpawn)
            Instantiate(obj, new Vector3(transform.position.x, 0.0f, transform.position.z), Quaternion.identity);
    }

    public void Deadzone(){
        if(Controller.instance.currentScene != 0){
            timer -= Time.deltaTime;
            if(timer <=0)
            {
                if(transform.position.x < (ControlCam.instance.startPlane.x + ControlCam.instance.size.x * 0.12f) || transform.position.x > (ControlCam.instance.endPlane.x - ControlCam.instance.size.x * 0.12f )){
                    canSpawn = false;
                }else{
                    canSpawn = true;
                }
            }
        }
    }

}
