using UnityEngine;
using System.Collections.Generic;

public class Satellite : Obstacle
{
    public float sizeVariation; // Smaller size variation for satellites
    
    public List<OrbitSpline> orbits;
    private int currentSplineIndex = 0; // Track the current spline index
    private float splineTime;

    protected override void Start()
    {
        base.Start();
        speed = 0.45f;
    }

    public override void OnObjectSpawn()
    {
        base.OnObjectSpawn();

        // Reset the satellite's properties, apply size variation, and set the path
        transform.localScale = Vector3.one;
        transform.localScale *= sizeVariation;

        if (orbits == null || orbits.Count <= 0)
        {
            Debug.LogError("Satellite: Path is null or invalid. Returning to spawner.");
            ReturnToSpawner();
        }

        //Debug.Log($"Satellite: Spawned");
    }

    protected override void Update()
    {
        if (orbits == null || orbits.Count <= 0)
        {
            base.Update(); // Fallback to default behavior
            Debug.LogWarning($"Satellite: NULL path or path count less then 1: {orbits}{orbits.Count}");
        }

        OrbitSpline currentOrbit = orbits[currentSplineIndex];

        splineTime += speed * Time.deltaTime;

        if (splineTime >= 1f) // Check if the current spline is completed
        {
            splineTime = 0f; // Reset time for the next spline
            currentSplineIndex = (currentSplineIndex + 1) % orbits.Count; // Move to the next spline, loop back if at the end
            currentOrbit = orbits[currentSplineIndex]; // Update the current spline
        }

        // TODO: Add rotation to the satellite
        transform.position = currentOrbit.GetPoints(splineTime);
        transform.up = currentOrbit.GetPoints(splineTime + 0.01f) - transform.position; // Rotate towards the next point
    }

    public override void OnObjectDespawn()
    {
        base.OnObjectDespawn();
        // Add any cleanup logic specific to satellites here
    }
}