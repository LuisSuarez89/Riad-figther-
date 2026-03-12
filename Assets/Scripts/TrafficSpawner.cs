using UnityEngine;

public class TrafficSpawner : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private GameObject[] trafficPrefabs;
    [SerializeField] private GameObject fuelPickupPrefab;

    [Header("Frecuencia")]
    [SerializeField] private float spawnEverySeconds = 1.2f;
    [SerializeField] private float laneWidth = 3.3f;
    [SerializeField] private int laneCount = 3;

    [Header("Distancias")]
    [SerializeField] private float minSpawnAhead = 45f;
    [SerializeField] private float maxSpawnAhead = 70f;

    [Header("Probabilidades")]
    [SerializeField] [Range(0f, 1f)] private float fuelSpawnChance = 0.16f;
    [SerializeField] [Range(0f, 1f)] private float obstacleChance = 0.5f;

    private float timer;

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver || player == null)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= spawnEverySeconds)
        {
            timer = 0f;
            SpawnElement();
        }
    }

    private void SpawnElement()
    {
        float z = player.position.z + Random.Range(minSpawnAhead, maxSpawnAhead);
        float x = GetRandomLaneX();

        if (Random.value <= fuelSpawnChance && fuelPickupPrefab != null)
        {
            Instantiate(fuelPickupPrefab, new Vector3(x, 0.5f, z), Quaternion.identity);
            return;
        }

        bool spawnObstacle = Random.value <= obstacleChance;
        GameObject prefab = PickRandom(spawnObstacle ? obstaclePrefabs : trafficPrefabs);

        if (prefab != null)
        {
            Instantiate(prefab, new Vector3(x, 0f, z), Quaternion.identity);
        }
    }

    private float GetRandomLaneX()
    {
        int lane = Random.Range(0, laneCount);
        float leftMost = -((laneCount - 1) * laneWidth) * 0.5f;
        return leftMost + lane * laneWidth;
    }

    private GameObject PickRandom(GameObject[] list)
    {
        if (list == null || list.Length == 0)
        {
            return null;
        }

        return list[Random.Range(0, list.Length)];
    }
}
