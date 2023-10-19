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
    public NewSkill [] nextSkills;
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void h()
    {
        
        for (int i = 0; i < nextSkills.Length; i++)
        {
            nextSkills[i].buttonSkill.interactable = true;
            NewSkillController.instance.unlockedSkills.Add(nextSkills[i]);
        }
    }
}
