using UnityEngine;

public class SatellitePathVisualizer : MonoBehaviour
{
    public SatellitePath[] satellitePaths;
    public PathColor[] colors;

    public enum PathColor
    {
        Red,
        Green,
        Blue,
        Yellow,
        Cyan,
        Magenta
    }

    private void OnDrawGizmos()
    {
        if (satellitePaths == null || colors == null)
        {
            return;
        }

        for (int i = 0; i < satellitePaths.Length; i++)
        {
            if (i < colors.Length)
            {
                DrawSatellitePath(satellitePaths[i], GetColor(colors[i]));
            }
            else
            {
                Debug.LogWarning("Not enough colors specified for all satellite paths.");
                DrawSatellitePath(satellitePaths[i], Color.red); // Default to red if not enough colors
            }
        }
    }

    private void DrawSatellitePath(SatellitePath satellitePath, Color color)
    {
        // Draw paths for each satellite path
        if (satellitePath == null || satellitePath.waypoints == null || satellitePath.waypoints.Length < 2)
        {
            return;
        }

        Gizmos.color = color; // Use the specified color

        // Draw waypoints
        foreach (var waypoint in satellitePath.waypoints)
        {
            Gizmos.DrawSphere(waypoint, 0.1f);
        }

        // Draw lines between waypoints
        for (int i = 0; i < satellitePath.waypoints.Length - 1; i++)
        {
            Gizmos.DrawLine(satellitePath.waypoints[i], satellitePath.waypoints[i + 1]);
        }
    }

    private Color GetColor(PathColor pathColor)
    {
        switch (pathColor)
        {
            case PathColor.Red:
                return Color.red;
            case PathColor.Green:
                return Color.green;
            case PathColor.Blue:
                return Color.blue;
            case PathColor.Yellow:
                return Color.yellow;
            case PathColor.Cyan:
                return Color.cyan;
            case PathColor.Magenta:
                return Color.magenta;
            default:
                return Color.white;
        }
    }
}
