using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UI : MonoBehaviour
{
    [HideInInspector]
    public GameObject currentScreen = null;
    public GameObject nextLevel;
    [Header("Telas")]
    public GameObject inGameScreen;
    public GameObject pauseScreen, gameOverScreen, shop, dialoguePanel, endGameScreen, skillTreePanel;
    public TextMeshProUGUI comboCounter, comboComment, dialogueText;
    public CommentSO comments;

    public bool inputPause = false;
    public bool playerPause = false;

    #region Singleton 
    [HideInInspector]
    public static UI instance;  
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
    public void Start()
    {
        
    }
    public void StartGame(GameObject screen)
    {
        inputPause = false; // Trocar para tocar pós animação
        playerPause = false;
        ChangeScreen(screen);
    }
    
    // Usado em botões para trocar telas, como menu, opções, etc
    public void ChangeScreen(GameObject screen)
    {
        if(currentScreen!= null) currentScreen.SetActive(false);
        currentScreen = screen; 
        currentScreen.SetActive(true);
        if(screen.name != "InGame"){
            //EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(screen.transform.GetChild(1).gameObject);
        }
    }
    
    public GameObject auxScreen;
    // Usado em botões para abrir e fechar subjanelas
    public void TogglePanel(GameObject panel)
    {
        auxScreen = EventSystem.current.firstSelectedGameObject;
        Debug.Log(auxScreen);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(panel.transform.GetChild(1).gameObject);
        panel.SetActive(!panel.activeSelf);
        if(panel.activeSelf == false)
        EventSystem.current.SetSelectedGameObject(auxScreen);        
    }

    public Image introImg;
    public void ScrollingIntro()
    {
        introImg.material.mainTextureOffset += Vector2.right * (Time.deltaTime) * 0.03f;
    }


    #region Writing

    public bool isWriting;
    string quoteToWrite;
    string[] allQuotes;
    int currentLetter, currentSentence, currentEvent;
    float lettersCD, nextQuoteCD;

    UnityEvent endQuoteEvent;
    UnityEvent[] allQuoteEvents;
    public void WriteText()
    {
        if(quoteToWrite.Length == currentLetter)
        {
            if(currentSentence == allQuotes.Length-1)
            {
                isWriting = false;
                Invoke("StopWrite", 3f);
            }
            else
            {
                if(nextQuoteCD<=0)
                {
                    endQuoteEvent.Invoke();
                    currentSentence++;
                    currentEvent++;
                    currentLetter = 0;
                    dialogueText.text = "";
                    nextQuoteCD = 3;
                    endQuoteEvent = allQuoteEvents[currentEvent];
                }
                else
                {
                    nextQuoteCD-=Time.deltaTime;
                }
            }
        }
        else if (lettersCD<=0)
        {
            dialogueText.text+= quoteToWrite[currentLetter];
            if(quoteToWrite[currentLetter] == ' ') lettersCD = 0f;
            else lettersCD = 0.01f;
            currentLetter++;
        }
        else lettersCD-=Time.deltaTime;
    }

    public void StartWriting(string[] sentences, UnityEvent[] endEvent)
    {
        TogglePanel(dialoguePanel);
        dialogueText.text = "";
        currentSentence = 0;
        allQuotes = sentences;
        quoteToWrite = allQuotes[currentSentence];
        currentLetter = 0;
        nextQuoteCD = 3;
        allQuoteEvents = endEvent;
        endQuoteEvent = allQuoteEvents[currentEvent];
        Invoke("BeginWrite", 1);
    }

    void BeginWrite()
    {
        isWriting = true;
        inputPause = true;
    }

    void StopWrite()
    {
        isWriting = false;
        inputPause = false;
        TogglePanel(dialoguePanel);
        endQuoteEvent.Invoke();
    }
    #endregion
}