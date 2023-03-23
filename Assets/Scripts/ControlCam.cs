using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ControlCam : MonoBehaviour
{
    public CinemachineVirtualCamera cam;
    public GameObject plane,player;
    
    void Awake() 
    {
        plane = GameObject.Find("Plane");
        cam = GetComponent<CinemachineVirtualCamera>();
    }
    void Start()
    {
        player = GameObject.Find("Player_PlaceHolder");
        cam.Follow = player.transform;
        cam.LookAt = player.transform;
    }

    void Update()
    {
       Deadzone();

    }

    #region DEADZONE AO FINAL DA TELA
    public void Deadzone()
    {
        Vector3 startPlane,center,endPlane,size;
        CinemachineFramingTransposer ft = cam.GetCinemachineComponent<CinemachineFramingTransposer>();

        size = plane.GetComponent<MeshFilter>().mesh.bounds.size;
        Vector3 sizeScaled = Vector3.Scale(size,plane.transform.localScale);

        center = plane.GetComponent<MeshFilter>().mesh.bounds.center;

        startPlane = center - sizeScaled/2;
        endPlane = center + sizeScaled/2;

        if(player.transform.position.x > (startPlane.x + sizeScaled.x * 0.15f) && player.transform.position.x < (endPlane.x - sizeScaled.x * 0.15f ))
        {
            ft.m_DeadZoneWidth = 0f;
        }
        else{
            ft.m_DeadZoneWidth = 0.8f;
        }
    }

    #endregion
}
