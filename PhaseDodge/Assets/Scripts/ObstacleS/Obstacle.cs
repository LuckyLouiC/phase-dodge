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
    }

    protected virtual void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        if (hasEnteredScreen && IsFullyOffScreen())
        {
            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }

    protected bool IsFullyOffScreen()
    {
        Bounds bounds = obstacleCollider2D.bounds;
        Vector3 minScreenBounds = mainCamera.WorldToScreenPoint(bounds.min);
        Vector3 maxScreenBounds = mainCamera.WorldToScreenPoint(bounds.max);

        return maxScreenBounds.y < 0 || minScreenBounds.y > Screen.height || maxScreenBounds.x < 0 || minScreenBounds.x > Screen.width;
    }

    protected void OnBecameVisible()
    {
        hasEnteredScreen = true;
    }
}
