using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveList", menuName = "Game/Combat/MoveList", order = 3)]
public class MoveList : ScriptableObject
{
    public Attack[] _attack;
    public bool[] attackUnlocked;
}
