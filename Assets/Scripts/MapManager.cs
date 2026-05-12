using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Gerencia a geração procedural do mapa de expedição.
public class MapManager : MonoBehaviour
{
    [Header("Referências")]
    public GameObject mapParent;
    public Transform player;
    public Transform baseSpawn;
    public Transform mapSpawn;
    public CameraFollow cameraFollow;
    public PlayerMove playerMove;
    public FloorTextureRandomizer floorRandomizer;
    public CardSelectionUI cardSelectionUI;

    [Header("Mapa")]
    public float mapSize = 50f;
    public float playerSafeZone = 8f;
    public float borderTreeDepth = 6f;

    [Header("Árvores")]
    public GameObject[] treePrefabs;
    public int treeCount = 60;
    public float treeMinScale = 0.8f;
    public float treeMaxScale = 1.3f;
    public float treeMinDistance = 2.5f;
    public float treeSpawnY = 0f;

    [Header("Arbustos")]
    public GameObject[] bushPrefabs;
    public int bushCount = 30;
    public float bushMinScale = 0.7f;
    public float bushMaxScale = 1.2f;
    public float bushMinDistance = 1.5f;
    public float bushSpawnY = 0f;

    [Header("Pedras")]
    public GameObject[] rockPrefabs;
    public int rockCount = 20;
    public float rockMinScale = 0.8f;
    public float rockMaxScale = 1.5f;
    public float rockMinDistance = 2f;
    public float rockSpawnY = 0f;

    [Header("Inimigos")]
    public GameObject[] enemyPrefabs;
    public int enemyCount = 8;
    public float enemySpawnY = 0f;

    [Header("Waves de inimigos")]
    public int waveCount = 3;
    public float timeBetweenWaves = 5f;
    public float timeBetweenSpawns = 0.5f;

    [HideInInspector] public Vector3 lastEntryDirection = Vector3.zero;

    public bool allEnemiesDead { get; private set; } = false;

    // Contadores da HUD
    public int currentFloor { get; private set; } = 0;
    public int totalKills { get; private set; } = 0;

    private int totalEnemiesSpawned = 0;
    private int enemiesDead = 0;
    private bool wavesFinished = false;

    void Update()
    {
        if (player != null && player.position.y < -1f)
            GenerateNewMap();
    }

    public void GenerateNewMap()
    {
        allEnemiesDead = false;
        totalEnemiesSpawned = 0;
        enemiesDead = 0;
        wavesFinished = false;

        // Incrementa o contador de fases a cada novo mapa gerado
        currentFloor++;

        ClearMap();

        SpawnCategory(treePrefabs, treeCount, treeMinScale, treeMaxScale, treeMinDistance, treeSpawnY);
        SpawnCategory(bushPrefabs, bushCount, bushMinScale, bushMaxScale, bushMinDistance, bushSpawnY);
        SpawnCategory(rockPrefabs, rockCount, rockMinScale, rockMaxScale, rockMinDistance, rockSpawnY);
        SpawnBorderTrees();

        TeleportPlayer(GetOppositeSpawnPosition());

        if (cameraFollow != null)
            cameraFollow.SetMapBounds(mapSpawn.position, mapSize);

        if (floorRandomizer != null)
            floorRandomizer.RandomizeTexture();

        StartCoroutine(SpawnWaves());
    }

    public void GoToBase()
    {
        StopAllCoroutines();
        ClearMap();
        TeleportPlayer(baseSpawn.position);

        if (cameraFollow != null)
            cameraFollow.ClearMapBounds();

        if (playerMove != null)
        {
            playerMove.dashEnabled = false;
            playerMove.runEnabled = false;
        }
    }

    public void OnEnemyDied()
    {
        enemiesDead++;
        totalKills++; // Contador global de abates

        if (wavesFinished && enemiesDead >= totalEnemiesSpawned)
            OnAllEnemiesDead();
    }

    void OnAllEnemiesDead()
    {
        allEnemiesDead = true;

        if (cardSelectionUI != null)
            cardSelectionUI.ShowCards();
    }

