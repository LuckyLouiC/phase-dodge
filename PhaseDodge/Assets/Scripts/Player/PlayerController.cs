using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Camera mainCamera;
    private GameManager gameManager;

    public float acceleration = 0.3f;
    public float maxSpeed = 1.0f;
    public float rotationSpeed = 10f; // Adjust for turning speed
    private Vector3 targetPosition;
    private Quaternion targetRotation; // Store the target rotation
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 smoothDampVelocity = Vector3.zero;

    void Start()
    {
        mainCamera = Camera.main;
        targetPosition = transform.position;
        gameManager = FindAnyObjectByType<GameManager>();
        targetRotation = transform.rotation; // Initialize target rotation
    }

    void Update()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            Vector3 worldTouchPosition = mainCamera.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, transform.position.z - mainCamera.transform.position.z));
            targetPosition = worldTouchPosition;

            // Calculate and store the target rotation
            Vector3 direction = targetPosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        }

        // Rotate towards the stored target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        float slowDownDistance = 0.5f;

        if (distanceToTarget < slowDownDistance)
        {
            float deceleration = 2.0f - Mathf.Clamp01(distanceToTarget / slowDownDistance);
            float targetSpeed = maxSpeed * deceleration;
            Vector3 desiredVelocity = (targetPosition - transform.position).normalized * targetSpeed;
            currentVelocity = Vector3.SmoothDamp(currentVelocity, desiredVelocity, ref smoothDampVelocity, acceleration);
        }
        else
        {
            Vector3 desiredVelocity = (targetPosition - transform.position).normalized * maxSpeed;
            currentVelocity = Vector3.SmoothDamp(currentVelocity, desiredVelocity, ref smoothDampVelocity, acceleration);
        }

        transform.position += currentVelocity * Time.deltaTime;

        ClampPositionWithinCameraBounds();
        Debug.DrawLine(transform.position, transform.position + transform.up * (targetPosition - transform.position).magnitude, Color.red);
    }

    private void ClampPositionWithinCameraBounds()
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