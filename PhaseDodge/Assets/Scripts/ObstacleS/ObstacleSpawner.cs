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

        // Initialize object pools
        InitializeObjectPools();
    }

    private void InitializeObjectPools()
    {
        asteroidPool = CreateObjectPool(asteroidPrefab);
        smallAsteroidPool = CreateObjectPool(smallAsteroidPrefab);
        satellitePool = CreateObjectPool(satellitePrefab);
        alienShipPool = CreateObjectPool(alienShipPrefab);
    }

    private void Start()
    {
        mainCamera = Camera.main;
        PreWarmPools(); // Pre-warm object pools
        SetStage(1); // Start with stage 1
    }

    private void PreWarmPools()
    {
        // Pre-warm all pools to avoid runtime instantiation overhead
        PreWarmPool(asteroidPool, asteroidPrefab);
        PreWarmPool(smallAsteroidPool, smallAsteroidPrefab);
        PreWarmPool(satellitePool, satellitePrefab);
        PreWarmPool(alienShipPool, alienShipPrefab);
    }

    private void PreWarmPool(ObjectPool<GameObject> pool, GameObject prefab)
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Release(obj);
        }
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

        return pool;
    }

    public void SetStage(int stage)
    {
        // Stop all existing coroutines
        StopAllCoroutines();

        // Define spawn rates and coroutines for each stage
        var stageConfig = new Dictionary<int, (float asteroidRate, float satelliteRate, float alienShipRate)>
        {
            // Stage | Asteroid Rate | Satellite Rate | Alien Ship Rate
            { 1,     (2.0f,             0f,             0f) },
            { 2,     (2.0f,             5.0f,           0f) },
            { 3,     (2.0f,             5.0f,           10.0f) },
            { 4,     (0f,               2.0f,           0f) }
        };

        if (stageConfig.TryGetValue(stage, out var config))
        {
            asteroidSpawnRate = config.asteroidRate;
            satelliteSpawnRate = config.satelliteRate;
            alienShipSpawnRate = config.alienShipRate;

            if (asteroidSpawnRate > 0) asteroidCoroutine = StartCoroutine(SpawnAsteroids());
            if (satelliteSpawnRate > 0) satelliteCoroutine = StartCoroutine(SpawnSatellites());
            if (alienShipSpawnRate > 0) alienShipCoroutine = StartCoroutine(SpawnAlienShips());
        }
        else
        {
            Debug.LogError($"ObstacleSpawner: Invalid stage {stage}.");
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
        obstacle.SetActive(true);

        // Initialize the obstacle component
        Obstacle obstacleComponent = obstacle.GetComponent<Obstacle>();
        if (obstacleComponent == null)
        {
            Debug.LogError($"ObstacleSpawner: Obstacle component missing on {obstacle.name}. Releasing object.");
            ReleaseToPool(prefab, obstacle);
            return;
        }
        obstacleComponent.originalPrefab = prefab;

        if (prefab == satellitePrefab)
        {
            InitializeSatellite(obstacle);
        }
        else
        {
            obstacleComponent.SetDirection(direction);
            obstacle.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
        }

        obstacleComponent.OnObjectSpawn();
    }

    private void InitializeSatellite(GameObject satellite)
    {
        if (satellitePaths == null || satellitePaths.Count == 0)
        {
            Debug.LogError("ObstacleSpawner: satellitePaths list is null or empty! Cannot spawn satellite.");
            satellitePool.Release(satellite);
            return;
        }

        Satellite satelliteComponent = satellite.GetComponent<Satellite>();
        SplineAnimate satelliteAnimator = satellite.GetComponent<SplineAnimate>();

        if (satelliteComponent == null || satelliteAnimator == null)
        {
            Debug.LogError($"ObstacleSpawner: Satellite or SplineAnimate component missing on {satellite.name}. Releasing object.");
            satellitePool.Release(satellite);
            return;
        }

        SplineContainer randomPath = satellitePaths[Random.Range(0, satellitePaths.Count)];
        if (randomPath == null || randomPath.Spline == null || randomPath.Spline.Count == 0)
        {
            Debug.LogError($"ObstacleSpawner: Selected randomPath is invalid for {satellite.name}. Releasing object.");
            satellitePool.Release(satellite);
            return;
        }

        satelliteAnimator.Container = randomPath;
        Vector3 splineStartPosition = randomPath.transform.TransformPoint(randomPath.Spline.ToArray()[0].Position);
        splineStartPosition.z = 0;
        satellite.transform.position = splineStartPosition;
    }

    private void ReleaseToPool(GameObject prefab, GameObject obstacle)
    {
        if (prefab == asteroidPrefab) asteroidPool.Release(obstacle);
        else if (prefab == smallAsteroidPrefab) smallAsteroidPool.Release(obstacle);
        else if (prefab == satellitePrefab) satellitePool.Release(obstacle);
        else if (prefab == alienShipPrefab) alienShipPool.Release(obstacle);
        else Destroy(obstacle);
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
}