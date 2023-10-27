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
            skills[i].buttonSkill.interactable = false;
        }
        
    }

    
    void Update()
    {
        
    }
}
