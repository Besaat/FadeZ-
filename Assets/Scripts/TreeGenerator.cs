using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] treePrefabs;

    [Header("Quantidade")]
    public int treeCount = 10;

    [Header("Área do chunk")]
    public Vector2 mapSize = new Vector2(5, 5);
    public float rayHeight = 50f;

    [Header("Escala")]
    public float minScale = 0.8f;
    public float maxScale = 1.3f;

    [Header("Evitar sobreposição")]
    public float minDistance = 2f;

    void Start()
    {
        GenerateTrees();
    }

    void GenerateTrees()
    {
        for (int i = 0; i < treeCount; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-mapSize.x, mapSize.x),
                0,
                Random.Range(-mapSize.y, mapSize.y)
            );

            Vector3 rayStart = transform.position + randomOffset + Vector3.up * rayHeight;

            RaycastHit hit;

            if (Physics.Raycast(rayStart, Vector3.down, out hit, 100f))
            {
                // 🚫 evita spawn em cima de outra coisa
                if (Physics.CheckSphere(hit.point, minDistance))
                    continue;

                // 🔥 escolhe prefab aleatório
                if (treePrefabs == null || treePrefabs.Length == 0)
                    return;

                GameObject chosenPrefab = treePrefabs[Random.Range(0, treePrefabs.Length)];

                GameObject tree = Instantiate(chosenPrefab, hit.point, Quaternion.identity, transform);

                // 🔥 escala aleatória
                float scale = Random.Range(minScale, maxScale);
                tree.transform.localScale *= scale;

                // 🔥 ajusta altura baseado no renderer
                Renderer rend = tree.GetComponentInChildren<Renderer>();
                if (rend != null)
                {
                    float height = rend.bounds.size.y;
                    tree.transform.position += Vector3.up * (height / 2.1f);
                }
            }
        }
    }
}