using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public GameObject satellitePrefab;
    public GameObject alienShipPrefab;
    public float asteroidSpawnRate = 1.0f;
    public float satelliteSpawnRate = 2.0f; // More rare
    public float alienShipSpawnRate = 3.0f; // Define as needed

    private float asteroidSpawnTimer;
    private float satelliteSpawnTimer;
    private float alienShipSpawnTimer;
    private Camera mainCamera;

    void Start()
    {
        SetTimers();
        mainCamera = Camera.main;
    }

    void Update()
    {
        DecrementTimers();

        CheckTimeToSpawn();

    }

    private void SetTimers()
    {
        asteroidSpawnTimer = asteroidSpawnRate;
        satelliteSpawnTimer = satelliteSpawnRate;
        alienShipSpawnTimer = alienShipSpawnRate;
    }

    private void DecrementTimers()
    {
        asteroidSpawnTimer -= Time.deltaTime;
        satelliteSpawnTimer -= Time.deltaTime;
        alienShipSpawnTimer -= Time.deltaTime;
    }

    private void CheckTimeToSpawn()
    {
        if (asteroidSpawnTimer <= 0)
        {
            SpawnObstacle(asteroidPrefab);
            asteroidSpawnTimer = asteroidSpawnRate;
        }

        if (satelliteSpawnTimer <= 0)
        {
            SpawnObstacle(satellitePrefab);
            satelliteSpawnTimer = satelliteSpawnRate;
        }

        if (alienShipSpawnTimer <= 0)
        {
            SpawnObstacle(alienShipPrefab);
            alienShipSpawnTimer = alienShipSpawnRate;
        }
    }

    void SpawnObstacle(GameObject obstaclePrefab)
    {
        Vector3 spawnPosition = Vector3.zero;
        Vector3 direction = Vector3.zero;
        int edge = Random.Range(0, 4);

        switch (edge)
        {
            case 0: // Top
                spawnPosition = mainCamera.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), Screen.height, mainCamera.nearClipPlane));
                direction = Vector3.down;
                break;
            case 1: // Bottom
                spawnPosition = mainCamera.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), 0, mainCamera.nearClipPlane));
                direction = Vector3.up;
                break;
            case 2: // Left
                spawnPosition = mainCamera.ScreenToWorldPoint(new Vector3(0, Random.Range(0, Screen.height), mainCamera.nearClipPlane));
                direction = Vector3.right;
                break;
            case 3: // Right
                spawnPosition = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Random.Range(0, Screen.height), mainCamera.nearClipPlane));
                direction = Vector3.left;
                break;
        }

        spawnPosition.z = 0; // Ensure the obstacle is at the correct depth
        GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
        obstacle.GetComponent<Obstacle>().SetDirection(direction);
    }
}
