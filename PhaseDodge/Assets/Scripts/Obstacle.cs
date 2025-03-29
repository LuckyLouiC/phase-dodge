using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class Obstacle : MonoBehaviour
{
    public float speed = 5f;
    private Camera mainCamera;
    private Vector3 direction;
    private bool hasEnteredScreen = false;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        if (IsOffScreen())
        {
            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }

    private bool IsOffScreen()
    {
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(transform.position);
        return (screenPosition.y < 0 || screenPosition.y > Screen.height || screenPosition.x < 0 || screenPosition.x > Screen.width) && hasEnteredScreen;
    }
}
