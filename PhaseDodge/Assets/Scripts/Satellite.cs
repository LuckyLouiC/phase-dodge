using UnityEngine;

public class Satellite : Obstacle
{
    protected override void Start()
    {
        base.Start();
        speed = 1.0f;
    }
}
