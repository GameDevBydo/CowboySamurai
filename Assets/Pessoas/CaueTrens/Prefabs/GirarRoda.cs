using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirarRoda : MonoBehaviour
{
    public float velocidade;

    void Update()
    {
        // Gira a roda em torno do eixo Y
        transform.Rotate(Vector3.up, velocidade * Time.deltaTime);
    }
}
