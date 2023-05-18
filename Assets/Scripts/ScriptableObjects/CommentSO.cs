using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Comment", menuName = "Game/Combat/Comment", order = 3)]
public class CommentSO : ScriptableObject
{
    public string[] comments;
}
