using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewSkillController : MonoBehaviour
{
    public List<NewSkill> skills; //serializar
    public List<NewSkill> unlockedSkills;
    public List<NewSkill> acquiredSkills;
    public List<NewSkill> lostSkills;

    public GameObject skillTreeUI;
    public TextMeshProUGUI xpText;
    public float xp;

    [SerializeField] private MoveList moveList;
    [SerializeField] private int recoveryHP;
    

    public static NewSkillController instance;

    private void Awake() {
        instance = this;
        unlockedSkills.Add(skills[0]);
    }
    void Start()
    {  
        
        for (int i = 1; i < skills.Count; i++)
        {
            skills[i].buttonSkill.interactable = false;
        }

        if(acquiredSkills.Contains(skills[0]))
        {
            ColorBlock acquiredColor = skills[0].buttonSkill.colors;
            acquiredColor.disabledColor = new Color(0, 1, 0, 0.6f);
            skills[0].buttonSkill.colors = acquiredColor;
            skills[0].buttonSkill.interactable = false;
        }
        
    }
    void Update()
    {
        UpdateXP();
    }

    public void UpdateXP()
    {
        xp = Player.instance.exp;
        xpText.text = xp.ToString() + "Xp";
    }

    public void LoadSkillTree()
    {
        for (int i = 0; i < skills.Count; i++)
        {
            if(unlockedSkills.Contains(skills[i]))
            {
                skills[i].buttonSkill.interactable = true;
            }
            else if (acquiredSkills.Contains(skills[i]))
            {
                ColorBlock acquiredColor = skills[i].buttonSkill.colors;
                acquiredColor.disabledColor = new Color(0, 1, 0, 0.6f);
                skills[i].buttonSkill.colors = acquiredColor;
                skills[i].buttonSkill.interactable = false;
            }
            else if(lostSkills.Contains(skills[i]))
            {
                ColorBlock lostColor = skills[i].buttonSkill.colors;
                lostColor.disabledColor = new Color(1, 0, 0, 0.6f);
                skills[i].buttonSkill.colors = lostColor;
                skills[i].buttonSkill.interactable = false;

            }
            else
            {
                skills[i].buttonSkill.interactable = false;
            }
        }
    }
    
    public void Skill1()
    {
        Controller.instance.UpdateBulletBar(40);
        moveList.attackUnlocked[1] = true;
        moveList.attackUnlocked[2] = true;
    }
    public void Skill2()
    {
        moveList.attackUnlocked[3] = true;
        moveList.attackUnlocked[4] = true;
    }
    public void Skill3()
    {
        Controller.instance.UpdateBulletBar(80);
    }
    public void Skill4()
    {
        Player.instance.IncreaseBaseDamage(5);
    }
    public void Skill5()
    {
        Player.instance.dashCD = 0.5f;
    }
    public void Skill6()
    {
        Player.instance.hitPoints += recoveryHP;
        Controller.instance.UpdateLifeBar((float)Player.instance.hitPoints/(float)Player.instance.maxHP);
    }
    public void Skill7()
    {
        
    }
    public void Skill8()
    {
        Controller.instance.UpdateBulletBar(120);
    }
    public void Skill9()
    {
        Player.instance.IncreaseBaseDamage(15);
    }
}
