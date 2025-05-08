using UnityEngine;

public class ResourceMiner : MonoBehaviour
{
    // Function of this ship component is to mine resources from asteroids
    // and send them to the ship's inventory (Cargo Bay).

    // variables:
    // miningRate: how fast the ship mines resources
    // yieldPerSecond: how much resources are mined per second
    // resourceType: type of resource that can be mined
    // maximumCapacities: maximum amount of resources that can be stored in the ship's inventory

    private bool isMining = false;
    //private float miningRate = 1.0f;

    // Methods:
    // Start: initializes the mining rate and resource type
    // Update: checks if the ship is mining and updates the inventory
    // Mine: mines resources from the asteroid
    // SendToInventory: sends the mined resources to the ship's inventory

    public void StartMining()
    {
        isMining = true;
        Debug.Log("Mining started.");
    }

    public void StopMining()
    {
        isMining = false;
        Debug.Log("Mining stopped.");
    }

    // Properties:
    // IsMining: checks if the ship is currently mining
    // GetResourceType: gets the type of resource that can be mined
    // GetMiningRate: gets the mining rate of the ship
    // GetYieldPerSecond: gets the yield per second of the ship
    // Example usage:
    // ResourceMiner miner = new ResourceMiner();
    // miner.Start();
    // miner.Update();
    // miner.Mine();
    // miner.SendToInventory();
    // This script is attached to the ship object in the game.
}
