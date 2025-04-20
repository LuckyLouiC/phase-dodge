using UnityEngine;
using System.Collections;

public class PhaseJumpInitializer : MonoBehaviour
{
    [Header("Fuel Properties")]
    [SerializeField] private float maxFuel = 100f;
    [SerializeField] private float currentFuel = 100f;

    [Header("Jump Properties")]
    [SerializeField] private float jumpDuration = 5f;
    [SerializeField] private float jumpCooldown = 5f;
    [SerializeField] private float jumpDistance = 5f;
    [SerializeField] private bool isJumping = false;

    [SerializeField] private Vector3 initialLocation;
    [SerializeField] private Vector3 targetLocation;

    [SerializeField] private System.Action onPhaseJumpStart;
    [SerializeField] private System.Action onPhaseJumpEnd;

    [SerializeField] public PhaseJumpHandler phaseJumpHandler;

    public void RegisterPhaseJumpCallbacks(System.Action startCallback, System.Action endCallback)
    {
        onPhaseJumpStart = startCallback;
        onPhaseJumpEnd = endCallback;
    }

    public void TryPhaseJump(Vector2 collisionLocation)
    {
        Debug.Log($"Phase Jump Triggered, collisionLocation: {collisionLocation}");
        if (currentFuel > 0 && !isJumping)
        {
            Vector3 initialLocation = transform.position;

            // Calculate the direction from the object's center to the collision point
            Vector3 direction = (Vector3)collisionLocation - initialLocation;
            direction.Normalize(); // Normalize to get the unit vector

            // Calculate the target location based on the direction and jump distance
            Vector3 targetLocation = initialLocation + direction * jumpDistance;

            // Pass data to the PhaseJumpHandler
            phaseJumpHandler.ExecutePhaseJump(initialLocation, targetLocation, jumpDuration);

            //currentFuel -= 10f; // Deduct fuel (example value)
        }
    }
}
