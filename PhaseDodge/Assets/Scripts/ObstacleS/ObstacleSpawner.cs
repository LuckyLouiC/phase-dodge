using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Collections;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public GameObject satellitePrefab;
    public GameObject alienShipPrefab;
    public float asteroidSpawnRate = 1.0f;
    public float satelliteSpawnRate = 2.0f; // More rare
    public float alienShipSpawnRate = 3.0f; // Define as needed

    public SatellitePath[] satellitePaths;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(SpawnAsteroids());
        StartCoroutine(SpawnSatellites());
        StartCoroutine(SpawnAlienShips());
    }

    private IEnumerator SpawnAsteroids()
    {
        while (true)
        {
            yield return new WaitForSeconds(asteroidSpawnRate);
            SpawnObstacle(asteroidPrefab);
        }
    }

    private IEnumerator SpawnSatellites()
    {
        while (true)
        {
            yield return new WaitForSeconds(satelliteSpawnRate);
            SpawnObstacle(satellitePrefab);
        }
    }

    private IEnumerator SpawnAlienShips()
    {
        while (true)
        {
            yield return new WaitForSeconds(alienShipSpawnRate);
            SpawnObstacle(alienShipPrefab);
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

        if (obstaclePrefab == satellitePrefab && satellitePaths.Length > 0)
        {
            Satellite satelliteComponent = obstacle.GetComponent<Satellite>();
            satelliteComponent.path = satellitePaths[Random.Range(0, satellitePaths.Length)];
        }
    }
}
