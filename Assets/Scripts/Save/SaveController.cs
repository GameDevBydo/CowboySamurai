using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveController : MonoBehaviour
{
    //realiza o save do jogo em um arquivo Json
    public static void Save(){
        //Criação das variaveis para usar as classes criadas para o salvamento da cena e das variaveis do jogo
        SceneData scene = new SceneData();
        scene.game = new GameSaveData();

        

        //Gravação dos dados no arquivo Json
        string s = JsonUtility.ToJson(scene);
        File.WriteAllText(Application.persistentDataPath + "/saveGame.txt", s);
    }

    public static void Load(){
        //leitura do arquivo Json
        string s = File.ReadAllText(Application.persistentDataPath + "/saveGame.txt");

        SceneData data = JsonUtility.FromJson<SceneData>(s);
        
    }
}
