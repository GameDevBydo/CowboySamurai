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
    public TextMeshProUGUI skillname,desc,expUp;
    public Image skillsprite;

    


    void Start()
    {
        instance = this;
    }

    void Update()
    {
        ControllerUnlocked(); 
        expUp.text = "EXP: " + exp;
    }


    // Controle de skills desbloqueadas e interatividade dos botões
    public void ControllerUnlocked ()
    {

        if(exp >= 1f && exp < 2)
        {
            skills[0].GetComponent<Button>().interactable = true;
            skills[0].skillUnlocked = true;
        }
        else if(exp >= 2f && exp < 3 && skills[1].GetComponent<Skill>().previousSkill[0].skillUnlocked == true)
        {
            skills[1].GetComponent<Button>().interactable = true;
            skills[1].skillUnlocked = true;
        }
        else if(exp >= 3f && exp < 4 && skills[2].GetComponent<Skill>().previousSkill[0].skillUnlocked == true)
        {
            skills[2].GetComponent<Button>().interactable = true;
            skills[2].skillUnlocked = true;
        }
        else if(exp >= 4f && exp < 5 && skills[3].GetComponent<Skill>().previousSkill[0].skillUnlocked == true)
        {
            skills[3].GetComponent<Button>().interactable = true;
            skills[3].skillUnlocked = true;
        }
        else if(exp >= 5f && exp < 6 && skills[4].GetComponent<Skill>().previousSkill[0].skillUnlocked == true)
        {
            skills[4].GetComponent<Button>().interactable = true;
            skills[4].skillUnlocked = true;
        } 
    }

    public void unlockSkill1()
    {
        Player.instance.moveList.attackUnlocked[1] = true;
    }
    public void unlockSkill2()
    {
        Player.instance.moveList.attackUnlocked[2] = true;
    }
    public void unlockSkill3()
    {
        Player.instance.moveList.attackUnlocked[3] = true;
    }
    public void unlockSkill4()
    {
        Player.instance.moveList.attackUnlocked[4] = true;
    }
    public void unlockSkill5()
    {
        Player.instance.moveList.attackUnlocked[5] = true;
    }
    

}


