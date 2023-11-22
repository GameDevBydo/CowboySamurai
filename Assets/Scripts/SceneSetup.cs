using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    Controller main;

    public Transform tic1Pos, tic2Pos;
    void Awake()
    {
        main = Controller.instance;
    }

    void Start()
    {
       main.tic1Pos = tic1Pos;
       main.tic2Pos = tic2Pos;
    }

    public void NextScene()
    {
        if(main.questClear || main.currentScene == 0) 
        {
            if(main.currentScene == 0)
            {
                main.tutorialDone = true;
            }
            main.ChangeScreen(main.ticketScreen);
            main.SetSelectedObj(main.firstBtnTicket);
        }
    }

    public void EndGame()
    {
        main.ChangeScreen(main.endGameScreen);
        main.PauseFullGame();
    }
    

    public void UI_Outlines(int id)
    {
        main.ToggleOutline(id);
    }

    public void ClearLevel()
    {
        main.ClearLevel();
    }

}
