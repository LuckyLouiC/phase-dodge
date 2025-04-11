using System.Collections.Generic;
using UnityEngine;

public class SplineFollower : MonoBehaviour
{
    [SerializeField] public List<OrbitSpline> splines; // Renamed to plural for clarity
    [SerializeField] private float speed = 0.25f; // Speed of movement along the spline

    private int currentSplineIndex = 0; // Track the current spline index
    private float time;

    private void Update()
    {
        if (splines.Count <= 0) return;

        OrbitSpline currentSpline = splines[currentSplineIndex];

        time += speed * Time.deltaTime;

        if (time >= 1f) // Check if the current spline is completed
        {
            time = 0f; // Reset time for the next spline
            currentSplineIndex = (currentSplineIndex + 1) % splines.Count; // Move to the next spline, loop back if at the end
            currentSpline = splines[currentSplineIndex]; // Update the current spline
        }

        // Update the position of the follower along the current spline
        transform.position = currentSpline.GetPoints(time);
    }
}
