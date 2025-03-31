using UnityEngine;

public class AlienShip : Obstacle
{
    protected override void Start()
    {
        base.Start();
        speed = 2.0f;
    }
}
