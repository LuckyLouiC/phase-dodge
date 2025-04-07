using System.Threading;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public GameObject satellitePrefab;
    public GameObject alienShipPrefab;
    public float asteroidSpawnRate = 2.0f;
    public float satelliteSpawnRate = 2.5f;
    public float alienShipSpawnRate = 4.0f;

    public SatellitePath[] satellitePaths;

    private Camera mainCamera;
    private Coroutine asteroidCoroutine;
    private Coroutine satelliteCoroutine;
    private Coroutine alienShipCoroutine;

    // Prefabs to use for pooling
    private List<GameObject> asteroidPool = new List<GameObject>();
    private List<GameObject> satellitePool = new List<GameObject>();
    private List<GameObject> alienShipPool = new List<GameObject>();
    private int poolSize = 10; // Initial pool size for each type

    void Start()
    {
        mainCamera = Camera.main;
        SetStage(1);
        InitializePools();
    }

    void InitializePools()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject asteroid = Instantiate(asteroidPrefab);
            asteroid.SetActive(false);
            asteroidPool.Add(asteroid);

            GameObject satellite = Instantiate(satellitePrefab);
            satellite.SetActive(false);
            satellitePool.Add(satellite);

            GameObject alienShip = Instantiate(alienShipPrefab);
            alienShip.SetActive(false);
            alienShipPool.Add(alienShip);
        }
    }

    public void SetStage(int stage)
    {
        if (asteroidCoroutine != null) StopCoroutine(asteroidCoroutine);
        if (satelliteCoroutine != null) StopCoroutine(satelliteCoroutine);
        if (alienShipCoroutine != null) StopCoroutine(alienShipCoroutine);

        switch (stage)
        {
            case 1:
                asteroidSpawnRate = 2.0f;
                asteroidCoroutine = StartCoroutine(SpawnAsteroids());
                break;
            case 2:
                asteroidSpawnRate = 1.5f;
                satelliteSpawnRate = 3.0f;
                asteroidCoroutine = StartCoroutine(SpawnAsteroids());
                satelliteCoroutine = StartCoroutine(SpawnSatellites());
                break;
            case 3:
                asteroidSpawnRate = 1.0f;
                satelliteSpawnRate = 2.5f;
                alienShipSpawnRate = 4.0f;
                asteroidCoroutine = StartCoroutine(SpawnAsteroids());
                satelliteCoroutine = StartCoroutine(SpawnSatellites());
                alienShipCoroutine = StartCoroutine(SpawnAlienShips());
                break;
        }
    }

    private IEnumerator SpawnAsteroids()
    {
        while (true)
        {
            yield return new WaitForSeconds(asteroidSpawnRate);
            SpawnObstacle(asteroidPrefab, true); // True indicates it's from the pool
        }
    }

    private IEnumerator SpawnSatellites()
    {
        while (true)
        {
            yield return new WaitForSeconds(satelliteSpawnRate);
            SpawnObstacle(satellitePrefab, true);
        }
    }

    private IEnumerator SpawnAlienShips()
    {
        while (true)
        {
            yield return new WaitForSeconds(alienShipSpawnRate);
            SpawnObstacle(alienShipPrefab, true);
        }
    }

    void SpawnObstacle(GameObject obstaclePrefab, bool fromPool = false)
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

        spawnPosition.z = 0;
        GameObject obstacle = fromPool ? ObjectPool.Instance.GetPooledObject(obstaclePrefab) : Instantiate(obstaclePrefab);
        obstacle.transform.position = spawnPosition;
        obstacle.transform.rotation = Quaternion.identity; // Reset rotation
        obstacle.SetActive(true); // Ensure it's active

        Obstacle obstacleComponent = obstacle.GetComponent<Obstacle>();
        obstacleComponent.SetDirection(direction);
        obstacleComponent.isPooled = fromPool; // Set the isPooled flag
        obstacleComponent.OnObjectSpawn(); // Call OnObjectSpawn

        if (obstaclePrefab == satellitePrefab && satellitePaths.Length > 0)
        {
            Satellite satelliteComponent = obstacle.GetComponent<Satellite>();
            satelliteComponent.path = satellitePaths[Random.Range(0, satellitePaths.Length)];
        }
    }

    void DestroyObstacle(GameObject obstacle)
    {
        obstacle.GetComponent<Obstacle>().OnObjectDespawn();
        obstacle.SetActive(false); // Deactivate instead of Destroy
        ObjectPool.Instance.ReturnToPool(obstacle.gameObject, obstacle);
    }
}