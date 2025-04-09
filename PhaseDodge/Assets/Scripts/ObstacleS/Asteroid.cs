using UnityEngine;

public class Asteroid : Obstacle
{
    public GameObject asteroidSmallPrefab;
    private bool hasSplit = false;

    private float currentRotation;
    private int rotationDirection = 1;

    [Header("Asteroid Properties")]
    public float sizeVariation = 1.0f;
    public float sizeVariationMin = 0.50f;
    public float sizeVariationMax = 1.0f;
    public float rotationSpeed = 10.0f;
    public float rotationSpeedMin = 75.0f;
    public float rotationSpeedMax = 140.0f;

    protected override void Start()
    {
        base.Start();
        if (asteroidSmallPrefab == null)
        {
            Debug.LogError("AsteroidSmallPrefab is not assigned in the Asteroid script! Disabling asteroid.");
            gameObject.SetActive(false); // Disable the asteroid if prefab is missing
        }
    }

    public override void OnObjectSpawn()
    {
        base.OnObjectSpawn();
        hasSplit = false;

        SetRotationAndSize();

        // Adjust speed based on size variation (smaller size = higher speed)
        speed = Mathf.Lerp(1.0f, 0.3f, sizeVariation); // Larger sizeVariation results in slower speed
        //Debug.Log($"Asteroid: OnObjectSpawn - Speed set to {speed} based on size variation {sizeVariation}");
    }

    protected override void Update()
    {
        base.Update();

        // Rotate the asteroid
        currentRotation += rotationDirection * rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, currentRotation);
    }

    void SetRotationAndSize()
    {
        // Set random rotation and size variation
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax);
        sizeVariation = Random.Range(sizeVariationMin, sizeVariationMax);
        rotationDirection = Random.Range(0, 2) == 0 ? 1 : -1;
        transform.localScale *= sizeVariation;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasSplit && other.CompareTag("Obstacle"))
        {
            Debug.Log($"Asteroid: OnTriggerEnter2D - {gameObject.name} hit {other.gameObject.name}");
            SplitAsteroid();
            hasSplit = true;

            // Return this asteroid to the pool instead of destroying it
            ReturnToSpawner();
        }
    }

    void SplitAsteroid()
    {
        //Debug.Log($"Asteroid: SplitAsteroid - {gameObject.name}");
        if (asteroidSmallPrefab != null)
        {
            // Retrieve small asteroids from the pool
            GameObject smallAsteroid1 = ObstacleSpawner.Instance.GetPooledObject(asteroidSmallPrefab);
            GameObject smallAsteroid2 = ObstacleSpawner.Instance.GetPooledObject(asteroidSmallPrefab);

            if (smallAsteroid1 == null || smallAsteroid2 == null)
            {
                Debug.LogError("Asteroid: Failed to retrieve small asteroids from the pool!");
                return;
            }

            InitializeSmallAsteroid(smallAsteroid1);
            InitializeSmallAsteroid(smallAsteroid2);
            
            //Debug.Log($"Asteroid: SplitAsteroid - Spawned small asteroids: {smallAsteroid1.name}, {smallAsteroid2.name}");
        }
        else
        {
            Debug.LogError("AsteroidSmallPrefab is not assigned in the Asteroid script!");
        }
    }

    void InitializeSmallAsteroid(GameObject smallAsteroid)
    {
        // Initialize the small asteroid transform and properties
        smallAsteroid.transform.position = transform.position;
        smallAsteroid.transform.rotation = Quaternion.identity;
        smallAsteroid.GetComponent<Obstacle>().originalPrefab = asteroidSmallPrefab; // Set originalPrefab
        smallAsteroid.GetComponent<Obstacle>().SetDirection(Random.insideUnitCircle.normalized);
        smallAsteroid.GetComponent<Obstacle>().OnObjectSpawn();
    }

    public override void OnObjectDespawn()
    {
        base.OnObjectDespawn();
        Debug.Log($"Asteroid: OnObjectDespawn - {gameObject.name}");
    }
}
