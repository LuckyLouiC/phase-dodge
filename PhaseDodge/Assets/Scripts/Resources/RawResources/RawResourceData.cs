using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RawResourceData", menuName = "Scriptable Objects/RawResourceData")]
public class RawResourceData : ScriptableObject
{
    // The unique ID of the resource
    public int ID;

    // The name of the resource
    public string Name;

    // The rarity of the resource;
    public RarityLevel Rarity;

    // Types of asteroids this resource can be mined from (for Raw Resources)
    public List<AsteroidClass> AsteroidTypes;

    // The yield when mining this resource (for Raw Resources)
    public float MineYield;

    // Refined resources this raw resource can become (for Raw Resources)
    public List<RefineResourceType> RefinedResources;

    // A description of the resource for display to the player
    [TextArea]
    public string Decsription;
}
