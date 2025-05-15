using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceDatabase", menuName = "Scriptable Objects/ResourceDatabase")]
public class ResourceDatabase : ScriptableObject
{
    // List of all resources in the database
    public List<Resource> resources = new List<Resource>();
}

// Represents a single resource with all its attributes
[System.Serializable]
public class Resource
{
}
