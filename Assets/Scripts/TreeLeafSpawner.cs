using UnityEngine;
using System.Collections;

// Spawna folhas caindo de uma árvore em intervalos aleatórios.
// Cada folha é um prefab com o script LeafFall que gerencia o movimento.
// Colocar este script no topo da árvore para que as folhas comecem a cair dali.
public class TreeLeafSpawner : MonoBehaviour
{
    [Header("Referências")]
    public GameObject leafPrefab; // Prefab da folha (deve ter o script LeafFall)

    [Header("Intervalo")]
    public float minTime = 1f; // Tempo mínimo entre spawns (segundos)
    public float maxTime = 4f; // Tempo máximo entre spawns (segundos)

    [Header("Área de spawn")]
    public Vector3 spawnArea = new Vector3(2f, 0f, 2f); // Raio de dispersão horizontal

    void Start()
    {
        StartCoroutine(SpawnLeaves());
    }

    IEnumerator SpawnLeaves()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTime, maxTime));
            SpawnLeaf();
        }
    }

    void SpawnLeaf()
    {
        if (leafPrefab == null) return;

        // Posição aleatória dentro da área definida, centrada no topo da árvore
        Vector3 offset = new Vector3(
            Random.Range(-spawnArea.x, spawnArea.x),
            0f,
            Random.Range(-spawnArea.z, spawnArea.z)
        );

        Vector3 spawnPos = transform.position + offset;
        GameObject leaf = Instantiate(leafPrefab, spawnPos, Quaternion.identity);

        // Escala aleatória para variedade visual
        float scale = Random.Range(0.8f, 1.5f);
        leaf.transform.localScale = Vector3.one * scale;
    }
}
