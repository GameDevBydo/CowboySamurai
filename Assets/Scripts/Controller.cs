using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.InputSystem;
using UnityEngine.Video;


public class Controller : MonoBehaviour
{

    #region Singleton 
    [HideInInspector]
    public static Controller instance;

    public int money = 0;
    public TextMeshProUGUI moneyText;
    public GameObject skillTreePrefab;
    [SerializeField]
    private InputActionReference pause, skillTree;

    [HideInInspector]
    public AudioControlador audio;

    void Awake()
    {
        skillTreePrefab.SetActive(true);
        BasicSetup();
        
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);

        for (int i = 1; i < Player.instance.moveList.attackUnlocked.Length; i++)
        {
            Player.instance.moveList.attackUnlocked[i] = false;
        }

        audio = GetComponent<AudioControlador>();
        videoPlayer = GetComponent<VideoPlayer>();
    }
    #endregion 
    public Button continueBtn;
    void Start()
    {
        skillTreePrefab.SetActive(false);
        introImg.material.mainTextureOffset = Vector2.zero;
        introImg.material = new Material(introImg.material);
        playerBasePos = Player.instance.transform.position;
        playerBaseRot = Player.instance.transform.rotation;
        
    }

    
    void Update()
    {
        verifySaveGame();
        if(SceneManager.GetActiveScene().name != "TutorialScene")
            saveButton.interactable = true;
        if(pause.action.triggered && skillTreePanel.activeSelf == false){
            TogglePlayerPause(); // Pause funciona apenas no teclado por enquanto
            SetSelectedObj(firstBtnPause);
        } 
        if(skillTree.action.triggered){
            ChangeScreen(skillTreePanel);
            SetSelectedObj(NewSkillController.instance.unlockedSkills[0].gameObject);
            PauseFullGame();
        }
        moneyText.text = money.ToString();
        ScrollingIntro();
        if(isWriting) WriteText();
        //if(Input.GetKeyDown(KeyCode.N)) LoadNextScene();
    }

    public void StartGame(GameObject screen)
    {
        ChangeGameStates(1);
        inputPause = false; // Trocar para tocar pós animação
        playerPause = false;
        currentScene = 0;
        LoadScene(SceneManager.sceneCountInBuildSettings-2);
        ChangeScreen(screen);
        Player.instance.hitPoints = Player.instance.maxHP;
        Player.instance.bulletBar = 0;
        Player.instance.exp = 0;
        money = 0;
        UpdateLifeBar(1);
        UpdateBulletBar(0);
    }

    public void ContinueGame(string saveName)
    {
        ChangeGameStates(1);
        inputPause = false; // Trocar para tocar pós animação
        playerPause = false;
        ChangeScreen(inGameScreen);
        SaveController.Load(saveName);
        UpdateBulletBar(Player.instance.bulletBar);
        UpdateLifeBar((float)Player.instance.hitPoints/(float)Player.instance.maxHP);
        nextLevel.SetActive(false);
    }
    
    public Sprite gameSaved, slotEmpty;
    public TextMeshProUGUI[]  descLoad;
    public Image saveImage1, saveImage2, saveImage3;
    public Image loadImage1, loadImage2, loadImage3;
    public Button save1, save2, save3;
    public Button load1, load2, load3;
    [Header("Jogo Salvo")]
    public SpriteState spriteStateSaved;
    [Header("Slot Vazio")]
    public SpriteState spriteStateEmpty;

    public void verifySaveGame(){
        if(File.Exists(Application.persistentDataPath + "/SaveGame1.txt")){
            saveImage1.sprite = gameSaved;
            loadImage1.sprite = gameSaved;
            descLoad[0].text = File.GetCreationTime(Application.persistentDataPath + "/SaveGame1.txt").ToString();
            save1.interactable = true;
            save1.spriteState = spriteStateSaved;
            load1.spriteState = spriteStateSaved;
        }else{
            saveImage1.sprite = slotEmpty;
            loadImage1.sprite = slotEmpty;
            save1.spriteState = spriteStateEmpty;
            load1.spriteState = spriteStateEmpty;
            save1.interactable = false;
        }
        if(File.Exists(Application.persistentDataPath + "/SaveGame2.txt")){
            saveImage2.sprite = gameSaved;
            loadImage2.sprite = gameSaved;
            save2.interactable = true;
            descLoad[1].text = File.GetCreationTime(Application.persistentDataPath + "/SaveGame2.txt").ToString();
            save2.spriteState = spriteStateSaved;
            load2.spriteState = spriteStateSaved;
        }else{
            saveImage2.sprite = slotEmpty;
            loadImage2.sprite = slotEmpty;
            save2.spriteState = spriteStateEmpty;
            load2.spriteState = spriteStateEmpty;
            save2.interactable = false;
        }
        if(File.Exists(Application.persistentDataPath + "/SaveGame3.txt")){
            saveImage3.sprite = gameSaved;
            loadImage3.sprite = gameSaved;
            save3.interactable = true;
            descLoad[2].text = File.GetCreationTime(Application.persistentDataPath + "/SaveGame3.txt").ToString();
            save3.spriteState = spriteStateSaved;
            load3.spriteState = spriteStateSaved;
        }else{
            saveImage3.sprite = slotEmpty;
            loadImage3.sprite = slotEmpty;
            save3.spriteState = spriteStateEmpty;
            load3.spriteState = spriteStateEmpty;
            save3.interactable = false;
        }
        if(File.Exists(Application.persistentDataPath + "/SaveGame1.txt") || File.Exists(Application.persistentDataPath + "/SaveGame2.txt") || File.Exists(Application.persistentDataPath + "/SaveGame3.txt"))
            continueBtn.interactable = true;
        else
            continueBtn.interactable = false;
    }

    public void TiraPause(){
        Time.timeScale = 1;
    }
    public void CreditsScreen(GameObject screen){
        SceneManager.LoadScene("Creditos");
        ChangeScreen(screen);
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
    public GameObject firstBtnPause, firstBtnSkillTree, firstBtnShop, firstBtnTicket;

    public Button saveButton;

    
    // Usado em botões para trocar telas, como menu, opções, etc
    public void ChangeScreen(GameObject screen)
    {
        if(currentScreen!= null) currentScreen.SetActive(false);
        currentScreen = screen;
        currentScreen.SetActive(true);
    }
    GameObject auxScreen;
    // Usado em botões para abrir e fechar subjanelas
    public void TogglePanel(GameObject panel)
    {
        auxScreen = EventSystem.current.firstSelectedGameObject;
        //Debug.Log(auxScreen);
        panel.SetActive(!panel.activeSelf);      
    }

    public Image introImg;
    void ScrollingIntro()
    {
        introImg.material.mainTextureOffset += Vector2.right * (Time.deltaTime) * 0.03f;
    }

    public void SetSelectedObj(GameObject obj){
        EventSystem.current.SetSelectedGameObject(obj);
    }

    public GameObject firstBtnMenu;
    public void SetObjSettings(){
        if(SceneManager.GetActiveScene().name == "Menu"){
            EventSystem.current.SetSelectedGameObject(firstBtnMenu);
        }else{
            EventSystem.current.SetSelectedGameObject(firstBtnPause);
        }
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
        //public void LoadNextScene()
        //{
        //    if(playTutorial && currentScene == 0)
        //    {
        //        LoadScene(SceneManager.sceneCountInBuildSettings-2); // Lembrar de mudar para ser = a ultima cena que tem no projeto - 1
        //        WriteCurrentSceneText();
        //    }
        //}

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
        if(playerPause) 
        {
            Time.timeScale = 0;
            ChangeScreen(pauseScreen);
        }
        else 
        {
            Time.timeScale = 1;
            ChangeScreen(inGameScreen);
        }

    }

    public void closeShop(GameObject screen){
        screen.SetActive(false);
        TiraPause();
    }
    #endregion

    public void ToggleShop(){
        playerPause = !playerPause;
        ChangeGameStates(playerPause?0:1);
        if(playerPause) 
        {
            Time.timeScale = 0;
            ChangeScreen(skillTreePanel);
        }
        else 
        {
            Time.timeScale = 1;
            ChangeScreen(inGameScreen);
        }
    }

    #region Spawn Controller
    public GameObject[] entities;
    bool spawnTimer;
    public int enemiesInScene;
    private float respawnTimer = 0.7f;
    private float timer;

   
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
        //Player.instance.SetMaxMeter(0);
        inputPause = true;
        playerPause = false;
        comboCounter.text = "Combo: " + Convert.ToString(0);
        playableScenes = SceneManager.sceneCountInBuildSettings - 3; // Alterar o valor baseado em quantas cenas não jogáveis existem
        currentScene = 0;
        seed = null;
        currentScreen = GameObject.Find("Intro");
        enemiesInScene = 0;
        enemiesDefeated = 0;
        
    }

    #region Updating Stats
    public Image lifeBar;
    public GameObject hp_Background, hp_Fill, ui_Bullet;
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
    public int killsNeeded = 7;

    public void CheckClearCondition()
    {
        if(!questClear)
        {
            if(enemiesDefeated>=killsNeeded && currentScene!= 0)
            {
                questClear = true;
                ClearLevel();
            }
        }
    }

    public void ClearLevel()
    {
        questClear = true;
        nextLevel.SetActive(true);
        InstantiateTickets();
        GameObject.FindWithTag("EndLevel").GetComponent<BoxCollider>().enabled = true;
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

    

    #region Tickets
    [Header("Tickets")]
    public GameObject ticketCollectedIcon;
    public GameObject ticketScreen;
    public Image ticket1Sprite, ticket2Sprite; 
    private SpriteState ticket1High, ticket2High, ticket1Select, ticket2Select;
    public Button ticket1Button, ticket2Button;
    public Sprite ticketNoSprite;
    public TicketSO ticket1 = null, ticket2 = null;

    public List<TicketSO> ticketsAvailable;

    public void CollectTicket(TicketSO collected)
    {
        if(ticket1 == null) 
        {
            ticket1 = collected;
            UpdateTickets(0);
        }
        else if (ticket2 == null) 
        {
            ticket2 = collected;
            UpdateTickets(1);
        }
        else Debug.Log("Erro! Sem Espaço para tickets.");

        ticketCollectedIcon.SetActive(true);
    }

    public void UpdateTickets(int id)
    {
        if(id==0)
        {
            ticket1Sprite.sprite = ticket1.ticketSprite;
            ticket1High.highlightedSprite = ticket1.HighlightedSprite;
            ticket1High.selectedSprite = ticket1.SelectedSprite;
            ticket1Button.spriteState = ticket1High;
            ticket1Sprite.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ticket1.ticketName;
            ticket1Sprite.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = ticket1.ticketDescription;
        }
        else
        {
            ticket2Sprite.sprite = ticket2.ticketSprite;
            ticket2High.highlightedSprite = ticket2.HighlightedSprite;
            ticket2High.selectedSprite = ticket2.SelectedSprite;
            ticket2Button.spriteState = ticket2High;
            ticket2Sprite.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ticket2.ticketName;
            ticket2Sprite.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = ticket2.ticketDescription;
        }
        
    }

    void ClearTicketSprites()
    {
        ticket1Sprite.sprite = ticket1.ticketSprite;
        ticket1Sprite.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ticket1.ticketName;
        ticket1Sprite.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = ticket1.ticketDescription;

        ticket2Sprite.sprite = ticket2.ticketSprite;
        ticket2Sprite.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ticket2.ticketName;
        ticket2Sprite.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = ticket2.ticketDescription;
    }

    public void LoadSceneTicket(int id)
    {
        TicketSO ticketUsed = id==0 ? ticket1 : ticket2;
        SceneManager.LoadScene(ticketUsed.ticketLevel);
        currentScene++;
        ticketCollectedIcon.SetActive(false);
        ChangeScreen(inGameScreen);
        ticket1 = null;
        ticket2 = null;
        questClear = false;
        enemiesDefeated = 0;
        UpdateLifeBar((float)Player.instance.hitPoints/(float)Player.instance.maxHP);
        UpdateBulletBar(Player.instance.bulletBar);
        nextLevel.SetActive(false);
        killsNeeded = 5+currentScene*5;
        ticketsAvailable.Remove(ticketUsed);
    }

    [HideInInspector]
    public Transform tic1Pos, tic2Pos;
    public void InstantiateTickets()
    {
        TicketSO tic1, tic2;
        
        int tic1id=0, tic2id=0;
        if(ticketsAvailable.Count>=2)
        {
            while(tic1id==tic2id)
            {
                tic1id = UnityEngine.Random.Range(0, ticketsAvailable.Count-1);
                tic2id = UnityEngine.Random.Range(0, ticketsAvailable.Count-1);
            }
            if(currentScene>4)
            {
                tic1id = ticketsAvailable.Count-1;
            }

            tic1 = ticketsAvailable[tic1id];
            tic2 = ticketsAvailable[tic2id];

            Instantiate(tic1.ticketModel, tic1Pos.position, tic1Pos.rotation);
            Instantiate(tic2.ticketModel, tic2Pos.position, tic2Pos.rotation);    
        }
        else
        {
            tic1id = 0;
            tic1 = ticketsAvailable[tic1id];
            Instantiate(tic1.ticketModel, tic1Pos.position, tic1Pos.rotation);
        }        
    }

    #endregion


    
    VideoPlayer videoPlayer;
    public VideoClip openingVideo, endingVideo;
    public GameObject videoObj;

    public void PlayOpeningVideo()
    {
        audio.PauseSong();
        inputPause = true;
        videoPlayer.clip = openingVideo;
        videoObj.SetActive(true);
        videoPlayer.Play();
        Invoke("CloseVideo", (float)videoPlayer.clip.length);
    }

    public void PlayEndingVideo()
    {
        audio.PauseSong();
        inputPause = true;
        videoPlayer.clip = endingVideo;
        videoObj.SetActive(true);
        videoPlayer.Play();
        Invoke("CloseVideo", (float)videoPlayer.clip.length+1);
    }

    void CloseVideo()
    {
        audio.UnpauseSong();
        videoObj.SetActive(false);
        inputPause = false;
    }


}