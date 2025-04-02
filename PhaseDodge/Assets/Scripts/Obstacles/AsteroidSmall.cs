using UnityEngine;

public class AsteroidSmall : Obstacle
{
    protected override void Start()
    {
        base.Start();
        speed = 1.5f; // Faster than the regular asteroid
    }
}