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

    public static NewSkillController instance;

    private void Awake() {
        instance = this;
    }
    void Start()
    {
        for (int i = 1; i < skills.Count; i++)
        {
            skills[i].buttonSkill.interactable = false;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
