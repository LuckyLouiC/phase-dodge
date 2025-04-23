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
    }

    protected override void Start()
    {
        base.Start();
        speed = 0.45f;
    }

    public override void OnObjectSpawn()
    {
        base.OnObjectSpawn();
        ResetTransform();
        SetupSplineAnimator();
    }

    private void ResetTransform()
    {
        // Reset the satellite's transform properties
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    private void SetupSplineAnimator()
    {
        if (splineAnimator == null)
        {
            Debug.LogError("Satellite: SplineAnimator is null. Returning to spawner.");
            ReturnToSpawner();
        }
        splineAnimator.Restart(true);
        // Subscribe to the Completed event in OnObjectSpawn - unsubscribe in OnObjectDespawn
        splineAnimator.Completed += OnSplineAnimationCompleted;
    }

    private void OnSplineAnimationCompleted()
    {
        ReturnToSpawner();
    }

    public override void OnObjectDespawn()
    {
        base.OnObjectDespawn();
        // Unsubscribe from SplineAnimate's Completed event to prevent memory leaks
        if (splineAnimator != null)
        {
            splineAnimator.Completed -= OnSplineAnimationCompleted;
        }
    }
}