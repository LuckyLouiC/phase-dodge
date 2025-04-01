using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 targetPosition;
    private GameManager gameManager;

    void Start()
    {
        mainCamera = Camera.main;
        targetPosition = transform.position;
        gameManager = FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            targetPosition = mainCamera.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, transform.position.z - mainCamera.transform.position.z));
            RotateTowards(targetPosition);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 5f);
        ClampPositionWithinCameraBounds();

        Debug.DrawLine(transform.position, transform.position + transform.up * (targetPosition - transform.position).magnitude, Color.red);
    }

    private void RotateTowards(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }

    void ClampPositionWithinCameraBounds()
    {
        Vector3 position = transform.position;
        Vector3 minScreenBounds = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, transform.position.z - mainCamera.transform.position.z));
        Vector3 maxScreenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, transform.position.z - mainCamera.transform.position.z));

        position.x = Mathf.Clamp(position.x, minScreenBounds.x, maxScreenBounds.x);
        position.y = Mathf.Clamp(position.y, minScreenBounds.y, maxScreenBounds.y);

        transform.position = position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Game Over!");
            gameManager.GameOver();
            Destroy(this.gameObject);
        }
    }
}
