using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Game/Combat/Attack", order = 2)]
public class Attack : ScriptableObject
{
    public string attackName;
    public Hitbox[] hitboxes;
    public int damage;
    [HideInInspector]
    public bool hit;
    public float startUp;
    public float recovery;
    public float stun;
    public float knockbackRatio;
    public float knockupRatio;

    public float meterGen;
    public AudioClip sfx;
}
