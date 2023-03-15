using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    Controller main;
    void Awake()
    {
        main = Controller.instance;
    }
    
}
