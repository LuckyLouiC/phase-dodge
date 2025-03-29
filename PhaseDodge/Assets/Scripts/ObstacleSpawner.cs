using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public float spawnRate = 1.0f;

    private float spawnTimer;
    private Camera mainCamera;
    private Vector3 obstacleDirection;

    void Start()
    {
        spawnTimer = spawnRate;
        mainCamera = Camera.main;
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnObstacle();
            spawnTimer = spawnRate;
        }
    }

    void SpawnObstacle()
    {
        Vector3 spawnPosition = Vector3.zero;
        Vector3 screenPosition = Vector3.zero;
        int edge = Random.Range(0, 4);

        switch (edge)
        {
            case 0: // Top
                spawnPosition = mainCamera.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), Screen.height, mainCamera.nearClipPlane));
                obstacleDirection = Vector3.down;
                break;
            case 1: // Bottom
                spawnPosition = mainCamera.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), 0, mainCamera.nearClipPlane));
                obstacleDirection = Vector3.up;
                break;
            case 2: // Left
                spawnPosition = mainCamera.ScreenToWorldPoint(new Vector3(0, Random.Range(0, Screen.height), mainCamera.nearClipPlane));
                obstacleDirection = Vector3.right;
                break;
            case 3: // Right
                spawnPosition = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Random.Range(0, Screen.height), mainCamera.nearClipPlane));
                obstacleDirection = Vector3.left;
                break;
        }

        spawnPosition.z = 0; // Ensure the obstacle is at the correct depth
        GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
        obstacle.GetComponent<Obstacle>().SetDirection(obstacleDirection);
    }
}
