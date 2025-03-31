using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private float speed = 20.0f;
    private Camera mainCamera;
    private Vector3 direction;
    private bool hasEnteredScreen = false;
    private PolygonCollider2D collider2D;

    void Start()
    {
        mainCamera = Camera.main;
        collider2D = GetComponent<PolygonCollider2D>();
    }

    void Update()
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

    private bool IsFullyOffScreen()
    {
        Bounds bounds = collider2D.bounds;
        Vector3 minScreenBounds = mainCamera.WorldToScreenPoint(bounds.min);
        Vector3 maxScreenBounds = mainCamera.WorldToScreenPoint(bounds.max);

        return maxScreenBounds.y < 0 || minScreenBounds.y > Screen.height || maxScreenBounds.x < 0 || minScreenBounds.x > Screen.width;
    }

    void OnBecameVisible()
    {
        hasEnteredScreen = true;
    }
}
