using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class BackMenu : MonoBehaviour
{
    public GameObject eventSystem, firstObj;
    private void Start() {
        eventSystem = GameObject.FindGameObjectWithTag("EV");
        eventSystem.GetComponent<EventSystem>().firstSelectedGameObject = firstObj;
    }
    public void Menu(){
        SceneManager.LoadScene("Menu");
    }

}
