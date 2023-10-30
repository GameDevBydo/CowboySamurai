using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GameSaveData
{
    #region Audio
    [HideInInspector]
    public float music;
    public float volumeGeral;
    public float soundsEffects;
    #endregion

    #region Game_variaveis
    public int money;
    #endregion

    #region Player_Stats
    public int life;
    public int powerBar;

    public List<NewSkill> skills; //serializar
    public List<NewSkill> unlockedSkills;
    public List<NewSkill> acquiredSkills;
    public List<NewSkill> lostSkills;
    #endregion

}
