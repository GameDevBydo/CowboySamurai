using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class BackMenu : MonoBehaviour
{
    public void Menu(){
        SceneManager.LoadScene("Menu");
    }

}
