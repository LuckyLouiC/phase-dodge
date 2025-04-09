using UnityEngine;

public class AsteroidSmall : Obstacle
{
    protected override void Start()
    {
        base.Start();
        speed = 1.5f; // Faster than the regular asteroid
        
    }

    public override void OnObjectSpawn()
    {
        base.OnObjectSpawn();
        // Reset any specific properties for small asteroids here
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        transform.localScale *= 0.25f;
    }

    public override void OnObjectDespawn()
    {
        base.OnObjectDespawn();
        // Add any cleanup logic specific to small asteroids here
    }
}