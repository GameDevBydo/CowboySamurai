using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewSkillController : MonoBehaviour
{
    public List<NewSkill> skills; //serializar
    public List<NewSkill> unlockedSkills;
    public List<NewSkill> acquiredSkills;
    public List<NewSkill> lostSkills;

    public GameObject skillTreeUI;

    public int xp; // temporário(Substituir por xp do player)
    

    public static NewSkillController instance;

    private void Awake() {
        instance = this;
    }
    void Start()
    {  
        unlockedSkills.Add(skills[0]);
        for (int i = 1; i < skills.Count; i++)
        {
            if(skillTreeUI.activeSelf)
                skills[i].buttonSkill.interactable = false;
        }
        
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
            }
            else if(lostSkills.Contains(skills[i]))
            {
                ColorBlock lostColor = skills[i].buttonSkill.colors;
                lostColor.disabledColor = new Color(1, 0, 0, 0.6f);
                skills[i].buttonSkill.colors = lostColor;

            }
            else
            {
                skills[i].buttonSkill.interactable = true;
            }
        }
    }
    
    void Update()
    {
        
    }
}
