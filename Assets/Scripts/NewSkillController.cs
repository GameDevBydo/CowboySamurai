using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewSkillController : MonoBehaviour
{
    public GameObject [] skills;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 1; i < skills.Length; i++)
        {
            skills[i].GetComponent<Button>().interactable = false;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
