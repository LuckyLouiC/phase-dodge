using System.Threading;
using UnityEngine;
using System.Collections;
using UnityEngine.Pool;
using System.Collections.Generic;
using UnityEngine.Splines;


public class ObstacleSpawner : MonoBehaviour
{
    public static ObstacleSpawner Instance { get; private set; } // Singleton instance

    private Camera mainCamera;

    [Header("Obstacle Prefabs")]
    public GameObject asteroidPrefab;
    public float asteroidSpawnRate = 2.0f;
    public GameObject smallAsteroidPrefab;
    public GameObject satellitePrefab;
    public float satelliteSpawnRate = 2.5f;
    public GameObject alienShipPrefab;
    public float alienShipSpawnRate = 10.0f;

    [Header("Satellite Path Splines")]
    public List<SplineContainer> satellitePaths;

    // Coroutines for spawning
    private Coroutine asteroidCoroutine;
    private Coroutine satelliteCoroutine;
    private Coroutine alienShipCoroutine;

    // Object pools for each type of obstacle
    private ObjectPool<GameObject> asteroidPool;
    private ObjectPool<GameObject> smallAsteroidPool;
    private ObjectPool<GameObject> satellitePool;
    private ObjectPool<GameObject> alienShipPool;

    private int poolSize = 20; // Initial pool size for each type
    private int maxPoolSize = 50; // Maximum size the pool can grow to for any prefab

    private void Awake()
    {
        // Ensure only one instance of ObstacleSpawner exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple instances of ObstacleSpawner detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        // Initialize object pools for each obstacle type
        asteroidPool = CreateObjectPool(asteroidPrefab);
        smallAsteroidPool = CreateObjectPool(smallAsteroidPrefab); // Initialize small asteroid pool
        satellitePool = CreateObjectPool(satellitePrefab);
        alienShipPool = CreateObjectPool(alienShipPrefab);
    }

    private void Start()
    {
        mainCamera = Camera.main;
        SetStage(1); // Start with stage 1
    }

