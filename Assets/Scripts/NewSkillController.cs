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
    public Animator anim;

    [SerializeField] private MoveList moveList;
    [SerializeField] private int recoveryHP;
    

    public static NewSkillController instance;

    private void Awake() {
        instance = this;
        unlockedSkills.Add(skills[0]);
    }
    void Start()
    {  
        LoadSkillTree();
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
        Debug.Log(skills.Count);
        for (int i = 0; i < skills.Count; i++)
        { 
            if(unlockedSkills.Contains(skills[i]))
            {
                skills[i].buttonSkill.interactable = true;
                skills[i].upSkill.SetActive(true);
            }
            else if (acquiredSkills.Contains(skills[i]))
            {
                ColorBlock acquiredColor = skills[i].buttonSkill.colors;
                acquiredColor.disabledColor = new Color(0, 1, 0, 0.6f);
                skills[i].buttonSkill.colors = acquiredColor;
                skills[i].buttonSkill.interactable = false;
                skills[i].upSkill.SetActive(false);
            }
            else if(lostSkills.Contains(skills[i]))
            {
                ColorBlock lostColor = skills[i].buttonSkill.colors;
                lostColor.disabledColor = new Color(1, 0, 0, 0.6f);
                skills[i].buttonSkill.colors = lostColor;
                skills[i].buttonSkill.interactable = false;
                skills[i].upSkill.SetActive(false);

            }
            else
            {
                skills[i].buttonSkill.interactable = false;
                skills[i].upSkill.SetActive(false);
            }
        }
    }
    
    public void Skill1()
    {
        Player.instance.SetMaxMeter(2);
        Player.instance.ChangeMeter(2);
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
        Player.instance.SetMaxMeter(4);
        Player.instance.ChangeMeter(2);
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
        var rectBack = Controller.instance.hp_Background.GetComponent<RectTransform>();
        Vector2 backWidth = rectBack.sizeDelta;
        backWidth.x = 1000f;
        rectBack.sizeDelta = backWidth;

        Vector2 backPos = rectBack.anchoredPosition;
        backPos.x = 499.3f;
        rectBack.anchoredPosition = backPos;

        var rectFill = Controller.instance.hp_Fill.GetComponent<RectTransform>();
        Vector2 fillWidth = rectFill.sizeDelta;
        fillWidth.x = 1000f;
        rectFill.sizeDelta = fillWidth;

        Vector2 fillPos = rectFill.anchoredPosition;
        fillPos.x = 114f;
        rectFill.anchoredPosition = fillPos;

        var rectBullet = Controller.instance.ui_Bullet.GetComponent<RectTransform>();
        Vector2 bulletPos = rectBullet.anchoredPosition;
        bulletPos.x = 1110f;
        rectBullet.anchoredPosition = bulletPos;


        Player.instance.maxHP += recoveryHP;
        //Controller.instance.UpdateLifeBar((float)Player.instance.hitPoints/(float)Player.instance.maxHP);
    }
    public void Skill7()
    {
        Player.instance.IncreaseBaseDamage(15);
    }
    public void Skill8()
    {
        Player.instance.SetMaxMeter(6);
        Player.instance.ChangeMeter(2);
    }
    public void Skill9()
    {
        Player.instance.IncreaseBaseDamage(10);
    }
}
