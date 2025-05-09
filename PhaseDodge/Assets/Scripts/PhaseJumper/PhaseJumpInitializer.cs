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
    [SerializeField] private GameplayUIManager uiManager;

    private ResourceMiner miner;

    private void Start()
    {
        if (uiManager == null)
        {
            uiManager = Object.FindAnyObjectByType<GameplayUIManager>();
        }
        UpdateFuelUI();
        if (miner == null)
        {
            miner = FindAnyObjectByType<ResourceMiner>();
        }
    }

    private void UpdateFuelUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateFuelGauge(currentFuel, maxFuel);
        }
    }

    public void RegisterPhaseJumpCallbacks(System.Action startCallback, System.Action endCallback)
    {
        onPhaseJumpStart = startCallback;
        onPhaseJumpEnd = endCallback;
    }

    public void TryPhaseJump(Asteroid asteroid)
    {
        if (currentFuel > 0 && !isJumping)
        {
            Vector3 initialLocation = transform.position;
            Vector3 obstacleCenter = asteroid.transform.position;

            Debug.Log($"PhaseJumpInitializer: TryPhaseJump asteroid: {asteroid}");

            // Pass data to the PhaseJumpHandler
            miner.mineTime = asteroid.mineTime;
            jumpDuration = asteroid.mineTime;
            phaseJumpHandler.ExecutePhaseJump(initialLocation, obstacleCenter, jumpDuration);
            currentFuel = Mathf.Max(0, currentFuel - 10f); // Deduct fuel (example value)
            UpdateFuelUI();
        }
    }
}
