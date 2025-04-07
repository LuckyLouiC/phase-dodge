using UnityEngine;
using UnityEngine.Rendering;

public class AlienShip : Obstacle
{
    // Variables for the obstacle avoidance behavior
    /*public float avoidanceRadius = 5.0f; // Radius to check for nearby obstacles
    public LayerMask obstacleLayer; // Layer containing obstacles
    public float avoidanceStrength = 0.5f; // Strength of the avoidance steering*/

    // Variables for the player interception behavior
    private PlayerController player;
    public float predictionTime = 0.5f; // How far into the future to predict the player's position
    public float moveSpeed = 1.5f;     // Movement speed, adjustable in editor
    public float turnSpeed = 5.0f;     // Turning speed, adjustable in editor

    protected override void Start()
    {
        base.Start();
        speed = moveSpeed; // Initialize base speed
        // Find the player in the scene
        player = Object.FindAnyObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("AlienShip could not find PlayerController in the scene!");
        }
    }

    protected override void Update()
    {
        if (player != null)
        {
            InterceptPlayer();
            ClampPositionWithinCameraBounds();
        }
        else
        {
            base.Update();
        }
    }

    void InterceptPlayer()
    {
        // Get player's current position and (approx.) velocity
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

        // Move towards the target direction
        MoveInDirection(targetDirection);
    }

    private void MoveInDirection(Vector3 targetDirection)
    {
        // Ensure the speed remains constant
        direction = Vector3.Lerp(direction, targetDirection, Time.deltaTime * turnSpeed).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
        RotateTowardsDirection();
    }

    private void ClampPositionWithinCameraBounds()
    {
        Vector3 position = transform.position;
        Vector3 minScreenBounds = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, transform.position.z - mainCamera.transform.position.z));
        Vector3 maxScreenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, transform.position.z - mainCamera.transform.position.z));

        position.x = Mathf.Clamp(position.x, minScreenBounds.x, maxScreenBounds.x);
        position.y = Mathf.Clamp(position.y, minScreenBounds.y, maxScreenBounds.y);

        transform.position = position;
    }

    public override void OnObjectSpawn()
    {
        base.OnObjectSpawn();
        // Reset properties like position, rotation, etc.
    }

    public override void OnObjectDespawn()
    {
        base.OnObjectDespawn();
        // Stop any particle effects or other temporary effects
    }

    /*private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Alien Ship destroyed!");
            Destroy(gameObject);
        }
    }*/

    /*private void DrawDebugCircle(Vector3 center, float radius, Color color)
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
    }*/

    /*void AvoidObstacles()
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
                if (hitCollider.gameObject != gameObject && hitCollider.GetComponent<AlienShip>() == null) // Avoid other AlienShip instances
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
                MoveInDirection();
                return; // Exit early to prioritize avoidance
            }
        }
    }*/

}