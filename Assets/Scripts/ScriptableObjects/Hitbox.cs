using UnityEngine;

[CreateAssetMenu(fileName = "Hitbox", menuName = "Game/Combat/Hitbox", order = 1 )]
public class Hitbox : ScriptableObject
{
    public Vector3 _startingPoint;
    public Vector3 startingPoint => new Vector3(_startingPoint.x * -Mathf.Sign(Player.instance.transform.rotation.eulerAngles.y-180), _startingPoint.y, _startingPoint.z);
    public Vector3 extension;
    public Vector3 _startingPointEnemy;
    public Vector3 startingPointEnemy =>new Vector3(_startingPoint.x , _startingPoint.y, _startingPoint.z);
    
}
