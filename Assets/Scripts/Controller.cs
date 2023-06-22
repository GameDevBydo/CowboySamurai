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

    public int money = 0;
    public TextMeshProUGUI moneyText;
    void Awake()
    {
        BasicSetup();
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
    public GameObject nextLevel;
    [Header("Telas")]
    public GameObject inGameScreen;
    public GameObject pauseScreen, gameOverScreen, shop, dialoguePanel;
    public TextMeshProUGUI comboCounter, comboComment, dialogueText;
    public CommentSO comments;
    
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

    public Image introImg;
    void ScrollingIntro()
    {
        introImg.material.mainTextureOffset += Vector2.right * (Time.deltaTime) * 0.03f;
    }


    void WriteText()
    {
        if(quoteToWrite.Length == currentLetter)
        {
            isWriting = false;
            Invoke("StopWrite", 3f);
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

    bool isWriting;
    string quoteToWrite;
    int currentLetter;
    float lettersCD;
    public void StartWriting(string sentence)
    {
        TogglePanel(dialoguePanel);
        dialogueText.text = "";
        quoteToWrite = sentence;
        currentLetter = 0;
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
    }

    void Start()
    {
        introImg.material.mainTextureOffset = Vector2.zero;
        introImg.material = new Material(introImg.material);
    }
    #endregion

    #region Scene and Application Management
    
        #region Scene Variables
        //Quantas cenas jogaveis existem para a randomização, e um rastreador da quantidade de cenas jogaveis.
        int playableScenes = 0; 
        public int currentScene = 0;
        //Lista das cenas randomizadas.
        int[] sceneList;
        // Seed atual
        string seed;
        public bool questClear = false;
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
        questClear = false;
        UpdateLifeBar((float)Player.instance.hitPoints/(float)Player.instance.maxHP);
        UpdateBulletBar(Player.instance.bulletBar);
        nextLevel.SetActive(false);
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
        enemiesInScene = 0;
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
        playerPause = false;
        LoadNextScene();
        ChangeScreen(screen);
        Player.instance.hitPoints = Player.instance.maxHP;
        Player.instance.bulletBar = 0;
        UpdateLifeBar(1);
        UpdateBulletBar(0);
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

    public void TogglePlayerPause()
    {
        playerPause = !playerPause;
        ChangeGameStates(playerPause?0:1);
        if(playerPause) ChangeScreen(pauseScreen);
        else ChangeScreen(inGameScreen);
    }
    #endregion

    #region Spawn Controller
    public Spawn[] spawns;
    public GameObject[] entities;
    bool spawnTimer;
    public int enemiesInScene;
    private float respawnTimer = 0.7f;
    private float timer;

    void SetSpawns()
    {
        spawns = new Spawn[transform.GetChild(2).childCount];
        for(int i = 0; i< transform.GetChild(2).childCount; i++)
        {
            spawns[i] = transform.GetChild(2).GetChild(i).GetComponent<Spawn>();
        }
    }

    public void StartSpawnEntities(int entityId, int quantity, int spawnId)
    {
        for(int i = 0; i < quantity; i++)
        {
            spawns[spawnId].SpawnEntity(entities[entityId]);
            enemiesInScene++;
        }
    }
    #endregion

    #region Death
        public void GameOver()
        {
            ChangeGameStates(0);
            ChangeScreen(gameOverScreen);
            inputPause = true;
        }
    #endregion

    void BasicSetup() // Coisas para acontecerem no inicio do jogo.
    {
        ChangeGameStates(0);
        inputPause = true;
        playerPause = false;
        comboCounter.text = "Combo: " + Convert.ToString(0);
        playableScenes = SceneManager.sceneCountInBuildSettings - 1; // Alterar o valor baseado em quantas cenas não jogáveis existem
        currentScene = 0;
        seed = null;
        currentScreen = GameObject.Find("Intro");
        enemiesInScene = 0;
        enemiesDefeated = 0;
        SetSpawns();
    }

    void Update()
    {
        
        moneyText.text = money.ToString();
        if(currentScene != 0){
            timer -= Time.deltaTime;
            if(timer <=0 && enemiesInScene<=10)
            {
                StartSpawnEntities(UnityEngine.Random.Range(0,entities.Length),1,UnityEngine.Random.Range(0,spawns.Length));
                timer = respawnTimer;
            }
        }
        ChangeLevel();
        ScrollingIntro();
        if(isWriting) WriteText();
    }

    
    #region Updating Stats
    public Image lifeBar;
    public void UpdateLifeBar(float fill)
    {
        lifeBar.fillAmount = fill;
    }
    [Header("Bullets")]
    public Image[] bulletIcons;
    public Color bulletColor, bulletMaxColor;
    public void UpdateBulletBar(float ammount)
    {
        bulletIcons[0].fillAmount = Mathf.Clamp(ammount/20f, 0,1);
        bulletIcons[1].fillAmount = Mathf.Clamp(ammount/20f-1, 0,1);
        bulletIcons[2].fillAmount = Mathf.Clamp(ammount/20f-2, 0,1);
        bulletIcons[3].fillAmount = Mathf.Clamp(ammount/20f-3, 0,1);
        bulletIcons[4].fillAmount = Mathf.Clamp(ammount/20f-4, 0,1);
        bulletIcons[5].fillAmount = Mathf.Clamp(ammount/20f-5, 0,1);
        foreach(Image bullet in bulletIcons)
        {
            if(bullet.fillAmount >=1) bullet.transform.GetComponent<Shadow>().enabled = true;
            else bullet.transform.GetComponent<Shadow>().enabled = false;
        }
        if(ammount >= Player.instance.bulletMax)
        {
            foreach(Image bullet in bulletIcons)
            {
                bullet.color = bulletMaxColor*10;
                bullet.transform.GetComponent<Shadow>().effectColor = bulletMaxColor *0.8f;
            }
        }
        else
        {
            foreach(Image bullet in bulletIcons)
            {
                bullet.color = bulletColor;
                bullet.transform.GetComponent<Shadow>().effectColor = bulletColor *0.8f;
            }
        }
    }
    #endregion

    #region Level Completion  (será trocado por sistema de quest)
    public int enemiesDefeated;
    public int ChangeScene;
    public void ClearLevel()
    {
    }

    public void ChangeLevel(){
        switch(currentScene){
            case 1:
                ChangeScene = 3;
                break;
            case 2:
                ChangeScene = 5;
                break;
            case 3:
                ChangeScene = 7;
                break;
            case 4:
                ChangeScene = 10;
                break;
            case 5:
                ChangeScene = 12;
                break;
            default:
                ChangeScene = 3;
                break;
        }
        if(enemiesDefeated>ChangeScene){
            questClear = true;
            nextLevel.SetActive(true);
            enemiesDefeated = 0;
        }
    }
    #endregion

    public void PauseFullGame(){
        inputPause = true;
    }
    public void UnPauseFullGame(){
        inputPause = false;
    }
}