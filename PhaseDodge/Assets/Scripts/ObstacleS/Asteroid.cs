using UnityEngine;

public class Asteroid : Obstacle
{
    [Header("Asteroid Properties")]
    public float sizeVariation = 1.0f;
    public float sizeVariationMin = 0.50f;
    public float sizeVariationMax = 1.0f;
    public float rotationSpeed = 10.0f;
    public float rotationSpeedMin = 75.0f;
    public float rotationSpeedMax = 140.0f;

    public GameObject asteroidSmallPrefab;

    private float currentRotation;
    private int rotationDirection = 1;
    private bool hasSplit = false;

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

        mineTime = sizeVariation * 3f; // Adjust mine time based on size variation
    }

    protected override void Update()
    {
        base.Update();

        MoveObstacle();

        RotateAsteroid();
    }

    private void SetRotationAndSize()
    {
        // Set random rotation and size variation
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax);
        sizeVariation = Random.Range(sizeVariationMin, sizeVariationMax);
        rotationDirection = Random.Range(0, 2) == 0 ? 1 : -1;
        transform.localScale *= sizeVariation;
    }

    private void RotateAsteroid()
    {
        // Rotate the asteroid around its center
        currentRotation += rotationSpeed * Time.deltaTime * rotationDirection;
        transform.rotation = Quaternion.Euler(0, 0, currentRotation);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasSplit && other.CompareTag("Obstacle"))
        {
            SplitAsteroid();
            hasSplit = true;

            // Return this asteroid to the pool instead of destroying it
            ReturnToSpawner();
        }
    }

    void SplitAsteroid()
    {
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
    }
}
