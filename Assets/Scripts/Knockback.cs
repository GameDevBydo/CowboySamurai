using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public string knockbackTag = "";
    public float knockbackForce = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(knockbackTag))
        {
            Rigidbody otherRigidBody = collision.gameObject.GetComponent<Rigidbody>();
            if (otherRigidBody != null)
            {
                Vector3 direction = (collision.transform.position - transform.position).normalized;
                otherRigidBody.AddForce(direction * knockbackForce, ForceMode.Impulse);
            }
        }
    }
}
