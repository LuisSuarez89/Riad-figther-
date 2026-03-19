using System.Collections.Generic;
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
    [SerializeField] private int maxElementsPerWave = 2;

    [Header("Distancias")]
    [SerializeField] private float minSpawnAhead = 45f;
    [SerializeField] private float maxSpawnAhead = 70f;

    [Header("Probabilidades")]
    [SerializeField] [Range(0f, 1f)] private float fuelSpawnChance = 0.16f;
    [SerializeField] [Range(0f, 1f)] private float obstacleChance = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float trafficMovingChance = 0.65f;

    [Header("Velocidad tráfico")]
    [SerializeField] private float minTrafficSpeed = 8f;
    [SerializeField] private float maxTrafficSpeed = 16f;

    private float timer;

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver || player == null)
        {
            return;
        }

        timer += Time.deltaTime;
        float adjustedSpawnRate = spawnEverySeconds / Mathf.Max(GameManager.Instance.DifficultyMultiplier, 0.65f);
        if (timer >= adjustedSpawnRate)
        {
            timer = 0f;
            SpawnWave();
        }
    }

    private void SpawnWave()
    {
        int elementsToSpawn = Random.Range(1, maxElementsPerWave + 1);
        List<int> usedLanes = new List<int>();

        bool shouldSpawnFuel = fuelPickupPrefab != null &&
                               GameManager.Instance.CurrentFuel < 65f &&
                               Random.value <= fuelSpawnChance;

        if (shouldSpawnFuel)
        {
            int fuelLane = GetAvailableLane(usedLanes);
            if (fuelLane >= 0)
            {
                usedLanes.Add(fuelLane);
                SpawnFuelInLane(fuelLane);
                elementsToSpawn = Mathf.Max(1, elementsToSpawn - 1);
            }
        }

        for (int i = 0; i < elementsToSpawn; i++)
        {
            int lane = GetAvailableLane(usedLanes);
            if (lane < 0)
            {
                break;
            }

            usedLanes.Add(lane);
            SpawnHazardInLane(lane);
        }
    }

    private void SpawnFuelInLane(int lane)
    {
        float z = player.position.z + Random.Range(minSpawnAhead, maxSpawnAhead);
        float x = GetLaneX(lane);
        Instantiate(fuelPickupPrefab, new Vector3(x, 0.5f, z), Quaternion.identity);
    }

    private void SpawnHazardInLane(int lane)
    {
        float z = player.position.z + Random.Range(minSpawnAhead, maxSpawnAhead);
        float x = GetLaneX(lane);

        bool spawnObstacle = Random.value <= obstacleChance;
        GameObject prefab = PickRandom(spawnObstacle ? obstaclePrefabs : trafficPrefabs);
        if (prefab == null)
        {
            return;
        }

        GameObject spawned = Instantiate(prefab, new Vector3(x, 0f, z), Quaternion.identity);
        SetupForwardDestroyer(spawned);

        if (!spawnObstacle && Random.value <= trafficMovingChance)
        {
            TrafficMover mover = spawned.GetComponent<TrafficMover>();
            if (mover == null)
            {
                mover = spawned.AddComponent<TrafficMover>();
            }

            float difficulty = GameManager.Instance != null ? GameManager.Instance.DifficultyMultiplier : 1f;
            float trafficSpeed = Random.Range(minTrafficSpeed, maxTrafficSpeed) * difficulty;
            mover.Initialize(player, trafficSpeed);
        }
    }

    private void SetupForwardDestroyer(GameObject target)
    {
        ForwardDestroyer destroyer = target.GetComponent<ForwardDestroyer>();
        if (destroyer == null)
        {
            destroyer = target.AddComponent<ForwardDestroyer>();
        }

        destroyer.SetPlayer(player);
    }

    private int GetAvailableLane(List<int> usedLanes)
    {
        List<int> availableLanes = new List<int>();
        for (int lane = 0; lane < laneCount; lane++)
        {
            if (!usedLanes.Contains(lane))
            {
                availableLanes.Add(lane);
            }
        }

        if (availableLanes.Count == 0)
        {
            return -1;
        }

        return availableLanes[Random.Range(0, availableLanes.Count)];
    }

    private float GetLaneX(int lane)
    {
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
