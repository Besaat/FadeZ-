using UnityEngine;

public class TreeLeafSpawner : MonoBehaviour
{
    public GameObject leafPrefab;

    public float minTime = 1f;
    public float maxTime = 4f;

    public Vector3 spawnArea = new Vector3(2f, 0f, 2f);

    void Start()
    {
        StartCoroutine(SpawnLeaves());
    }

    System.Collections.IEnumerator SpawnLeaves()
    {
        while (true)
        {
            float wait = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(wait);

            SpawnLeaf();
        }
    }

   void SpawnLeaf()
    {
         Vector3 randomOffset = new Vector3(
        Random.Range(-spawnArea.x, spawnArea.x),
        0,
        Random.Range(-spawnArea.z, spawnArea.z)
         );

        Vector3 spawnPos = transform.position + randomOffset;

        GameObject leaf = Instantiate(leafPrefab, spawnPos, Quaternion.identity);

        float scale = Random.Range(0.8f, 1.5f);
        leaf.transform.localScale = new Vector3(scale, scale, scale);
    }
}