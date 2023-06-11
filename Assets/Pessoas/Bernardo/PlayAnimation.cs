using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
    public AnimationClip anim;
    void Start()
    {
        Animation animation = GetComponent<Animation>();
        animation.AddClip(anim, "Idle");
        animation.clip = anim;
        animation.Play();
    }

}
