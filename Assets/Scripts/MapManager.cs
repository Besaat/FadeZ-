using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject mapParent;

    [Header("Árvores")]
    public GameObject[] treePrefabs;
    public int treeCount = 20;
    public float treeMinScale = 0.8f;
    public float treeMaxScale = 1.3f;
    public float treeMinDistance = 2.5f;
    public float treeSpawnY = 0f;

    [Header("Arbustos")]
    public GameObject[] bushPrefabs;
    public int bushCount = 15;
    public float bushMinScale = 0.7f;
    public float bushMaxScale = 1.2f;
    public float bushMinDistance = 1.5f;
    public float bushSpawnY = 0f;

    [Header("Pedras")]
    public GameObject[] rockPrefabs;
    public int rockCount = 10;
    public float rockMinScale = 0.8f;
    public float rockMaxScale = 1.5f;
    public float rockMinDistance = 2f;
    public float rockSpawnY = 0f;

    [Header("Inimigos")]
    public GameObject[] enemyPrefabs;
    public int enemyCount = 5;
    public float enemySpawnY = 0f;

    [Header("Mapa")]
    public float mapSize = 25f;
    public Transform player;
    public Transform baseSpawn;
    public Transform mapSpawn;

    void Update()
    {
        if (player == null) return;
        if (player.position.y < -1f)
            GenerateNewMap();
    }

    public void GenerateNewMap()
    {
        for (int i = mapParent.transform.childCount - 1; i >= 0; i--)
            Destroy(mapParent.transform.GetChild(i).gameObject);

        SpawnCategory(treePrefabs, treeCount, treeMinScale, treeMaxScale, treeMinDistance, treeSpawnY);
        SpawnCategory(bushPrefabs, bushCount, bushMinScale, bushMaxScale, bushMinDistance, bushSpawnY);
        SpawnCategory(rockPrefabs, rockCount, rockMinScale, rockMaxScale, rockMinDistance, rockSpawnY);

        SpawnEnemies();

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.position = mapSpawn.position;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
        }
    }

    void SpawnCategory(GameObject[] prefabs, int count, float minScale, float maxScale, float minDistance, float spawnY)
    {
        if (prefabs == null || prefabs.Length == 0 || count <= 0) return;

        int spawned = 0;
        int maxAttempts = count * 20;

        for (int i = 0; i < maxAttempts && spawned < count; i++)
        {
            float x = Random.Range(-mapSize, mapSize);
            float z = Random.Range(-mapSize, mapSize);
            Vector3 spawnPos = new Vector3(mapSpawn.position.x + x, spawnY, mapSpawn.position.z + z);

            if (Vector3.Distance(spawnPos, mapSpawn.position) < 5f) continue;

            bool tooClose = false;
            for (int j = 0; j < mapParent.transform.childCount; j++)
            {
                if (Vector3.Distance(mapParent.transform.GetChild(j).position, spawnPos) < minDistance)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose) continue;

            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
            GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity, mapParent.transform);
            float scale = Random.Range(minScale, maxScale);
            obj.transform.localScale = new Vector3(scale, scale, scale);
            spawned++;
        }
    }

    void SpawnEnemies()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0 || enemyCount <= 0) return;

        int cols = Mathf.CeilToInt(Mathf.Sqrt(enemyCount));
        int rows = Mathf.CeilToInt((float)enemyCount / cols);
        float cellW = (mapSize * 2) / cols;
        float cellH = (mapSize * 2) / rows;

        int spawned = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (spawned >= enemyCount) break;

                float x = Random.Range(-mapSize + col * cellW + 1f,
                                       -mapSize + (col + 1) * cellW - 1f);
                float z = Random.Range(-mapSize + row * cellH + 1f,
                                       -mapSize + (row + 1) * cellH - 1f);

                Vector3 pos = new Vector3(mapSpawn.position.x + x, enemySpawnY, mapSpawn.position.z + z);

                if (Vector3.Distance(pos, mapSpawn.position) < 15f) continue;

                GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                Instantiate(prefab, pos, Quaternion.identity, mapParent.transform);
                spawned++;
            }
        }
    }

    public void GoToBase()
    {
        for (int i = mapParent.transform.childCount - 1; i >= 0; i--)
            Destroy(mapParent.transform.GetChild(i).gameObject);

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.position = baseSpawn.position;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
        }
    }
}