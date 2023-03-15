using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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
    
        #region Scene Variables
        //Quantas cenas jogaveis existem para a randomização, e um rastreador da quantidade de cenas jogaveis.
        int playableScenes = 0, currentScene = 0;
        //Lista das cenas randomizadas.
        int[] sceneList;
        // Seed atual
        string seed;
        #endregion

        #region Scene References
        public TextMeshProUGUI[] seedText, sceneCountText;
        #endregion

    //Fechar o app
    public void ExitApp()
    {
        Debug.Log("<color=red>Fechou o jogo.</color>");
        Application.Quit();
    }
    // Carrega uma cena com id especificado.
    public void LoadScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }

    // Carrega o menu, e altera os trem q mudar
    public void LoadMenu()
    {
        ChangeGameStates(0);
        LoadScene(0);
        currentScene = 0;
        WriteCurrentSceneText();
        inputPause = true;
    }

    // Carrega a proxima cena da lista ja seedada
    public void LoadNextScene()
    {
        LoadScene(sceneList[currentScene]);
        currentScene++;
        WriteCurrentSceneText();
    }

    public void StartGame(GameObject screen)
    {
        if(seed == null)
        {
            ConvertSeedIntoList();
        }
        ChangeGameStates(1);
        inputPause = false; // Trocar para tocar pós animação
        LoadNextScene();
        ChangeScreen(screen);
    }

        #region Coisas de Seed 
        public void CreateSeed() // Cria uma seed aleatória em formato de string baseado em quantas cenas jogaveis existem
        {   
            List<char> seedList = new List<char>();
            for(int i = 0; i <playableScenes; i++)
            {
                char id = ((char)UnityEngine.Random.Range(97,97+playableScenes));
                while(seedList.Contains(id))
                {
                    id = ((char)UnityEngine.Random.Range(97,97+playableScenes));
                }
                seedList.Add(id);
            }
            seed = string.Concat(seedList);
            WriteSeedText();
        }

        // Le uma seed e preenche a lista de fases na ordem correta.
        public void ConvertSeedIntoList()
        {
            if(seed==null) CreateSeed();
            string seedName = seed;
            sceneList = new int[playableScenes];
            char[] seedSplit = seedName.ToCharArray();
            for(int i = 0; i < seedName.Length; i++)
            {
                sceneList[i] = (((byte)seedSplit[i])-97)+1;
            }
        }

        //Confere se a seed é válida comparando o tamanho dela, e os caracteres presentes
        public void CheckSeedIntegrity(string writtenSeed) 
        {
            bool flag= false;
            if(writtenSeed.Length != playableScenes) flag = true;
            for(int i = 0; i < playableScenes; i++)
            {
                if(!writtenSeed.Contains(Convert.ToChar(97+i))) flag = true;
            }
            if(flag) 
            {
                Debug.Log("Seed inválida.");
                //GameObject.Find("SeedError").SetActive(true);         Trocar função para possibilizar encontrar objetos desativados.
                CreateSeed();
            }
            else
            {
                ///GameObject.Find("SeedError").SetActive(false);
                seed = writtenSeed;
            }
        }

        // Função para ser chamada e pegar o nome da seed
        public void InputSeed(string s)
        {
            CheckSeedIntegrity(s.ToLower());
            WriteSeedText();
        }

        // Transforma a seed em texto visível para o jogador
        public void WriteSeedText()
        {
            for(int i = 0; i <seedText.Length; i++)
            {
                seedText[i].text = "Seed: "+seed;
            }
        }

        // Escreve para o player qual a cena (quantitativamente) atual ele está jogando (1ª cena, 2ª cena, etc)
        public void WriteCurrentSceneText()
        {
            for(int i = 0; i <sceneCountText.Length; i++)
            {
                sceneCountText[i].text = "Cena: " +currentScene.ToString();
            }
        }
        #endregion
    #endregion

    #region Game States
    public enum States 
    {
        UI,
        Game,
    }
    public States states;
    public void ChangeGameStates(int stateID)
    {
        switch(stateID)
        {
            case 0: states = States.UI;
            break;
            case 1: states = States.Game;
            break;

            default: Debug.Log("<color=red>Estado não inserido. Estado se tornou UI.</color>");
            states = States.UI;
            break;
        }
    }
    public bool inputPause, playerPause;
    #endregion
    void BasicSetup() // Coisas para acontecerem no inicio do jogo.
    {
        ChangeGameStates(0);
        inputPause = true;
        playerPause = false;
        playableScenes = SceneManager.sceneCountInBuildSettings - 1; // Alterar o valor baseado em quantas cenas não jogáveis existem
        currentScene = 0;
        seed = null;
        currentScreen = GameObject.Find("Intro");
    }
    void Start()
    {
        BasicSetup();   
    }
}