using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractEnemy : MonoBehaviour
{
    public Animator animEnemy;
    public Transform playerPosition;

    public abstract void Move();
    public abstract void Attack();
    public abstract void FollowPlayer();
    public abstract void Death();

}
