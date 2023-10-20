using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewSkill : MonoBehaviour
{
    [Header("Propriedades da Skill")]
    public string skillName;

    [TextArea(1,4)]
    public string skillDesc;
    public Sprite skillSprite;
    public int skillPrice;
    public List<NewSkill> nextSkills;
    public NewSkill sideSkill;
    public Button buttonSkill;
    
    [Header("Propriedades da UI Skill")]

    [SerializeField] private  TextMeshProUGUI skillNameUI;
    [SerializeField] private  TextMeshProUGUI skillDescUI;
    [SerializeField] private  TextMeshProUGUI skillPriceUI;
    [SerializeField] private  Image skillIconUI;

    

    private void Awake() {
        
        buttonSkill = GetComponent<Button>();   
    }
    void Start()
    {
        buttonSkill.onClick.AddListener(h);
    }

    
    void Update()
    {
        
    }

    public void h()
    {
        if(NewSkillController.instance.xp >= skillPrice)
        {
            NewSkillController.instance.xp -= skillPrice;
            NewSkillController.instance.unlockedSkills.Remove(this);
            NewSkillController.instance.acquiredSkills.Add(this);

            ColorBlock acquiredColor = buttonSkill.colors;
            acquiredColor.disabledColor = new Color(0, 1, 0, 0.6f);
            buttonSkill.colors = acquiredColor;
            
            this.buttonSkill.interactable = false;

            if(!nextSkills.Count.Equals(0))
            {
                for (int i = 0; i < nextSkills.Count; i++)
                {
                    nextSkills[i].buttonSkill.interactable = true;
                    NewSkillController.instance.unlockedSkills.Add(nextSkills[i]);
                }   
            }
            if(sideSkill != null)
            {
                NewSkillController.instance.unlockedSkills.Remove(sideSkill);
                NewSkillController.instance.lostSkills.Add(sideSkill);

                ColorBlock lostColor = sideSkill.buttonSkill.colors;
                lostColor.disabledColor = new Color(1, 0, 0, 0.6f);
                sideSkill.buttonSkill.colors = lostColor;

                sideSkill.buttonSkill.interactable = false;
            }
        }
        else
        {
            Debug.Log("Pobre");
        }
        
        
    }
}
