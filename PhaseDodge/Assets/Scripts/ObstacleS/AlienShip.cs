using UnityEngine;

public class AlienShip : Obstacle
{
    private PlayerController player;

    [Header("Alien Ship Properties")]
    public float predictionTime = 0.5f; // How far into the future to predict the player's position
    public float steeringSpeed = 2.0f; // Speed at which the ship adjusts its direction
    public float sizeVariation = 1.0f; // Adjustable in editor

    [SerializeField]private float pursueDistance = 5.0f;

    protected override void Start()
    {
        base.Start();
        speed = 0.75f;
        player = Object.FindAnyObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("AlienShip: PlayerController not found in the scene!");
        }
    }

    protected override void Update()
    {
        float playerDistance = Vector3.Distance(player.transform.position, transform.position);
        if (player != null && (playerDistance <= pursueDistance))
        {
            InterceptPlayer();
        }
        else
        {
            Debug.LogWarning("AlienShip: PlayerController is null. Falling back to default behavior.");
            MoveObstacle();
            RotateTowardsDirection();
        }
        base.Update(); // Fallback to default behavior
    }

    public override void OnObjectSpawn()
    {
        base.OnObjectSpawn();
        // Reset any specific properties for alien ships here
        ResetTransform();
    }

    private void ResetTransform()
    {
        transform.localScale = Vector3.one;
        transform.localScale *= sizeVariation;
    }

    private void InterceptPlayer()
    {
        Vector3 predictedPosition = GetPlayerPredictedPosition();
        SteerTowardsTarget(predictedPosition);
        RotateTowardsDirection();
        MoveObstacle();
    }

    private Vector3 GetPlayerPredictedPosition()
    {
        if (player == null)
        {
            Debug.LogError("AlienShip: PlayerController is null. Cannot get player position.");
            return Vector3.zero; // Return a default Vector3 value to fix CS0126  
        }
        Vector3 playerPosition = player.transform.position;
        Vector3 playerVelocity = (playerPosition - transform.position).normalized * speed;
        return playerPosition + playerVelocity * predictionTime;
    }

    private void SteerTowardsTarget(Vector3 targetPosition)
    {
        // Gradually adjust the direction using steeringSpeed
        Vector3 targetDirection = (targetPosition - transform.position).normalized;
        float maxSteerAngle = 5.0f * Time.deltaTime; // Maximum angle to steer in one frame
        direction = Vector3.RotateTowards(direction, targetDirection, maxSteerAngle, 0.0f);
    }

    public override void OnObjectDespawn()
    {
        base.OnObjectDespawn();
        // Add any cleanup logic specific to alien ships here
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, pursueDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, GetPlayerPredictedPosition());
    }
}