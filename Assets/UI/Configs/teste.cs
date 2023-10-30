using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class teste : MonoBehaviour
{
    public Image volumeBar;

    public void Test(){
        volumeBar.fillAmount += 0.249f;
        if(volumeBar.fillAmount >= 1.0f){
            volumeBar.fillAmount = 0;
        }
        Debug.Log(volumeBar.fillAmount);
    }
}
