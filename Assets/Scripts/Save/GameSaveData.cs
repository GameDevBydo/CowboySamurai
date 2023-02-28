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
    public float volume;
    public float soundsEffects;
    #endregion

    #region Game_variaveis
    public int money;
    #endregion

    #region Player_Stats
    public int life;
    public int powerBar;
    #endregion

}
