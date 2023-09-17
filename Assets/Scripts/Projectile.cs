using UnityEngine;

public class Projectile
{
    public GameObject gameObject;
    public Vector3 position;
    public Vector3 velocity;
    public bool isActive;

    public Projectile(GameObject prefab)
    {
        gameObject = Object.Instantiate(prefab);
        gameObject.SetActive(false);
        isActive = false;
    }
}
