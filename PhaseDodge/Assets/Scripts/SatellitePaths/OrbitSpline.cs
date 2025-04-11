using System.Collections.Generic;
using UnityEngine;

public class OrbitSpline : MonoBehaviour
{
    [SerializeField] private List<Transform> anchors;
    [SerializeField] private List<Transform> handles;

    public Vector3 GetPoints(float time)
    {
        if (anchors == null || handles == null || anchors.Count < 2 || handles.Count < 2)
        {
            Debug.LogError("OrbitSpline: Insufficient anchors or handles. Ensure at least two anchors and two handles are assigned.");
            return Vector3.zero; // Return a default value to prevent further errors
        }

        int numSections = anchors.Count - 1;
        int currentSection = Mathf.FloorToInt(time * numSections);
        float sectionTime = (time * numSections) - currentSection;

        // Clamp currentSection to prevent out-of-bounds access
        currentSection = Mathf.Clamp(currentSection, 0, numSections - 1);

        Vector3 p0 = anchors[currentSection].position;
        Vector3 p1 = handles[currentSection].position;
        Vector3 p2 = handles[currentSection + 1].position;
        Vector3 p3 = anchors[currentSection + 1].position;

        return CubicLerp(p0, p1, p2, p3, sectionTime);
    }

    private Vector3 CubicLerp(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float time)
    {
        Vector3 p0p1_p1p2 = Vector3.Lerp(Vector3.Lerp(p0, p1, time), Vector3.Lerp(p1, p2, time), time);
        Vector3 p1p2_p2p3 = Vector3.Lerp(Vector3.Lerp(p1, p2, time), Vector3.Lerp(p2, p3, time), time);
        return Vector3.Lerp(p0p1_p1p2, p1p2_p2p3, time);
    }

    private void OnDrawGizmos()
    {
        if (anchors == null || handles == null || anchors.Count < 2 || handles.Count < 2)
        {
            Debug.LogWarning("OrbitSpline: Insufficient anchors or handles for drawing gizmos. Ensure at least two anchors and two handles are assigned.");
            return;
        }

        Gizmos.color = Color.cyan;

        const int resolution = 50; // Number of segments to draw
        Vector3 previousPoint = anchors[0].position;

        for (int i = 1; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 currentPoint = GetPoints(t);
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
    }
}
