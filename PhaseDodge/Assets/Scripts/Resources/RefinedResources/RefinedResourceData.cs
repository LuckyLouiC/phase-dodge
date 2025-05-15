using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RefinedResourceData", menuName = "Scriptable Objects/RefinedResourceData")]
public class RefinedResourceData : ScriptableObject

    {
    // The unique ID of the resource
    public int ID;

    // The name of the resource
    public string Name;

    // The yield when mining this resource (for Raw Resources)
    public float RefineYield;

    // How long the refining process takes
    public float RefineTime;

    // Refined resources this raw resource can become (for Raw Resources)
    public List<RawResourceType> RequiredRawResources;

    // What the resource can be used for
    public List<Use> Uses;

    // A description of the resource for display to the player
    [TextArea]
    public string Decsription;
}
