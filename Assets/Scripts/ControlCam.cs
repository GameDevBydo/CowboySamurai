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
        player = Player.instance.gameObject;
    }
    void Start()
    {
        PositionPlayer();
        ft = cam.GetCinemachineComponent<CinemachineFramingTransposer>();
        cam.Follow = player.transform;
        cam.LookAt = player.transform;
    }

    void Update()
    {
        Deadzone();
    }

    void PositionPlayer(){
        //Debug.Log(new Vector3(startPlane.x + size.x * 0.12f,0,0));
        Player.instance.controller.enabled = false;
        Player.instance.transform.position = new Vector3(plane.transform.position.x + startPlane.x + size.x * 0.15f,0,0);
        Player.instance.transform.rotation = Quaternion.Euler(0,90,0);
        Player.instance.controller.enabled = true;
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
            ft.m_DeadZoneWidth = 0.8f;
        }
    }
    #endregion
}
