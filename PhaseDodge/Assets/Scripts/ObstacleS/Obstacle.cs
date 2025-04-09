using UnityEngine;

public class Obstacle : MonoBehaviour
{
    protected float speed = 0.5f;
    protected Camera mainCamera;
    protected Vector3 direction;
    protected bool hasEnteredScreen = false;
    protected PolygonCollider2D obstacleCollider2D;
    [HideInInspector] public GameObject originalPrefab; // Store the original prefab (assigned by ObstacleSpawner)

    protected virtual void Start()
    {
        mainCamera = Camera.main;
        obstacleCollider2D = GetComponent<PolygonCollider2D>();
        if (obstacleCollider2D == null)
        {
            Debug.LogError("Obstacle is missing a PolygonCollider2D component. Disabling the obstacle.");
            gameObject.SetActive(false); // Disable the obstacle if collider is missing
            return;
        }
        Debug.Log("Obstacle: Start - " + gameObject.name + " (originalPrefab: " + (originalPrefab != null ? originalPrefab.name : "null") + ")");
    }

    protected virtual void Update()
    {
        // Move the obstacle in the specified direction
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Check if the obstacle has left the screen
        if (hasEnteredScreen && IsFullyOffScreen())
        {
            ReturnToSpawner();
        }
        else if (!hasEnteredScreen && IsOnScreen())
        {
            hasEnteredScreen = true;
        }
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction.normalized;
        RotateTowardsDirection();
    }

    protected bool IsFullyOffScreen()
    {
        if (obstacleCollider2D == null) return false;

        Bounds bounds = obstacleCollider2D.bounds;
        Vector3 minScreenBounds = mainCamera.WorldToScreenPoint(bounds.min);
        Vector3 maxScreenBounds = mainCamera.WorldToScreenPoint(bounds.max);

        return maxScreenBounds.y < 0 || minScreenBounds.y > Screen.height || maxScreenBounds.x < 0 || minScreenBounds.x > Screen.width;
    }

    protected bool IsOnScreen()
    {
        if (obstacleCollider2D == null) return false;

        Bounds bounds = obstacleCollider2D.bounds;
        Vector3 minScreenPoint = mainCamera.WorldToViewportPoint(bounds.min);
        Vector3 maxScreenPoint = mainCamera.WorldToViewportPoint(bounds.max);

        // Check if the bounds rectangle overlaps with the viewport (0,0 to 1,1)
        return maxScreenPoint.x > 0 && minScreenPoint.x < 1 && maxScreenPoint.y > 0 && minScreenPoint.y < 1;
    }

    protected void RotateTowardsDirection()
    {
        if (direction == Vector3.zero) return;

        transform.up = direction;
    }

    public virtual void OnObjectSpawn()
    {
        Debug.Log("Obstacle: OnObjectSpawn - " + gameObject.name);
        hasEnteredScreen = false;
    }

    public virtual void OnObjectDespawn()
    {
        Debug.Log("Obstacle: OnObjectDespawn - " + gameObject.name);
        transform.localScale = Vector3.one; // Reset scale when despawning
    }

    protected virtual void ReturnToSpawner()
    {
        if (originalPrefab == null)
        {
            Debug.LogWarning($"Obstacle: ReturnToSpawner - originalPrefab is null for {gameObject.name}. Destroying object.");
            Destroy(gameObject);
            return;
        }

        // Return the object to the appropriate pool in ObstacleSpawner
        Debug.Log($"Obstacle: ReturnToSpawner - Returning {gameObject.name} to spawner.");
        ObstacleSpawner.Instance.DestroyObstacle(gameObject);
    }
}
