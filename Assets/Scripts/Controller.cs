using System.Collections;
using System.Collections.Generic;
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
        public TextMeshProUGUI seedText, sceneCountText;
        #endregion

    //Fechar o app
    public void ExitApp()
    {
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
        LoadScene(0);
        currentScene = 0;
        WriteCurrentSceneText();
    }

    // Carrega a proxima cena da lista ja seedada
    public void LoadNextScene()
    {
        LoadScene(sceneList[currentScene]);
        currentScene++;
        WriteCurrentSceneText();
    }
    // Cria uma string baseada na quantidade de cenas jogaveis disponiveis e retorna ela
    public void CreateSeed()
    {   
        List<char> seedList = new List<char>();
        for(int i = 0; i <playableScenes; i++)
        {
            char id = ((char)Random.Range(97,97+playableScenes));
            while(seedList.Contains(id))
            {
                id = ((char)Random.Range(97,97+playableScenes));
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

        #region Teste
    public void CheckSeedIntegrity(string writtenSeed)
    {
        if(writtenSeed.Length != playableScenes) {CreateSeed(); return;}
    }

    public void InputSeed(string s)
    {
        seed = s.ToLower();
        WriteSeedText();
    }

    public void WriteSeedText()
    {
        seedText.text = "Seed: "+seed;
    }

    public void WriteCurrentSceneText()
    {
        sceneCountText.text = "Cena:" +currentScene.ToString();
    }
    #endregion

    void BasicSetup() // Coisas para acontecerem no inicio do jogo.
    {
        playableScenes = SceneManager.sceneCountInBuildSettings - 1; // Alterar o valor baseado em quantas cenas não jogáveis existem
        currentScene = 0;
        seed = null;
    }
    #endregion
    void Start()
    {
        BasicSetup();
    }
}
