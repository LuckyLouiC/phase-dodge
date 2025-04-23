using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [HideInInspector] public GameObject originalPrefab; // Store the original prefab (assigned by ObstacleSpawner)

    protected Camera mainCamera; // Used for off-screen despawning
    protected PolygonCollider2D obstacleCollider2D;
    protected Vector3 direction;
    protected float speed = 0.5f;
    [SerializeField]protected bool hasEnteredScreen = false;

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
    }

    protected virtual void Update()
    {
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

    public void MoveObstacle()
    {   
        // Move the obstacle in the specified direction
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction.normalized;
        RotateTowardsDirection();
    }

    protected bool IsFullyOffScreen()
    {
        if (obstacleCollider2D == null) return false;

        var (minScreenBounds, maxScreenBounds) = GetScreenBoundsInViewport();

        // Debugging: Log the screen bounds to verify calculations
        //Debug.Log($"Obstacle: IsFullyOffScreen - Min: {minScreenBounds}, Max: {maxScreenBounds}");

        return maxScreenBounds.y < 0 || minScreenBounds.y > 1
            || maxScreenBounds.x < 0 || minScreenBounds.x > 1; // Corrected to use normalized viewport coordinates
    }

    protected bool IsOnScreen()
    {
        if (obstacleCollider2D == null) return false;

        var (minScreenPoint, maxScreenPoint) = GetScreenBoundsInViewport();

        // Check if the bounds rectangle overlaps with the viewport (0,0 to 1,1)
        return maxScreenPoint.x > 0 && minScreenPoint.x < 1 && maxScreenPoint.y > 0 && minScreenPoint.y < 1;
    }

    private (Vector3, Vector3) GetScreenBoundsInViewport()
    {
        if (obstacleCollider2D == null) return (Vector3.zero, Vector3.zero);

        Bounds bounds = obstacleCollider2D.bounds;
        Vector3 minScreenPoint = mainCamera.WorldToViewportPoint(bounds.min);
        Vector3 maxScreenPoint = mainCamera.WorldToViewportPoint(bounds.max);
        return (minScreenPoint, maxScreenPoint);
    }


    protected void RotateTowardsDirection()
    {
        if (direction == Vector3.zero) return;

        transform.up = direction;
    }

    public virtual void OnObjectSpawn()
    {
        hasEnteredScreen = false;
    }

    public virtual void OnObjectDespawn()
    {
        transform.localScale = Vector3.one;
    }

    protected virtual void ReturnToSpawner()
    {
        if (originalPrefab == null)
        {
            Debug.LogWarning($"Obstacle: ReturnToSpawner - originalPrefab is null for {gameObject.name}. Destroying object.");
            Destroy(gameObject);
            return;
        }

        //Debug.Log($"Obstacle: Returning {gameObject.name} to spawner.");
        ObstacleSpawner.Instance.DestroyObstacle(gameObject);
    }
}
