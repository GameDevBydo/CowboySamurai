using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    #region Singleton 
    [HideInInspector]
    public static Controller instance;
    void Awake()
    {
        //Singleton básico, para evitar multiplos controllers na cena
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
    #endregion 

    #region UI Stuff (Tudo relacionado a UI)
    [HideInInspector]
    public GameObject currentScreen = null;
    
    // Usado em botões para trocar telas, como menu, opções, etc
    public void ChangeScreen(GameObject screen)
    {
        if(currentScreen!= null) currentScreen.SetActive(false);
        currentScreen = screen;
        currentScreen.SetActive(true);
    }

    // Usado em botões para abrir e fechar subjanelas
    public void TogglePanel(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }
    #endregion

    #region Scene and Application Management
    
    //Fechar o app
    public void ExitApp()
    {
        Application.Quit();
    }
    //Será editado para que funcione com o sistema de ssed, mas é a versão temporária
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    #endregion
}
