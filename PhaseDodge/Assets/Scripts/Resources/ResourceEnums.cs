using UnityEngine;

[CreateAssetMenu(fileName = "ResourceEnums", menuName = "Scriptable Objects/ResourceEnums")]
public class ResourceEnums : ScriptableObject
{
    
}

public enum Use
{
    Ship_Fuel,
    Craft_Components,
    Upgrade_Materials,
}

//*** Raw Resource Enums ***//

// Enum to define the rarity level of a resource
public enum RarityLevel
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

// Enum to define the class of asteroids
public enum AsteroidClass
{
    C_Class,
    S_Class,
    M_Class
}

public enum RefineResourceType
{
    None,
    Fuel,
    Oxygen,
    Hydrogen,
    Processed_Organics,
    Refined_Silicates,
    Carbon_Nanotubes,
    Metalic_Alloys,
    Catalytic_Agents,
    Crystalised_Energy
}

//*** Refined Resource Enums ***//

public enum RawResourceType
{
    None,
    Clay_Rock,
    Silicate_Rock,
    Organic_Carbon,
    Water_Ice,
    Iron_Silicate,
    Magnesium_Silicate,
    Nickel_Iron
}