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
        if(Player.instance.money >= skills[0].price && skills[0].skillUnlocked == false)
        {
            skills[0].GetComponent<Button>().interactable = false;
            Player.instance.moveList.attackUnlocked[1] = true;
            skills[0].skillUnlocked = true;
            Player.instance.money -= skills[0].price;   
        }
    }
    public void unlockSkill2()
    {
        if(Player.instance.money >= skills[1].price && skills[1].skillUnlocked == false)
        {
            Player.instance.moveList.attackUnlocked[2] = true;
            skills[1].skillUnlocked = true;
            Player.instance.money -= skills[1].price;
        }
    }
    public void unlockSkill3()
    {
        if(Player.instance.money >= skills[2].price && skills[2].skillUnlocked == false)
        {
            Player.instance.moveList.attackUnlocked[3] = true;
            skills[2].skillUnlocked = true;
            Player.instance.money -= skills[2].price;
        }
    }
    public void unlockSkill4()
    {
        if(Player.instance.money >= skills[3].price && skills[3].skillUnlocked == false)
        {
            Player.instance.moveList.attackUnlocked[4] = true;
            skills[3].skillUnlocked = true;
            Player.instance.money -= skills[3].price;
        }
    }
    public void unlockSkill5()
    {
        if(Player.instance.money >= skills[4].price && skills[4].skillUnlocked == false)
        {
            Player.instance.moveList.attackUnlocked[5] = true;
            skills[4].skillUnlocked = true;
            Player.instance.money -= skills[4].price;
        }
    }
    

}


