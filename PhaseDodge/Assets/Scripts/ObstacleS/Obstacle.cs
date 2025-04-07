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
    private GameObject myPrefab; // Store the original prefab

    public bool isPooled = false; // Flag to track if the object came from the pool

    protected virtual void Start()
    {
        mainCamera = Camera.main;
        obstacleCollider2D = GetComponent<PolygonCollider2D>();
        if (obstacleCollider2D == null)
        {
            Debug.LogError("Obstacle is missing a PolygonCollider2D component");
        }
        myPrefab = transform.parent != null ? transform.parent.gameObject : gameObject; // Store the original prefab
    }

    protected virtual void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (hasEnteredScreen && IsFullyOffScreen())
        {
            if (isPooled)
            {
                ObjectPool.Instance.ReturnToPool(myPrefab, gameObject); // Return to pool if it came from the pool
            }
            else
            {
                Destroy(gameObject); // Otherwise, detroy it
            }
            ObjectPool.Instance.ReturnToPool(myPrefab, gameObject); // Use myPrefab
        }
        else if (!hasEnteredScreen && IsOnScreen())
        {
            hasEnteredScreen = true;
        }
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction.normalized;
        RotateTowardsDirection();
    }

    protected bool IsFullyOffScreen()
    {
        if (obstacleCollider2D == null) return false;

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

        return maxScreenPoint.x > 0 && minScreenPoint.x < 1 && maxScreenPoint.y > 0 && minScreenPoint.y < 1;
    }

    protected void RotateTowardsDirection()
    {
        if (direction == Vector3.zero) return;

        transform.up = direction;
    }

    public virtual void OnObjectSpawn()
    {
        hasEnteredScreen = false;
    }

    public virtual void OnObjectDespawn()
    {
        // Reset any temporary state here (e.g., particle effects)
    }
}