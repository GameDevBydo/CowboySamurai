using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenarioMovimento : MonoBehaviour
{
    public float Velocidade;
    public float tempoDestruicao;

    private float tempoPassado;

    void Start()
    {
        tempoPassado = 0f;
    }

    void Update()
    {
        // Move o cenário na direção negativa do eixo X (esquerda)
        transform.Translate(Vector3.left * Velocidade * Time.deltaTime);

        tempoPassado += Time.deltaTime;

        // Verifica se o tempo passado atingiu o tempo de destruição
        if (tempoPassado >= tempoDestruicao)
        {
            // Destroi o objeto do cenário
            Destroy(gameObject);
        }
    }
}
