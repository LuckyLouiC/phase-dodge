using UnityEngine;

[CreateAssetMenu(fileName = "SatellitePath", menuName = "Scriptable Objects/SatellitePath", order = 1)]
public class SatellitePath : ScriptableObject
{
    public Vector3[] waypoints;
}
