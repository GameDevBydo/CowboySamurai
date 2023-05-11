using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ControlCam : MonoBehaviour
{
    public static ControlCam instance;
    public CinemachineVirtualCamera cam;
    public GameObject plane,player,preFabPlayer;
    
    public Vector3 startPlane,center,endPlane,size;
    CinemachineFramingTransposer ft;

    void Awake() 
    {
        
        instance = this;
        plane = GameObject.Find("Plane");
        cam = GetComponent<CinemachineVirtualCamera>();
        DefineDeadzone();
        PositionPlayer();
        player = GameObject.FindWithTag("Player");
    }
    void Start()
    {
        ft = cam.GetCinemachineComponent<CinemachineFramingTransposer>();
        cam.Follow = player.transform;
        cam.LookAt = player.transform;
    }

    void Update()
    {
        Deadzone();
    }

    void PositionPlayer(){

        Instantiate(preFabPlayer, new Vector3(startPlane.x + size.x * 0.12f,0,0),Quaternion.identity);
        
    
    }


    #region DEADZONE AO FINAL DA TELA
    public void DefineDeadzone()
    {
        size = Vector3.Scale(plane.GetComponent<MeshFilter>().mesh.bounds.size, plane.transform.localScale);
        center = plane.GetComponent<MeshFilter>().mesh.bounds.center;

        startPlane = center - size/2;
        endPlane = center + size/2;
    }

    void Deadzone()
    {
        if(player.transform.position.x > (startPlane.x + size.x * 0.12f) && player.transform.position.x < (endPlane.x - size.x * 0.12f ))
        {
            ft.m_DeadZoneWidth = 0f;
        }
        else{
            ft.m_DeadZoneWidth = 1.2f;
        }
    }
    #endregion
}
