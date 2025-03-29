using System.Threading;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public float spawnRate = 1.0f;
    
    private float spawnTimer;
    private Camera mainCamera;

    void Start()
    {
        spawnTimer = spawnRate;
        mainCamera = Camera.main;
    }

    // Update is called once per frame
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
        float screenX = Random.Range(0, Screen.width);
        Vector3 spawnPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenX, Screen.height, mainCamera.nearClipPlane));
        spawnPosition.z = 0;
        Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
    }
}
