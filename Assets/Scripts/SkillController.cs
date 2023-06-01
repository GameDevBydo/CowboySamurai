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
    

        for(int i = 1; i< Player.instance.moveList.attackUnlocked.Length; i++)
            {
                Player.instance.moveList.attackUnlocked[i] = false;
            }
    }

    void Update()
    {
        ControllerUnlocked(); 
        expUp.text = "EXP: " + exp;
    }


    // Controle de skills desbloqueadas e interatividade dos botões
    public void ControllerUnlocked ()
    {

        if(exp >= 1f)
        {
            skills[0].GetComponent<Button>().interactable = true;
        }
        if(exp >= 2f && skills[1].GetComponent<Skill>().previousSkill[0].skillUnlocked == true)
        {
            skills[1].GetComponent<Button>().interactable = true;
            
        }
        if(exp >= 3f && skills[2].GetComponent<Skill>().previousSkill[0].skillUnlocked == true)
        {
            skills[2].GetComponent<Button>().interactable = true;
            
        }
        if(exp >= 4f && skills[3].GetComponent<Skill>().previousSkill[0].skillUnlocked == true)
        {
            skills[3].GetComponent<Button>().interactable = true;
            
        }
        if(exp >= 5f && skills[4].GetComponent<Skill>().previousSkill[0].skillUnlocked == true)
        {
            skills[4].GetComponent<Button>().interactable = true;
            
        } 
    }

    public void unlockSkill1()
    {
        Player.instance.moveList.attackUnlocked[1] = true;
        skills[0].skillUnlocked = true;
    }
    public void unlockSkill2()
    {
        Player.instance.moveList.attackUnlocked[2] = true;
        skills[1].skillUnlocked = true;
    }
    public void unlockSkill3()
    {
        Player.instance.moveList.attackUnlocked[3] = true;
        skills[2].skillUnlocked = true;
    }
    public void unlockSkill4()
    {
        Player.instance.moveList.attackUnlocked[4] = true;
        skills[3].skillUnlocked = true;
    }
    public void unlockSkill5()
    {
        Player.instance.moveList.attackUnlocked[5] = true;
        skills[4].skillUnlocked = true;
    }
    

}


