using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Splines;

public class Satellite : Obstacle
{
    private SplineAnimate splineAnimator;
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
        Debug.Log($"Satellite: OnObjectSpawn - {gameObject.name}: hasEnteredScreen: {hasEnteredScreen}");

        // Reset the satellite's properties, apply size variation, and set the path
        transform.localScale = Vector3.one;
        transform.localScale *= sizeVariation;

        if (splineAnimator == null)
        {
            Debug.LogError("Satellite: SplineAnimator is null. Returning to spawner.");
            ReturnToSpawner();
        }
        splineAnimator.Restart(true);
        Debug.Log($"Satellite: SplineAnimator: {splineAnimator} - Restarting animation");
    }

    protected override void Update()
    {
        if (splineAnimator == null)
        {
            Debug.LogWarning($"Satellite: NULL SplineAnimator: {splineAnimator}");
        }
        Debug.LogWarning($"{gameObject.name} - hasEnteredScreen: {hasEnteredScreen}, IsFullyOffScreen: {IsFullyOffScreen()}, location: {transform.position}");
        Debug.Log($"Satellite: SplineAnimator.IsPlaying: {splineAnimator.IsPlaying}, SplineAnimator.MaxSpeed: {splineAnimator.MaxSpeed}");
        splineAnimator.Completed += OnSplineAnimationCompleted;
    }

    private void OnSplineAnimationCompleted()
    {
        Debug.Log($"Satellite: Spline animation completed for {gameObject.name}");
        ReturnToSpawner();
    }

    public override void OnObjectDespawn()
    {
        base.OnObjectDespawn();
        // Add any cleanup logic specific to satellites here
    }
}