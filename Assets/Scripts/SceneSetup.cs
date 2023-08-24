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
        GenerateTickets(main.ticket1, main.ticket2);
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

    public void GenerateTickets(TicketSO tic1, TicketSO tic2)
    {
        Instantiate(tic1.ticketModel, tic1Pos.position, tic1Pos.rotation);
        Instantiate(tic2.ticketModel, tic2Pos.position, tic2Pos.rotation);
    }


}
