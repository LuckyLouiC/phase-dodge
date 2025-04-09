using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Camera mainCamera;
    private GameManager gameManager;

    [Header("Player Control Properties")]
    public float acceleration = 5f; // Increased acceleration
    public float maxSpeed = 5f; // Adjust max speed
    public float rotationSpeed = 10f;
    public float decelerationDistance = 0.5f; // Distance to start decelerating
    public float stoppingSpeed = 0.1f; // Speed to consider stopped

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 smoothDampVelocity = Vector3.zero;

    void Start()
    {
        mainCamera = Camera.main;
        targetPosition = transform.position;
        gameManager = FindAnyObjectByType<GameManager>();
        targetRotation = transform.rotation;
    }

    void Update()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            (targetPosition, targetRotation) = GetTargetDirectionAndRotation();
        }

        SetDirectionAndRotation();
        ClampPositionWithinCameraBounds();
        Debug.DrawLine(transform.position, transform.position + transform.up * (targetPosition - transform.position).magnitude, Color.red);
    }

    private (Vector3, Quaternion) GetTargetDirectionAndRotation()
    {
        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        Vector3 worldTouchPosition = mainCamera.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, transform.position.z - mainCamera.transform.position.z));
        targetPosition = worldTouchPosition;

        Vector3 direction = targetPosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

        return (targetPosition, targetRotation);
    }

    private void SetDirectionAndRotation()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (distanceToTarget < stoppingSpeed) // Only move if not close enough
        {
            currentVelocity = Vector3.SmoothDamp(currentVelocity, Vector3.zero, ref smoothDampVelocity, acceleration * Time.deltaTime);
            transform.position += currentVelocity * Time.deltaTime;
        }
        else
        {
            float targetSpeed = maxSpeed;
            if (distanceToTarget < decelerationDistance)
            {
                // Decelerate smoothly
                targetSpeed = Mathf.Lerp(0, maxSpeed, distanceToTarget / decelerationDistance);
            }

            Vector3 desiredVelocity = (targetPosition - transform.position).normalized * targetSpeed;
            currentVelocity = Vector3.SmoothDamp(currentVelocity, desiredVelocity, ref smoothDampVelocity, acceleration * Time.deltaTime);
            transform.position += currentVelocity * Time.deltaTime;
        }
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