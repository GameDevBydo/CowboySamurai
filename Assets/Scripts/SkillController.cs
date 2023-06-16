using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillController : MonoBehaviour
{
    public static SkillController instance;
    public Skill [] skills;
    
    public float exp;
    public TextMeshProUGUI skillname,desc,expUp,skillPrice;
    public Image skillsprite;

    public Sprite disabled, selectable, activated;


    void Start()
    {
        instance = this;
    
        for (int i = 1; i < Player.instance.moveList.attackUnlocked.Length; i++)
        {
            Player.instance.moveList.attackUnlocked[i] = false;
        }
        for(int i = 0; i< skills.Length; i++)
        {
            skills[i].GetComponent<Image>().sprite = disabled;
            Debug.Log("passei");
        }
        UnlockSKill(0);
        
        //Aqui seria o save para desbloquear as skills

        // Rodar um for, usando o UnlockSkill para ativar as skills salvas no save do player.
    }

    void Update()
    {
        expUp.text = "EXP: " + exp;
    }

    void EnableNextSkill(int id)
    {
        for(int i = 0; i< skills[id].nextSkills.Length; i++)
        {
            skills[id].nextSkills[i].GetComponent<Button>().interactable = true;
            skills[id].nextSkills[i].GetComponent<Image>().sprite = selectable;
        }
    }


    public void BuySkill(int id) // Função para p player usar ingame
    {
        if(exp >= skills[id].price && skills[id].skillUnlocked == false)
        {
            UnlockSKill(id);
            exp -= skills[id].price;
        }
    }

    void UnlockSKill(int id) // Função para usar em dev mode
    {
        skills[id].GetComponent<Button>().interactable = false;
        skills[id].GetComponent<Image>().sprite = activated;
        Player.instance.moveList.attackUnlocked[id] = true;
        skills[id].skillUnlocked = true;
        EnableNextSkill(id);
    }
}


