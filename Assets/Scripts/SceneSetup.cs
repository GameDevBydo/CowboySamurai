using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    Controller main;
    void Awake()
    {
        main = Controller.instance;
    }

    public void NextScene()
    {
        if(main.questClear || main.currentScene == 0) 
        {
            if(main.currentScene == 0)
            {
                main.tutorialDone = true;
            }
            main.LoadNextScene();
        }
    }

    public void EndGame()
    {
        UI.instance.ChangeScreen(UI.instance.endGameScreen);
        Controller.instance.PauseFullGame();
    }
    

    public void UI_Outlines(int id)
    {
        main.ToggleOutline(id);
    }
}
