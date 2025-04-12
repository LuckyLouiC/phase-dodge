using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Splines;

public class Satellite : Obstacle
{
    private SplineAnimate splineAnimator;
    public SplineContainer orbit;
    public float sizeVariation; // Smaller size variation for satellites


    private void Awake()
    {
        // Initialize the spline animator
        splineAnimator = GetComponent<SplineAnimate>();
        if (splineAnimator == null)
        {
            Debug.LogError("Satellite: SplineAnimate component is missing. Disabling the satellite.");
            gameObject.SetActive(false); // Disable the satellite if the component is missing
            return;
        }
        Debug.Log($"Satellite: SplineAnimate component found: {splineAnimator}");
    }
    protected override void Start()
    {
        base.Start();
        speed = 0.45f;
    }

    public override void OnObjectSpawn()
    {
        base.OnObjectSpawn();

        Debug.Log($"Satellite: SplineContainer (orbit): {orbit}");
        Debug.Log($"Satellite: SplineAnimate (splineAnimator): {splineAnimator}");

        // Reset the satellite's properties, apply size variation, and set the path
        transform.localScale = Vector3.one;
        transform.localScale *= sizeVariation;

        if (splineAnimator == null)
        {
            Debug.LogError("Satellite: SplineAnimator is null. Returning to spawner.");
            ReturnToSpawner();
        }
        splineAnimator.Play();
        Debug.Log($"Satellite: SplineAnimator: {splineAnimator} - Playing animation");

        Debug.Log($"Satellite: Spawned");
    }

    protected override void Update()
    {
        if (splineAnimator == null)
        {
            base.Update(); // Fallback to default behavior
            Debug.LogWarning($"Satellite: NULL SplineAnimator: {splineAnimator}");
        }

        // Move the satellite along the path
    }

    public override void OnObjectDespawn()
    {
        base.OnObjectDespawn();
        // Add any cleanup logic specific to satellites here
    }
}