    void ClearMap()
    {
        StopAllCoroutines();
        for (int i = mapParent.transform.childCount - 1; i >= 0; i--)
            Destroy(mapParent.transform.GetChild(i).gameObject);
    }

    Vector3 GetOppositeSpawnPosition()
    {
        if (lastEntryDirection == Vector3.zero)
            return mapSpawn.position;

        float spawnOffset = mapSize - borderTreeDepth - playerSafeZone;
        Vector3 spawnPos = mapSpawn.position + (-lastEntryDirection.normalized) * spawnOffset;
        spawnPos.y = mapSpawn.position.y;
        return spawnPos;
    }

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

    void SpawnBorderTrees()
    {
        if (treePrefabs == null || treePrefabs.Length == 0) return;

        int borderCount = treeCount / 2;
        int spawned = 0;
        int maxAttempts = borderCount * 20;

        for (int i = 0; i < maxAttempts && spawned < borderCount; i++)
        {
            float x = Random.Range(-mapSize, mapSize);
            float z = Random.Range(-mapSize, mapSize);

            bool inBorderX = Mathf.Abs(x) > mapSize - borderTreeDepth;
            bool inBorderZ = Mathf.Abs(z) > mapSize - borderTreeDepth;
            if (!inBorderX && !inBorderZ) continue;

            Vector3 spawnPos = new Vector3(mapSpawn.position.x + x, treeSpawnY, mapSpawn.position.z + z);

            bool tooClose = false;
            foreach (Transform child in mapParent.transform)
            {
                if (Vector3.Distance(child.position, spawnPos) < 1.5f)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose) continue;

            GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
            GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity, mapParent.transform);
            obj.transform.localScale = Vector3.one * Random.Range(treeMinScale, treeMaxScale);
            spawned++;
        }
    }

    void SpawnCategory(GameObject[] prefabs, int count, float minScale, float maxScale, float minDist, float spawnY)
    {
        if (prefabs == null || prefabs.Length == 0 || count <= 0) return;

        int spawned = 0;
        int maxAttempts = count * 20;

        for (int i = 0; i < maxAttempts && spawned < count; i++)
        {
            float x = Random.Range(-(mapSize - borderTreeDepth), mapSize - borderTreeDepth);
            float z = Random.Range(-(mapSize - borderTreeDepth), mapSize - borderTreeDepth);
            Vector3 spawnPos = new Vector3(mapSpawn.position.x + x, spawnY, mapSpawn.position.z + z);

            if (Vector3.Distance(spawnPos, mapSpawn.position) < playerSafeZone) continue;

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
            obj.transform.localScale = Vector3.one * Random.Range(minScale, maxScale);
            spawned++;
        }
    }

    IEnumerator SpawnWaves()
    {
        int enemiesPerWave = Mathf.CeilToInt((float)enemyCount / waveCount);

        for (int wave = 0; wave < waveCount; wave++)
        {
            if (wave > 0)
                yield return new WaitForSeconds(timeBetweenWaves);

            int maxThisWave = (wave == waveCount - 1)
                ? enemyCount - (wave * enemiesPerWave)
                : enemiesPerWave;

            for (int i = 0; i < maxThisWave; i++)
            {
                SpawnOneEnemy();
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }

        wavesFinished = true;

        if (enemiesDead >= totalEnemiesSpawned && totalEnemiesSpawned > 0)
            OnAllEnemiesDead();
    }

    void SpawnOneEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        int maxAttempts = 30;
        for (int i = 0; i < maxAttempts; i++)
        {
            float x = Random.Range(-(mapSize - borderTreeDepth), mapSize - borderTreeDepth);
            float z = Random.Range(-(mapSize - borderTreeDepth), mapSize - borderTreeDepth);
            Vector3 pos = new Vector3(mapSpawn.position.x + x, enemySpawnY, mapSpawn.position.z + z);

            if (Vector3.Distance(pos, player.position) < playerSafeZone + 5f) continue;

            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Instantiate(prefab, pos, Quaternion.identity, mapParent.transform);
            totalEnemiesSpawned++;
            return;
        }
    }
}
