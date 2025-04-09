using UnityEngine;

public class AlienShip : Obstacle
{
    private PlayerController player;
    public float predictionTime = 0.5f; // How far into the future to predict the player's position
    public float steeringSpeed = 2.0f; // Speed at which the ship adjusts its direction

    protected override void Start()
    {
        base.Start();
        speed = 1.5f;
        player = Object.FindAnyObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("AlienShip: PlayerController not found in the scene!");
        }
    }

    public override void OnObjectSpawn()
    {
        base.OnObjectSpawn();
        // Reset any specific properties for alien ships here
    }

    protected override void Update()
    {
        if (player != null)
        {
            InterceptPlayer();
        }
        else
        {
            Debug.LogWarning("AlienShip: PlayerController is null. Falling back to default behavior.");
            base.Update(); // Fallback to default behavior
        }
    }

    private void InterceptPlayer()
    {
        Vector3 playerPosition = player.transform.position;
        Vector3 playerVelocity = (playerPosition - transform.position).normalized * speed;
        Vector3 predictedPosition = playerPosition + playerVelocity * predictionTime;

        // Gradually adjust the direction using steeringSpeed
        Vector3 targetDirection = (predictedPosition - transform.position).normalized;
        direction = Vector3.Lerp(direction, targetDirection, steeringSpeed * Time.deltaTime);

        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        RotateTowardsDirection();
    }

    public override void OnObjectDespawn()
    {
        base.OnObjectDespawn();
        // Add any cleanup logic specific to alien ships here
    }
}