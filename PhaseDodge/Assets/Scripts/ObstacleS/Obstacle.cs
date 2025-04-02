using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class Obstacle : MonoBehaviour
{
    protected float speed = 0.5f;
    protected Camera mainCamera;
    protected Vector3 direction;
    protected bool hasEnteredScreen = false;
    protected PolygonCollider2D obstacleCollider2D;

    protected virtual void Start()
    {
        mainCamera = Camera.main;
        obstacleCollider2D = GetComponent<PolygonCollider2D>();
        if (obstacleCollider2D == null )
        {
            Debug.LogError("Obstacle is missing a PolygonCollider2D component");
        }

    }

    protected virtual void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        //RotateTowardsDirection();

        if (hasEnteredScreen && IsFullyOffScreen())
        {
            Destroy(gameObject);
        }
        else if (!hasEnteredScreen && IsOnScreen()) // Check if it has become visible
        {
            hasEnteredScreen = true;
        }
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction.normalized;
        RotateTowardsDirection(); // Set initial rotation based on direction
    }

    protected bool IsFullyOffScreen()
    {
        if (obstacleCollider2D == null) return false; // Avoid errors if collider is missing

        Bounds bounds = obstacleCollider2D.bounds;
        Vector3 minScreenBounds = mainCamera.WorldToScreenPoint(bounds.min);
        Vector3 maxScreenBounds = mainCamera.WorldToScreenPoint(bounds.max);

        return maxScreenBounds.y < 0 || minScreenBounds.y > Screen.height || maxScreenBounds.x < 0 || minScreenBounds.x > Screen.width;
    }

    protected bool IsOnScreen()
    {
        if (obstacleCollider2D == null) return false;

        Bounds bounds = obstacleCollider2D.bounds;
        Vector3 minScreenPoint = mainCamera.WorldToViewportPoint(bounds.min);
        Vector3 maxScreenPoint = mainCamera.WorldToViewportPoint(bounds.max);

        // Check if the bounds rectangle overlaps with the viewport (0,0 to 1,1)
        return maxScreenPoint.x > 0 && minScreenPoint.x < 1 && maxScreenPoint.y > 0 && minScreenPoint.y < 1;
    }

    // Using transform.up is often cleaner for 2D top-down rotations
    protected void RotateTowardsDirection()
    {
        if (direction == Vector3.zero) return; // Don't rotate if direction is zero

        // Set the local 'up' vector (Y-axis) to point in the direction of movement
        transform.up = direction;
    }
}
