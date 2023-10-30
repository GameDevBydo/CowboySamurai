using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEngine.SceneManagement;

public class SaveController : MonoBehaviour
{

    //realiza o save do jogo em um arquivo Json
    public static void Save(string name){
        //Criação das variaveis para usar as classes criadas para o salvamento da cena e das variaveis do jogo
        SceneData scene = new SceneData();
        scene.game = new GameSaveData();

        scene.game.skills = NewSkillController.instance.skills;
        scene.game.unlockedSkills = NewSkillController.instance.unlockedSkills;
        scene.game.lostSkills = NewSkillController.instance.lostSkills;
        scene.game.acquiredSkills = NewSkillController.instance.acquiredSkills;

        scene.game.life = Player.instance.hitPoints;
        scene.game.maxHP = Player.instance.maxHP;
        scene.game.money = Controller.instance.money;
        scene.game.xp = Player.instance.exp;
        scene.game.bulletBar = Player.instance.bulletBar;

        scene.game.currentscene = Controller.instance.currentScene;
        scene.game.ticketsAvailable = Controller.instance.ticketsAvailable;
        scene.game.nameScene = SceneManager.GetActiveScene().name;
        

        //Gravação dos dados no arquivo Json
        string s = JsonUtility.ToJson(scene);
        File.WriteAllText(Application.persistentDataPath + "/"+ name+".txt", s);
        Debug.Log("Salvou");
    }

    public static void Load(string name){
        //leitura do arquivo Json

        string s = File.ReadAllText(Application.persistentDataPath + "/"+ name+".txt");
        SceneData data = JsonUtility.FromJson<SceneData>(s);

        SceneManager.LoadScene(data.game.nameScene);

        NewSkillController.instance.skills = data.game.skills;
        NewSkillController.instance.unlockedSkills = data.game.unlockedSkills;
        NewSkillController.instance.lostSkills = data.game.lostSkills;
        NewSkillController.instance.acquiredSkills = data.game.acquiredSkills;

        Player.instance.hitPoints = data.game.life;
        Player.instance.maxHP = data.game.maxHP;
        Controller.instance.money = data.game.money;
        Player.instance.exp = data.game.xp;
        Player.instance.bulletBar = data.game.bulletBar;
        
        
        Controller.instance.currentScene = data.game.currentscene;
        Controller.instance.ticketsAvailable = data.game.ticketsAvailable;
    }

        //realiza o save do jogo em um arquivo Json
    public static void SaveConfigs(){
        //Criação das variaveis para usar as classes criadas para o salvamento da cena e das variaveis do jogo
        SceneData scene = new SceneData();
        scene.menu = new MenuSaveData();
        
        scene.menu.volumeGeral = AudioControlador.instance.masterVolBar.fillAmount;
        scene.menu.soundsEffects = AudioControlador.instance.sfxBar.fillAmount;
        scene.menu.music = AudioControlador.instance.BMGBar.fillAmount;

        //Gravação dos dados no arquivo Json
        string s = JsonUtility.ToJson(scene);
        File.WriteAllText(Application.persistentDataPath + "/SaveConfigs.txt", s);
        
    }

    public static void LoadConfigs(){
        //leitura do arquivo Json
        string s = File.ReadAllText(Application.persistentDataPath + "/SaveConfigs.txt");
        SceneData data = JsonUtility.FromJson<SceneData>(s);
        
        AudioControlador.instance.masterVolBar.fillAmount = data.menu.volumeGeral;
        AudioControlador.instance.sfxBar.fillAmount = data.menu.soundsEffects;
        AudioControlador.instance.BMGBar.fillAmount = data.menu.music;
    }
}
