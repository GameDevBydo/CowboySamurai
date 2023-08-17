using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;


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

    void Start()
    {
        introImg.material.mainTextureOffset = Vector2.zero;
        introImg.material = new Material(introImg.material);
        playerBasePos = Player.instance.transform.position;
        playerBaseRot = Player.instance.transform.rotation;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7)){
            TogglePlayerPause(); // Pause funciona apenas no teclado por enquanto
        } 
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
        if(Input.GetKeyDown(KeyCode.N)) LoadNextScene();
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
        Player.instance.exp = 0;
        money = 0;
        UpdateLifeBar(1);
        UpdateBulletBar(0);
    }

    #region UI Stuff (Tudo relacionado a UI)
    [HideInInspector]
    public GameObject currentScreen = null;
    public GameObject nextLevel;
    [Header("Telas")]
    public GameObject inGameScreen;
    public GameObject pauseScreen, gameOverScreen, shop, dialoguePanel, endGameScreen, skillTreePanel;
    public TextMeshProUGUI comboCounter, comboComment, dialogueText;
    public CommentSO comments;

    
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
    GameObject auxScreen;
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
    void ScrollingIntro()
    {
        introImg.material.mainTextureOffset += Vector2.right * (Time.deltaTime) * 0.03f;
    }


    void WriteText()
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

    bool isWriting;
    string quoteToWrite;
    string[] allQuotes;
    int currentLetter, currentSentence, currentEvent;
    float lettersCD, nextQuoteCD;

    UnityEvent endQuoteEvent;
    UnityEvent[] allQuoteEvents;
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
        public bool playTutorial = true, tutorialDone = false;
        public void ToggleTutorial(bool value)
        {
            playTutorial = value;
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
            ResetPlayerPosition();
            WriteCurrentSceneText();
            inputPause = true;
        }
        // Carrega a proxima cena da lista ja seedada
        public void LoadNextScene()
        {
            if(playTutorial && currentScene == 0 && !tutorialDone)
            {
                LoadScene(SceneManager.sceneCountInBuildSettings-2); // Lembrar de mudar para ser = a ultima cena que tem no projeto - 1
                spawns[0].allowSpawn = false;
                spawns[1].allowSpawn = false;
                enemiesInScene = 0;
                WriteCurrentSceneText();
            }
            else if(currentScene != playableScenes)
            {
                LoadScene(sceneList[currentScene]);
                spawns[0].allowSpawn = true;
                spawns[1].allowSpawn = true;
                enemiesInScene = 0;
                currentScene++;
                WriteCurrentSceneText();
            }
            else
            {
                LoadScene(SceneManager.sceneCountInBuildSettings-1); // Lembrar de mudar para ser = a ultima cena que tem no projeto
                spawns[0].allowSpawn = false;
                spawns[1].allowSpawn = false;
                enemiesInScene = 0;
                currentScene++;
                WriteCurrentSceneText();
            }
        }
    #endregion

    #region Player
        private Vector3 playerBasePos;
        private Quaternion playerBaseRot;
        void ResetPlayerPosition()
        {
            Player.instance.controller.enabled = false;
            Player.instance.transform.position = playerBasePos;
            Player.instance.transform.rotation = playerBaseRot;
            Player.instance.controller.enabled = true;
        }
    #endregion

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
    public GameObject FirstButtonPause, FirstButtonSkill;

    public void TogglePlayerPause()
    {
        playerPause = !playerPause;
        ChangeGameStates(playerPause?0:1);
        if(playerPause) ChangeScreen(pauseScreen);
        else ChangeScreen(inGameScreen);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(FirstButtonPause);
    }
    #endregion

    public void ToggleShop(){
        playerPause = !playerPause;
        ChangeGameStates(playerPause?0:1);
        if(playerPause) ChangeScreen(skillTreePanel);
        else ChangeScreen(inGameScreen);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(FirstButtonSkill);
    }

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
        playableScenes = SceneManager.sceneCountInBuildSettings - 3; // Alterar o valor baseado em quantas cenas não jogáveis existem
        currentScene = 0;
        seed = null;
        currentScreen = GameObject.Find("Intro");
        enemiesInScene = 0;
        enemiesDefeated = 0;
        SetSpawns();
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
                ChangeScene = 10;
                break;
            case 2:
                ChangeScene = 12;
                break;
            case 3:
                ChangeScene = 15;
                break;
            case 4:
                ChangeScene = 20;
                break;
            case 5:
                ChangeScene = 25;
                break;
            default:
                ChangeScene = 10;
                break;
        }
        if(enemiesDefeated>ChangeScene && currentScene!= 0){
            questClear = true;
            nextLevel.SetActive(true);
            enemiesDefeated = 0;
        }
    }
    #endregion

    public void PauseFullGame(){
        inputPause = true;
        states = States.UI;
    }
    public void UnPauseFullGame(){
        inputPause = false;
        states = States.Game;
    }

    public GameObject[] outlines;
    public void ToggleOutline(int id)
    {
        outlines[id].SetActive(!outlines[id].activeSelf);
    }

    
}