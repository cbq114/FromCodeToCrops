using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public GameObject[] fishPrefabs; // Kéo prefab cá vào đây
    public int numberOfFish = 10;
    public float spawnRadiusX = 3f;
    public float spawnRadiusY = 2f;

    void Start()
    {
        for (int i = 0; i < numberOfFish; i++)
        {
            SpawnFish();
        }
    }

    void SpawnFish()
    {
        Vector3 randomPos = transform.position + new Vector3(
            Random.Range(-spawnRadiusX, spawnRadiusX),
            Random.Range(-spawnRadiusY, spawnRadiusY),
            0f
        );

        int prefabIndex = Random.Range(0, fishPrefabs.Length);
        Instantiate(fishPrefabs[prefabIndex], randomPos, Quaternion.identity, transform);
    }
}
