// Example: Asteroid.cs
using UnityEngine;

public class Asteroid : Obstacle
{
    public GameObject asteroidSmallPrefab;
    private bool hasSplit = false;

    private int rotationDirection = 1;
    private float currentRotation;
    public float rotationSpeed = 10.0f; // Adjustable in editor
    public float sizeVariation = 1.0f; // Adjustable in editor
    public float rotationSpeedMin = 75.0f;
    public float rotationSpeedMax = 140.0f;
    public float sizeVariationMin = 0.50f;
    public float sizeVariationMax = 1.0f;

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
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        // Set random rotation and size variation
        rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax);
        sizeVariation = Random.Range(sizeVariationMin, sizeVariationMax);
        rotationDirection = Random.Range(0, 2) == 0 ? 1 : -1;
        transform.localScale *= sizeVariation;
    }

    protected override void Update()
    {
        base.Update();

        // Rotate the asteroid
        currentRotation += rotationDirection * rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, currentRotation);
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
        Debug.Log($"Asteroid: SplitAsteroid - {gameObject.name}");
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

            // Initialize the small asteroids
            smallAsteroid1.transform.position = transform.position;
            smallAsteroid1.transform.rotation = Quaternion.identity;
            smallAsteroid1.GetComponent<Obstacle>().originalPrefab = asteroidSmallPrefab; // Set originalPrefab
            smallAsteroid1.GetComponent<Obstacle>().SetDirection(Random.insideUnitCircle.normalized);
            smallAsteroid1.GetComponent<Obstacle>().OnObjectSpawn();

            smallAsteroid2.transform.position = transform.position;
            smallAsteroid2.transform.rotation = Quaternion.identity;
            smallAsteroid2.GetComponent<Obstacle>().originalPrefab = asteroidSmallPrefab; // Set originalPrefab
            smallAsteroid2.GetComponent<Obstacle>().SetDirection(Random.insideUnitCircle.normalized);
            smallAsteroid2.GetComponent<Obstacle>().OnObjectSpawn();

            Debug.Log($"Asteroid: SplitAsteroid - Spawned small asteroids: {smallAsteroid1.name}, {smallAsteroid2.name}");
        }
        else
        {
            Debug.LogError("AsteroidSmallPrefab is not assigned in the Asteroid script!");
        }
    }

    public override void OnObjectDespawn()
    {
        base.OnObjectDespawn();
        Debug.Log($"Asteroid: OnObjectDespawn - {gameObject.name}");
    }
}
