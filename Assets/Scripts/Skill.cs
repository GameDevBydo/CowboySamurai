using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Skill : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string skillName;

    [TextArea(1,4)]
    public string skillDesc;
    public Sprite skillSprite;
    public bool skillUnlocked;
    public int price;
    public Skill [] nextSkills;

    private Button buttonSkill;
    void Start()
    {
        buttonSkill = this.GetComponent<Button>();
        //buttonSkill.interactable = false;
        //gameObject.GetComponent<Image>().sprite = skillSprite;
    }

    // Assinala visualmente que a skill já foi desbloqueada e "comprada"
    public void LockButton()
    {
        buttonSkill.image.color = Color.black;
    }

    // Assinala que a skill já foi desbloqueada
    public void GetSkill()
    {
        skillUnlocked = true;
    }

    // Verifica se mouse está em cima de algum botão e pega as infos da skill "selecionada"
    public void OnPointerEnter(PointerEventData eventData)
    {
        
      SkillController.instance.skillname.text = skillName;
      SkillController.instance.desc.text = skillDesc;
      SkillController.instance.skillPrice.text = ("Preço: "+ price + " exp");
      SkillController.instance.skillsprite.sprite = skillSprite;
      SkillController.instance.skillsprite.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        
       SkillController.instance.skillname.text = string.Empty;
       SkillController.instance.desc.text = string.Empty;
       SkillController.instance.skillPrice.text = string.Empty;
       SkillController.instance.skillsprite.sprite = null;
       SkillController.instance.skillsprite.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);

    }
}
