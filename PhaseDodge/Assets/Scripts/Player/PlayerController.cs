using System;
using System.Collections;
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

    private bool isUnderExternalControl = false; // Flag to indicate external control

    public Vector2 autoTargetPosition; // Target position for automatic movement

    private InputAction moveShip;

    [SerializeField] private PhaseJumpInitializer phaseJumper;

    private bool canMove = true; // Flag to control movement

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        phaseJumper = FindAnyObjectByType<PhaseJumpInitializer>();
        targetPosition = transform.position;
        targetRotation = transform.rotation;
        mainCamera = Camera.main;

        SetupInputActions();
/*        phaseJumper.RegisterPhaseJumpCallbacks(OnPhaseJumpStart, OnPhaseJumpEnd);
        phaseJumper.phaseJumpHandler.RegisterPhaseJumpEndCallback(OnPhaseJumpEnd);*/
    }

    private void SetupInputActions()
    {
        // Update InputAction to support both touchscreen and mouse input
        moveShip = new InputAction("MoveShip");
        moveShip.AddBinding("<Touchscreen>/primaryTouch/position");
        moveShip.AddBinding("<Mouse>/position");
        moveShip.Enable();
    }

    public void SetExternalControl(Vector2 autoPosition)
    {
        isUnderExternalControl = true;
        targetPosition = autoPosition;
        canMove = false; // Disable player input
        //Debug.Log($"PlayerController: SetExternalControl - targetPosition: {targetPosition}");
    }

    public void ReleaseExternalControl()
    {
        isUnderExternalControl = false;
        canMove = true; // Re-enable player input
    }

    void Update()
    {
        if (Time.deltaTime == 0)
            return; // Skip update if time delta is zero
        if (isUnderExternalControl)
        {
            // Handle automatic movement under external control
            (targetPosition, targetRotation) = GetTargetDirectionAndRotation(targetPosition);
            //Debug.Log($"PlayerController: Update - isUnderExternalControl: {isUnderExternalControl}, targetPosition: {targetPosition}");
        }
        else if (canMove)
        {
            // Handle player input
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                (targetPosition, targetRotation) = GetTargetDirectionAndRotation(Touchscreen.current.primaryTouch.position.ReadValue());
            }
            else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            {
                (targetPosition, targetRotation) = GetTargetDirectionAndRotation(Mouse.current.position.ReadValue());
            }
        }

        SetDirectionAndRotation();
        ClampPositionWithinCameraBounds();
        Debug.DrawLine(transform.position, transform.position + transform.up * (targetPosition - transform.position).magnitude, Color.red);
    }

    // Update method to accept input position from both touch and mouse
    private (Vector3, Quaternion) GetTargetDirectionAndRotation(Vector3 inputPosition)
    {
        if (isUnderExternalControl)
        {
            targetPosition = inputPosition;
        }
        else
        {
            Vector3 worldInputPosition = mainCamera.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, transform.position.z - mainCamera.transform.position.z));
            targetPosition = worldInputPosition;
        }

        Vector3 direction = targetPosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

        return (targetPosition, targetRotation);
    }

    private void SetDirectionAndRotation()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (distanceToTarget < stoppingSpeed) // Only move if not close enough to target location
        {
            MovePlayer(Vector3.zero);
        }
        else
        {
            float targetSpeed = maxSpeed;
            if (distanceToTarget < decelerationDistance)
            {
                // Decelerate smoothly
                targetSpeed = Mathf.Lerp(0, maxSpeed, distanceToTarget / decelerationDistance);
            }
            Vector3 slowdownVelocity = (targetPosition - transform.position).normalized * targetSpeed;
            MovePlayer(slowdownVelocity);
        }
    }

    private void MovePlayer(Vector3 targetVelocity)
    {
        currentVelocity = Vector3.SmoothDamp(currentVelocity, targetVelocity, ref smoothDampVelocity, acceleration * Time.unscaledDeltaTime);
        transform.position += currentVelocity * Time.unscaledDeltaTime;
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
            Asteroid collidedObstacle = other.gameObject.GetComponent<Asteroid>();
            Debug.Log($"PlayerController: collidedObstacle: {collidedObstacle}");
            if (collidedObstacle == null)
            {
                Debug.LogWarning("Collided with object tagged 'Obstacle' but no Asteroid script attached");
                return;
            }
            if (phaseJumper == null)
            {
                Debug.LogWarning("PhaseJumper is null in PlayerController");
                return;
            }
            phaseJumper.TryPhaseJump(collidedObstacle);
        }
    }

    public void OnPhaseJumpStart()
    {
        // Disable player input during phase jump
        canMove = false;
    }

    public void OnPhaseJumpEnd()
    {
        // Re-enable player input after phase jump
        canMove = true;
    }

/*    private void OnPhaseJumpStart()
    {
        // Removed logic to disable movement
    }*/

/*    private void OnPhaseJumpEnd()
    {
        targetPosition = transform.position; // Clear the current target position
        StartCoroutine(WaitForNextInputToEnableMovement());
    }

    private IEnumerator WaitForNextInputToEnableMovement()
    {
        // Wait for the next screen tap
        while (!Touchscreen.current.primaryTouch.press.isPressed && !Mouse.current.leftButton.isPressed)
        {
            yield return null;
        }
        canMove = true; // Re-enable movement
    }*/
}