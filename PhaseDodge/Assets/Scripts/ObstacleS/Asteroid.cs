using UnityEngine;

public class Asteroid : Obstacle
{
    public GameObject asteroidSmallPrefab;
    private bool hasSplit = false;

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
