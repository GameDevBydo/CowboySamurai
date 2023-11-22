using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISettingsControl : MonoBehaviour
{
    public GameObject volume, controles;
    public void ScreenVolume(){
        volume.SetActive(true);
        controles.SetActive(false);
    }
    public void ScreenControles(){
        volume.SetActive(false);
        controles.SetActive(true);
    }
}
