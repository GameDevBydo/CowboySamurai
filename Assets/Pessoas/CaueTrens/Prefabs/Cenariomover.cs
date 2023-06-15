using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cenariomover : MonoBehaviour
{
    public GameObject cenarioPrefab;
    public float velocidade;
    public float intervalo;

    private float tempoPassado;

    void Start()
    {
        tempoPassado = intervalo;
    }

    void Update()
    {
        tempoPassado += Time.deltaTime;

        if (tempoPassado >= intervalo)
        {
            // Instancia o prefab do cenário
            GameObject novoCenario = Instantiate(cenarioPrefab, transform.position, Quaternion.identity);

            // Define a velocidade de movimento do cenário
            CenarioMovimento movimento = novoCenario.GetComponent<CenarioMovimento>();
            movimento.Velocidade = velocidade;

            tempoPassado = 0f;
        }
    }
}
