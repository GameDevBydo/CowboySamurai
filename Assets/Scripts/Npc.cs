using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Npc : MonoBehaviour
{
    public GameObject prefab;
    public float distance;
    private GameObject player;
    private float dist;
    public int npcType; // 0 para vendedor (abre uma loja), 1 para texto
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    [TextArea(2,4)]
    public string[] quotes;
    public UnityEvent[] endEvent;

    bool isWriting;
    string quoteToWrite;
    string[] allQuotes;
    int currentLetter, currentSentence, currentEvent;
    float lettersCD, nextQuoteCD;

    UnityEvent endQuoteEvent;
    UnityEvent[] allQuoteEvents;
    
    // Start is called before the first frame update
    void Start()
    {
       player = Player.instance.gameObject; 
    }

    // Update is called once per frame
    void Update()
    {
        dist = Vector3.Distance(transform.position, Player.instance.transform.position);
        if(dist <= distance)
        {
            if(!isWriting) prefab.SetActive(true);
            else prefab.SetActive(false);

            if(!isWriting)
            {
                if(Input.GetKeyDown(Player.instance.interactKey[0]) || Input.GetKeyDown(Player.instance.interactKey[1]))
                {
                    if(npcType == 0)
                    {
                        UI.instance.shop.SetActive(true);
                        Controller.instance.PauseFullGame();
                    }
                    if(npcType == 1)
                    {
                        StartWriting(quotes, endEvent);
                    }
                }
            }
        }
        else
        {
            prefab.SetActive(false);
        }
        if(isWriting) WriteText();
    }
    
    void WriteText()
    {
        Debug.Log(currentSentence);
        Debug.Log(allQuotes.Length-1);
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
                    quoteToWrite = allQuotes[currentSentence];
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
        Debug.Log(dialoguePanel.name);
        dialoguePanel.SetActive(true);
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
        //inputPause = true;
    }

    void StopWrite()
    {
        isWriting = false;
        //inputPause = false;
        dialoguePanel.SetActive(false);
        endQuoteEvent.Invoke();
    }
}

