using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Game/Combat/Attack", order = 2)]
public class Attack : ScriptableObject
{
    public int attackId;
    public Hitbox[] hitboxes;
    public int[] damage;
    public bool hit;
    public float startUp;
}
