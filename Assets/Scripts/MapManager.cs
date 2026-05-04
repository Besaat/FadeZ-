using UnityEngine;

// Gerencia a geração procedural do mapa de expedição.
// Gera árvores, arbustos, pedras e inimigos em posições aleatórias
// respeitando distâncias mínimas entre objetos.
public class MapManager : MonoBehaviour
{
    [Header("Referências")]
    public GameObject mapParent;   // Objeto pai de todos os elementos gerados
    public Transform player;       // Transform do player
    public Transform baseSpawn;    // Ponto de spawn da base (acampamento)
    public Transform mapSpawn;     // Ponto de spawn central do mapa de expedição

    [Header("Mapa")]
    public float mapSize = 25f;    // Metade do tamanho total do mapa (ex: 25 = área de 50x50)

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

    void Update()
    {
        // Detecta se o player caiu do mapa (y < -1) e regenera
        // TODO: substituir por um trigger de borda mais robusto (DungeonBorderTrigger)
        if (player != null && player.position.y < -1f)
            GenerateNewMap();
    }

    // Limpa o mapa atual e gera um novo completo
    public void GenerateNewMap()
    {
        ClearMap();

        SpawnCategory(treePrefabs, treeCount, treeMinScale, treeMaxScale, treeMinDistance, treeSpawnY);
        SpawnCategory(bushPrefabs, bushCount, bushMinScale, bushMaxScale, bushMinDistance, bushSpawnY);
        SpawnCategory(rockPrefabs, rockCount, rockMinScale, rockMaxScale, rockMinDistance, rockSpawnY);
        SpawnEnemies();

        TeleportPlayer(mapSpawn.position);
    }

    // Move o player para a base e limpa o mapa
    public void GoToBase()
    {
        ClearMap();
        TeleportPlayer(baseSpawn.position);
    }

    // Destrói todos os filhos do mapParent
    void ClearMap()
    {
        for (int i = mapParent.transform.childCount - 1; i >= 0; i--)
            Destroy(mapParent.transform.GetChild(i).gameObject);
    }

    // Teleporta o player via Rigidbody para evitar conflito com a física
    void TeleportPlayer(Vector3 destination)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.position = destination;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
        }
        else
        {
            player.position = destination;
        }
    }

    // Gera uma categoria de objetos (árvores, arbustos, pedras) com verificação de distância
    void SpawnCategory(GameObject[] prefabs, int count, float minScale, float maxScale, float minDist, float spawnY)
    {
        if (prefabs == null || prefabs.Length == 0 || count <= 0) return;

        int spawned = 0;
        int maxAttempts = count * 20; // Evita loop infinito se o mapa estiver cheio

        for (int i = 0; i < maxAttempts && spawned < count; i++)
        {
            float x = Random.Range(-mapSize, mapSize);
            float z = Random.Range(-mapSize, mapSize);
            Vector3 spawnPos = new Vector3(mapSpawn.position.x + x, spawnY, mapSpawn.position.z + z);

            // Mantém área livre ao redor do ponto de spawn do player
            if (Vector3.Distance(spawnPos, mapSpawn.position) < 5f) continue;

            // Verifica sobreposição com objetos já gerados
            bool tooClose = false;
            foreach (Transform child in mapParent.transform)
            {
                if (Vector3.Distance(child.position, spawnPos) < minDist)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose) continue;

            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
            GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity, mapParent.transform);
            float scale = Random.Range(minScale, maxScale);
            obj.transform.localScale = Vector3.one * scale;
            spawned++;
        }
    }

    // Gera inimigos distribuídos pelo mapa usando uma grade de células
    // para garantir distribuição espacial mais uniforme
    void SpawnEnemies()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0 || enemyCount <= 0) return;

        // Divide o mapa em células e tenta spawnar um inimigo por célula
        int cols = Mathf.CeilToInt(Mathf.Sqrt(enemyCount));
        int rows = Mathf.CeilToInt((float)enemyCount / cols);
        float cellW = (mapSize * 2f) / cols;
        float cellH = (mapSize * 2f) / rows;

        int spawned = 0;

        for (int row = 0; row < rows && spawned < enemyCount; row++)
        {
            for (int col = 0; col < cols && spawned < enemyCount; col++)
            {
                // Posição aleatória dentro da célula
                float x = Random.Range(-mapSize + col * cellW + 1f, -mapSize + (col + 1) * cellW - 1f);
                float z = Random.Range(-mapSize + row * cellH + 1f, -mapSize + (row + 1) * cellH - 1f);
                Vector3 pos = new Vector3(mapSpawn.position.x + x, enemySpawnY, mapSpawn.position.z + z);

                // Zona segura ao redor do spawn do player
                if (Vector3.Distance(pos, mapSpawn.position) < 15f) continue;

                GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                Instantiate(prefab, pos, Quaternion.identity, mapParent.transform);
                spawned++;
                // CORREÇÃO: o continue anterior fazia com que spawns em zona segura
                // simplesmente pulassem sem tentar outra posição na mesma célula.
                // Agora o loop de rows/cols garante que todos os inimigos sejam gerados.
            }
        }
    }
}
