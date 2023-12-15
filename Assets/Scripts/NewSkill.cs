using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
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
    public List<NewSkill> sideSkill;
    public Button buttonSkill;
    public GameObject upSkill;
    
    [Header("Propriedades da UI Skill")]

    [SerializeField] private  TextMeshProUGUI skillNameUI;
    [SerializeField] private  TextMeshProUGUI skillDescUI;
    
    [SerializeField] private  Image skillIconUI;

    public UnityEvent eventSkill;

    private void Awake() {
        
        buttonSkill = GetComponent<Button>();
        skillNameUI.text = skillName;
        skillDescUI.text = skillDesc + " - " + skillPrice + " XP";
    }
    void Start()
    {
        buttonSkill.onClick.AddListener(ControlSkill);
    }

    
    void Update()
    {
        
    }

    public void ControlSkill()
    {
        if(NewSkillController.instance.xp >= skillPrice)
        {
            eventSkill.Invoke();
            Player.instance.exp -= skillPrice;
            NewSkillController.instance.unlockedSkills.Remove(this);
            NewSkillController.instance.acquiredSkills.Add(this);

            ColorBlock acquiredColor = buttonSkill.colors;
            acquiredColor.disabledColor = new Color(0, 1, 0, 0.6f);
            buttonSkill.colors = acquiredColor;
            
            this.buttonSkill.interactable = false;
            this.upSkill.SetActive(false);
            

            if(!nextSkills.Count.Equals(0))
            {
                for (int i = 0; i < nextSkills.Count; i++)
                {
                    nextSkills[i].buttonSkill.interactable = true;
                    NewSkillController.instance.unlockedSkills.Add(nextSkills[i]);
                    nextSkills[i].upSkill.SetActive(true);
                }   
            }
            if(sideSkill != null)
            {
                foreach (var item in sideSkill)
                {
                    NewSkillController.instance.unlockedSkills.Remove(item);
                    NewSkillController.instance.lostSkills.Add(item);

                    ColorBlock lostColor = item.buttonSkill.colors;
                    lostColor.disabledColor = new Color(1, 0, 0, 0.6f);
                    item.buttonSkill.colors = lostColor;
                    item.upSkill.SetActive(false);
                    item.buttonSkill.interactable = false;
                }
                
            }
        }
        else
        {
            NewSkillController.instance.anim.SetTrigger("OneTime");
        }
        
        
    }
}
