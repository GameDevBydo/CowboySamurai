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

    public Sprite disabled, selectable, activated;


    void Start()
    {
        instance = this;
    

        for(int i = 1; i< skills.Length; i++)
        {
            //Player.instance.moveList.attackUnlocked[i] = false;
            skills[i].GetComponent<Image>().sprite = disabled;
        }

        UnlockSKill(0);
        //Aqui seria o save para desbloquear as skills

        // Rodar um for, usando o UnlockSkill para ativar as skills salvas no save do player.
    }

    void Update()
    {
        ControllerUnlocked(); 
        expUp.text = "EXP: " + exp;
    }


    // Controle de skills desbloqueadas e interatividade dos botões
    public void ControllerUnlocked ()
    {
        //if(exp >= 1f)
        //{
        //    skills[0].GetComponent<Button>().interactable = true;
        //}
        //if(exp >= 2f && skills[1].GetComponent<Skill>().previousSkill[0].skillUnlocked == true)
        //{
        //    skills[1].GetComponent<Button>().interactable = true;
        //    
        //}
        //if(exp >= 3f && skills[2].GetComponent<Skill>().previousSkill[0].skillUnlocked == true)
        //{
        //    skills[2].GetComponent<Button>().interactable = true;
        //    
        //}
        //if(exp >= 4f && skills[3].GetComponent<Skill>().previousSkill[0].skillUnlocked == true)
        //{
        //    skills[3].GetComponent<Button>().interactable = true;
        //    
        //}
        //if(exp >= 5f && skills[4].GetComponent<Skill>().previousSkill[0].skillUnlocked == true)
        //{
        //    skills[4].GetComponent<Button>().interactable = true;
        //    
        //} 
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

    public void unlockSkill1()
    {
        if(Controller.instance.money >= skills[0].price && skills[0].skillUnlocked == false)
        {
            skills[0].GetComponent<Button>().interactable = false;
            Player.instance.moveList.attackUnlocked[1] = true;
            skills[0].skillUnlocked = true;
            Controller.instance.money -= skills[0].price;   
        }
    }
    public void unlockSkill2()
    {
        if(Controller.instance.money >= skills[1].price && skills[1].skillUnlocked == false)
        {
            Player.instance.moveList.attackUnlocked[2] = true;
            skills[1].skillUnlocked = true;
            Controller.instance.money -= skills[1].price;
        }
    }
    public void unlockSkill3()
    {
        if(Controller.instance.money >= skills[2].price && skills[2].skillUnlocked == false)
        {
            Player.instance.moveList.attackUnlocked[3] = true;
            skills[2].skillUnlocked = true;
            Controller.instance.money -= skills[2].price;
        }
    }
    public void unlockSkill4()
    {
        if(Controller.instance.money >= skills[3].price && skills[3].skillUnlocked == false)
        {
            Player.instance.moveList.attackUnlocked[4] = true;
            skills[3].skillUnlocked = true;
            Controller.instance.money -= skills[3].price;
        }
    }
    public void unlockSkill5()
    {
        if(Controller.instance.money >= skills[4].price && skills[4].skillUnlocked == false)
        {
            Player.instance.moveList.attackUnlocked[5] = true;
            skills[4].skillUnlocked = true;
            Controller.instance.money -= skills[4].price;
        }
    }
    

}


