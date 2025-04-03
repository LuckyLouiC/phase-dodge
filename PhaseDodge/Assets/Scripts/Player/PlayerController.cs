using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Camera mainCamera;
    private GameManager gameManager;

    private Vector3 targetPosition;
    private Vector3? bufferedTargetPosition = null;

    public float acceleration = 0.3f; // Adjusted for SmoothDamp
    public float maxSpeed = 1.0f;
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 smoothDampVelocity = Vector3.zero;

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
            Vector3 worldTouchPosition = mainCamera.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, transform.position.z - mainCamera.transform.position.z));
            targetPosition = worldTouchPosition; // Store the touch position
            RotateTowards(targetPosition); // Rotate towards the stored target
        }

        // Calculate the desired velocity towards the targetPosition
        Vector3 desiredVelocity = (targetPosition - transform.position).normalized * maxSpeed;

        // Use SmoothDamp for acceleration
        currentVelocity = Vector3.SmoothDamp(currentVelocity, desiredVelocity, ref smoothDampVelocity, acceleration);
        transform.position += currentVelocity * Time.deltaTime;

        ClampPositionWithinCameraBounds();
        Debug.DrawLine(transform.position, transform.position + transform.up * (targetPosition - transform.position).magnitude, Color.red);
    }

    private void RotateTowards(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10.0f);
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
