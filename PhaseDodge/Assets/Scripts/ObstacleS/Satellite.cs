using System;
using UnityEngine;

public class Satellite : Obstacle
{
    public SatellitePath path;
    private int currentWaypointIndex = 0;

    protected override void Start()
    {
        base.Start();
        speed = 1.0f;
        if (path == null || path.waypoints.Length < 2)
        {
            Debug.LogError("Satellite is missing a valid path!");
            Destroy(gameObject);
        }
        else
        {
            // Set initial position to the first waypoint
            transform.position = path.waypoints[0];
            SetDirectionToNextWaypoint();
        }
    }

    protected override void Update()
    {
        if (path != null && path.waypoints.Length >= 2)
        {
            MoveAlongPath();
        }
        else
        {
            base.Update(); // Fallback to default behavior
        }
    }

    void MoveAlongPath()
    {
        Vector3 targetWaypoint = path.waypoints[currentWaypointIndex + 1];
        Vector3 currentPosition = transform.position;

        // Calculate the distance to the target waypoint
        float distanceToTarget = Vector3.Distance(currentPosition, targetWaypoint);

        if (distanceToTarget > 0.01f) // Use a small tolerance
        {
            // Move towards the target waypoint
            transform.position = Vector3.MoveTowards(currentPosition, targetWaypoint, speed * Time.deltaTime);
            SetDirectionToNextWaypoint(); // Update rotation
        }
        else
        {
            // Reached the target waypoint, move to the next one
            currentWaypointIndex++;
            if (currentWaypointIndex >= path.waypoints.Length - 1)
            {
                // Optionally loop the path or destroy the satellite
                Destroy(gameObject); // For now, let's destroy it at the end
                return;
            }
            SetDirectionToNextWaypoint();
        }

        // Check if fully off-screen (using base logic)
        if (hasEnteredScreen && IsFullyOffScreen())
        {
            Destroy(gameObject);
        }
        else if (!hasEnteredScreen && IsOnScreen())
        {
            hasEnteredScreen = true;
        }
    }

    void SetDirectionToNextWaypoint()
    {
        if (currentWaypointIndex < path.waypoints.Length - 1)
        {
            direction = (path.waypoints[currentWaypointIndex + 1] - transform.position).normalized;
            RotateTowardsDirection();
        }
    }
}
