using UnityEngine;

public class Satellite : Obstacle
{
    public SatellitePath path;
    private int currentWaypointIndex = 0;
    public float sizeVariation; // Smaller size variation for satellites

    protected override void Start()
    {
        base.Start();
        speed = 1.0f;
    }

    public override void OnObjectSpawn()
    {
        base.OnObjectSpawn();

        transform.localScale = Vector3.one;
        transform.localScale *= sizeVariation;

        currentWaypointIndex = 0;

        if (path != null && path.waypoints != null && path.waypoints.Length > 0)
        {
            transform.position = path.waypoints[0]; // Explicitly set position to the first waypoint
            Debug.Log($"Satellite: Spawned at first waypoint: {transform.position}");
            SetDirectionToNextWaypoint();
        }
        else
        {
            Debug.LogError("Satellite: Path is null or invalid. Returning to spawner.");
            ReturnToSpawner();
        }
    }

    protected override void Update()
    {
        if (path != null && path.waypoints.Length > 1)
        {
            MoveAlongPath();
        }
        else
        {
            base.Update(); // Fallback to default behavior
        }
    }

    private void MoveAlongPath()
    {
        if (currentWaypointIndex >= path.waypoints.Length - 1)
        {
            ReturnToSpawner(); // Return to the pool when the path is complete
            return;
        }

        Vector3 targetWaypoint = path.waypoints[currentWaypointIndex + 1];
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetWaypoint) < 0.1f)
        {
            currentWaypointIndex++;
            SetDirectionToNextWaypoint();
        }
    }

    private void SetDirectionToNextWaypoint()
    {
        if (currentWaypointIndex < path.waypoints.Length - 1)
        {
            direction = (path.waypoints[currentWaypointIndex + 1] - transform.position).normalized;
            RotateTowardsDirection();
        }
    }

    public override void OnObjectDespawn()
    {
        base.OnObjectDespawn();
        // Add any cleanup logic specific to satellites here
    }
}