using UnityEngine;
using UnityEngine.Rendering;

public class AlienShip : Obstacle
{
    private PlayerController player;
    public float predictionTime = 0.5f; // How far into the future to predict the player's position
    public float avoidanceRadius = 1.0f; // Radius to check for nearby obstacles
    public LayerMask obstacleLayer; // Layer containing obstacles
    public float avoidanceStrength = 1.5f; // Strength of the avoidance steering

    protected override void Start()
    {
        base.Start();
        speed = 1.5f;
        // Find the player in the scene
        player = Object.FindAnyObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("AlienShip could not find PlayerController in the scene!");
        }

        // Ensure the obstacle layer is set
        if (obstacleLayer.value == 0)
        {
            Debug.LogWarning("Obstacle Layer not set on AlienShip. It won't avoid obstacles.");
        }
    }

    protected override void Update()
    {
        // Create a debug circle to visualize the avoidance radius
        DrawDebugCircle(transform.position, avoidanceRadius, Color.red);

        if (player != null)
        {
            AvoidObstacles();
            InterceptPlayer();
        }
        else
        {
            base.Update();
        }
    }

    void AvoidObstacles()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, avoidanceRadius, obstacleLayer);
        Debug.Log($"Detected {hitColliders.Length} obstacles within avoidance radius.");
        if (hitColliders.Length > 0)
        {
            Vector3 avoidanceDirection = Vector3.zero;
            Vector3 closestPoint = Vector3.positiveInfinity;
            float closestDistanceSqr = Mathf.Infinity;

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject != gameObject && hitCollider.GetComponent<AlienShip>() != null) // Avoid other AlienShip instances
                {
                    Vector3 obstacleClosest = hitCollider.ClosestPoint(transform.position);
                    float distanceSqr = (transform.position - obstacleClosest).sqrMagnitude;

                    if (distanceSqr < closestDistanceSqr)
                    {
                        closestDistanceSqr = distanceSqr;
                        closestPoint = obstacleClosest;
                    }

                    // Steer away from the obstacle
                    Vector3 awayDirection = (transform.position - obstacleClosest).normalized;
                    avoidanceDirection += awayDirection;
                }
            }

            // If obstacles are detected, prioritize avoidance
            if (avoidanceDirection != Vector3.zero)
            {
                // Gradually blend the avoidance direction with the current direction
                direction = Vector3.Lerp(direction, avoidanceDirection.normalized, avoidanceStrength * Time.deltaTime).normalized;
                Debug.Log($"Avoiding obstacle, new direction: {direction}");
                return; // Exit early to prioritize avoidance
            }
        }
    }

    void InterceptPlayer()
    {
        // Get player's current position and velocity (we'll approximate velocity)
        Vector3 playerPos = player.transform.position;
        Vector3 playerVelocity = Vector3.zero;

        // A very simple velocity approximation: current position - last position
        if (Time.deltaTime > 0)
        {
            playerVelocity = (playerPos - player.transform.position) / Time.deltaTime;
        }

        // Predict player's position in the future
        Vector3 predictedPosition = playerPos + playerVelocity * predictionTime;

        // Calculate the direction to the predicted position
        Vector3 targetDirection = (predictedPosition - transform.position).normalized;

        // Blend avoidance and target direction (prioritizing avoidance if active)
        if (direction == Vector3.zero) // If not avoiding, target player
        {
            direction = targetDirection;
        }
        else
        {
            // Blend the directions to smoothly transition between avoidance and pursuit
            direction = Vector3.Lerp(direction, targetDirection, Time.deltaTime * speed).normalized;
        }

        // Move towards the target direction
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        RotateTowardsDirection();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Alien Ship destroyed!");
            Destroy(gameObject);
        }
    }

    private void DrawDebugCircle(Vector3 center, float radius, Color color)
    {
        int segments = 36;
        float angle = 0f;
        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            Vector3 start = new Vector3(x, y, 0) + center;
            angle += 2 * Mathf.PI / segments;
            x = Mathf.Cos(angle) * radius;
            y = Mathf.Sin(angle) * radius;
            Vector3 end = new Vector3(x, y, 0) + center;
            Debug.DrawLine(start, end, color);
        }
    }
}
