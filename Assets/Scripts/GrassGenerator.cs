using UnityEngine;

public class GrassGenerator : MonoBehaviour
{
    [Header("Prefabs de grama")]
    public GameObject[] grassPrefabs;

    [Header("Quantidade")]
    public int clusterCount = 10;
    public int grassPerCluster = 5;

    [Header("Área do chunk")]
    public Vector2 mapSize = new Vector2(5, 5);

    [Header("Raycast")]
    public float rayHeight = 50f;

    [Header("Escala")]
    public float minScale = 0.5f;
    public float maxScale = 1.2f;

    [Header("Cluster")]
    public float clusterRadius = 1.5f;

    [Header("Evitar sobreposição")]
    public float minDistance = 0.5f;

    void Start()
    {
        GenerateGrass();
    }

    void GenerateGrass()
    {
        for (int i = 0; i < clusterCount; i++)
        {
            Vector3 clusterCenter = transform.position + new Vector3(
                Random.Range(-mapSize.x, mapSize.x),
                0,
                Random.Range(-mapSize.y, mapSize.y)
            );

            for (int j = 0; j < grassPerCluster; j++)
            {
                Vector3 offset = new Vector3(
                    Random.Range(-clusterRadius, clusterRadius),
                    0,
                    Random.Range(-clusterRadius, clusterRadius)
                );

                Vector3 rayStart = clusterCenter + offset + Vector3.up * rayHeight;

                RaycastHit hit;

                if (Physics.Raycast(rayStart, Vector3.down, out hit, 100f))
                {
                    if (Physics.CheckSphere(hit.point, minDistance))
                        continue;

                    GameObject prefab = grassPrefabs[Random.Range(0, grassPrefabs.Length)];

                    GameObject grass = Instantiate(prefab, hit.point, Quaternion.identity, transform);

                    float scale = Random.Range(minScale, maxScale);
                    grass.transform.localScale *= scale;

                    Renderer rend = grass.GetComponentInChildren<Renderer>();
                    if (rend != null)
                    {
                        float height = rend.bounds.size.y;
                        grass.transform.position += Vector3.up * (height / 10f);
                    }
                }
            }
        }
    }
}