    private ObjectPool<GameObject> CreateObjectPool(GameObject prefab)
    {
        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            // Create a new object
            () =>
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                return obj;
            },
            // On object retrieval from the pool
            (obj) =>
            {
                obj.SetActive(true);
                //Debug.Log($"ObjectPool: Get from pool: {obj.name}");
            },
            // On object return to the pool
            (obj) =>
            {
                obj.SetActive(false);
                //Debug.Log($"ObjectPool: Return to pool: {obj.name}");
            },
            // On object destruction
            (obj) =>
            {
                Destroy(obj);
                //Debug.Log($"ObjectPool: Destroyed {obj.name}.");
            },
            true, // Enable collection checks
            poolSize, // Default pool size
            maxPoolSize // Maximum pool size
        );

        // Pre-instantiate objects up to the initial pool size
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab); // Explicitly create a new instance
            obj.SetActive(false); // Set the object to inactive
            pool.Release(obj); // Add the new instance to the pool
            //Debug.Log($"ObjectPool: Pre-instantiated object {i + 1}/{poolSize} for {prefab.name}.");
        }

        //Debug.Log($"ObjectPool: Pre-instantiated {poolSize} objects for {prefab.name}.");
        return pool;
    }

    public void SetStage(int stage)
    {
        // Stop any existing coroutines
        if (asteroidCoroutine != null) StopCoroutine(asteroidCoroutine);
        if (satelliteCoroutine != null) StopCoroutine(satelliteCoroutine);
        if (alienShipCoroutine != null) StopCoroutine(alienShipCoroutine);

        // Start coroutines based on the stage
        switch (stage)
        {
            case 1:
                satelliteSpawnRate = 5.0f;
                asteroidSpawnRate = 2.0f;
                satelliteCoroutine = StartCoroutine(SpawnSatellites());
                asteroidCoroutine = StartCoroutine(SpawnAsteroids());
                break;
            case 2:
                asteroidSpawnRate = 2.0f;
                satelliteSpawnRate = 5.0f;
                asteroidCoroutine = StartCoroutine(SpawnAsteroids());
                satelliteCoroutine = StartCoroutine(SpawnSatellites());
                break;
            case 3:
                asteroidSpawnRate = 2.0f;
                satelliteSpawnRate = 5.0f;
               // alienShipSpawnRate = 10.0f;
                asteroidCoroutine = StartCoroutine(SpawnAsteroids());
                satelliteCoroutine = StartCoroutine(SpawnSatellites());
                //alienShipCoroutine = StartCoroutine(SpawnAlienShips());
                break;
        }
    }

    private IEnumerator SpawnAsteroids()
    {
        while (true)
        {
            yield return new WaitForSeconds(asteroidSpawnRate);
            SpawnObstacle(asteroidPrefab, asteroidPool);
        }
    }

    private IEnumerator SpawnSatellites()
    {
        while (true)
        {
            yield return new WaitForSeconds(satelliteSpawnRate);
            SpawnObstacle(satellitePrefab, satellitePool);
        }
    }

    private IEnumerator SpawnAlienShips()
    {
        while (true)
        {
            yield return new WaitForSeconds(alienShipSpawnRate);
            SpawnObstacle(alienShipPrefab, alienShipPool);
        }
    }

    private void SpawnObstacle(GameObject prefab, ObjectPool<GameObject> pool)
    {
        // Get spawn position and direction
        (Vector3 spawnPosition, Vector3 direction) = GetSpawnPositionAndDirection(prefab);

        // Get an obstacle from the pool
        GameObject obstacle = pool.Get();
        if (obstacle == null)
        {
            Debug.LogError($"ObstacleSpawner: SpawnObstacle - Failed to get object from pool for {prefab.name}. Spawning delayed.");
            return;
        }

        // Initialize the obstacle
        InitializeObstacle(obstacle, prefab, spawnPosition, direction);
    }

    private (Vector3, Vector3) GetSpawnPositionAndDirection(GameObject prefab)
    {
        Vector3 spawnPosition = Vector3.zero;
        Vector3 direction = Vector3.zero;

        // Determine spawn position and direction based on screen edges
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

        // Apply a random angle spread for asteroids
        if (prefab == asteroidPrefab)
        {
            float angleSpread = 30f;
            float randomAngle = Random.Range(-angleSpread, angleSpread);
            direction = Quaternion.Euler(0f, 0f, randomAngle) * direction;
        }

        return (spawnPosition, direction);
    }

    private void InitializeObstacle(GameObject obstacle, GameObject prefab, Vector3 spawnPosition, Vector3 direction)
    {
        obstacle.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
        obstacle.SetActive(true);

        // Initialize the obstacle component
        Obstacle obstacleComponent = obstacle.GetComponent<Obstacle>();
        obstacleComponent.SetDirection(direction);
        obstacleComponent.originalPrefab = prefab;

        // Special handling for satellites
        if (prefab == satellitePrefab && satellitePaths.Count > 0)
        {
            Satellite satelliteComponent = obstacle.GetComponent<Satellite>();
            SplineAnimate satelliteAnimator = satelliteComponent.GetComponent<SplineAnimate>();
            satelliteComponent.orbit = null; // Clear existing spline orbit

            // Assign a random path from the list of satellite paths
            SplineContainer randomPath = satellitePaths[Random.Range(0, satellitePaths.Count)];
            satelliteAnimator.Container = randomPath;
            Debug.Log($"ObstacleSpawner: Assigned orbit path to {satelliteComponent}: {satelliteAnimator.Container}");
        }

        obstacleComponent.OnObjectSpawn();
    }

    public GameObject GetPooledObject(GameObject prefab)
    {
        if (prefab == asteroidPrefab)
        {
            return asteroidPool.Get();
        }
        else if (prefab == smallAsteroidPrefab)
        {
            return smallAsteroidPool.Get();
        }
        else if (prefab == satellitePrefab)
        {
            return satellitePool.Get();
        }
        else if (prefab == alienShipPrefab)
        {
            return alienShipPool.Get();
        }
        else
        {
            Debug.LogError($"ObstacleSpawner: GetPooledObject - Unknown prefab: {prefab.name}");
            return null;
        }
    }

    public void DestroyObstacle(GameObject obstacle)
    {
        if (obstacle == null)
        {
            Debug.LogError("ObstacleSpawner: DestroyObstacle - Obstacle GameObject is null!");
        }
        Obstacle obstacleComponent = obstacle.GetComponent<Obstacle>();
        if (obstacleComponent == null)
        {
            Debug.LogError($"ObstacleSpawner: DestroyObstacle - Obstacle component is null on {obstacle.name}. Destroying object.");
            Destroy(obstacle);
        }

        obstacleComponent.OnObjectDespawn();

        // Return the obstacle to the appropriate pool
        ReleaseObstacleToPool(obstacleComponent, obstacle);
    }

    void ReleaseObstacleToPool(Obstacle obstacleComponent, GameObject obstacle)
    {

        if (obstacleComponent.originalPrefab == asteroidPrefab)
        {
            asteroidPool.Release(obstacle);
        }
        else if (obstacleComponent.originalPrefab == smallAsteroidPrefab)
        {
            smallAsteroidPool.Release(obstacle);
        }
        else if (obstacleComponent.originalPrefab == satellitePrefab)
        {
            satellitePool.Release(obstacle);
        }
        else if (obstacleComponent.originalPrefab == alienShipPrefab)
        {
            alienShipPool.Release(obstacle);
        }
        else
        {
            Debug.LogWarning($"ObstacleSpawner: DestroyObstacle - Unknown prefab for {obstacle.name}. Destroying object.");
            Destroy(obstacle);
        }
    }
}