using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 targetPosition;

    void Start()
    {
        mainCamera = Camera.main;
        targetPosition = transform.position;
    }

    void Update()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            targetPosition = mainCamera.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, transform.position.z - mainCamera.transform.position.z));
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 5f);
        ClampPositionWithinCameraBounds();
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
            Destroy(this.gameObject);
        }
    }
}
