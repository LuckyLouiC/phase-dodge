using UnityEngine;

public class Asteroid : Obstacle
{
    public GameObject asteroidSmallPrefab;
    private bool hasSplit = false;

    private int rotationDirection = 1; // 1 for clockwise, -1 for counter-clockwise
    private float currentRotation; // Current rotation angle in degrees
    private float rotationSpeed; // Speed of rotation in degrees per second
    private float sizeVariation; // Variation in size

    // Rotation speed parameters
    [Header("RotationParameters")]
    public float rotationSpeedMin = 75.0f;
    public float rotationSpeedMax = 140.0f;
    [Header("SizeVarianceParameters")]
    public float sizeVariationMin = 0.50f;
    public float sizeVariationMax = 1.0f;


    protected override void Start()
    {
        base.Start();
        if (asteroidSmallPrefab == null)
        {
            Debug.LogError("asteroidSmallPrefab prefab is NULL in Start()!");
        }
        else
        {
            Debug.Log("asteroidSmallPrefab prefab is assigned in Start():" + asteroidSmallPrefab.name);
        }

        // Set rotation and size variation
        rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax); // Random rotation speed
        sizeVariation = Random.Range(sizeVariationMin, sizeVariationMax); // Random size variation (Between 85% and 100%)
        rotationDirection = Random.Range(0, 2) == 0 ? 1 : -1; // Randomly choose clockwise or counter-clockwise
        transform.localScale *= sizeVariation; // Apply size variation
    }

    protected override void Update()
    {
        base.Update();

        currentRotation += rotationDirection * rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, currentRotation);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasSplit && other.CompareTag("Obstacle"))
        {
            Debug.Log("Asteroid hit another obstacle!");
            SplitAsteroid();
            hasSplit = true;
            Destroy(gameObject);
        }
    }

    void SplitAsteroid()
    {
        Debug.Log("Splitting asteroid!");
        if (asteroidSmallPrefab != null)
        {
            // Spawn two smaller asteroids at the current position
            GameObject smallAsteroid1 = Instantiate(asteroidSmallPrefab, transform.position, Quaternion.identity);
            GameObject smallAsteroid2 = Instantiate(asteroidSmallPrefab, transform.position, Quaternion.identity);

            // Give them random directions
            Vector3 direction1 = Random.insideUnitCircle.normalized;
            Vector3 direction2 = Random.insideUnitCircle.normalized;

            smallAsteroid1.GetComponent<Obstacle>().SetDirection(direction1);
            smallAsteroid2.GetComponent<Obstacle>().SetDirection(direction2);
            Debug.Log("SplitAsteroid() called");
        }
        else
        {
            Debug.LogError("AsteroidSmallPrefab is not assigned in the Asteroid script!");
        }
    }
}
