using UnityEngine;
using System.Collections;

public class PhaseJumpHandler : MonoBehaviour
{
    private bool isJumping = false;
    [SerializeField] private System.Action onPhaseJumpEnd;

    [SerializeField] private ResourceMiner miner;

    private void Start()
    {
        miner = FindAnyObjectByType<ResourceMiner>();
    }

    public void ExecutePhaseJump(Vector3 initialLocation, Vector3 targetLocation, float duration)
    {
        Debug.Log($"Executing phase jump from {initialLocation} to {targetLocation} over {duration} seconds. isJumping: {isJumping}");
        if (!isJumping)
        {
            StartCoroutine(HandlePhaseJump(initialLocation, targetLocation, duration));
        }
    }

    public void RegisterPhaseJumpEndCallback(System.Action endCallback)
    {
        onPhaseJumpEnd = endCallback;
    }

    private IEnumerator HandlePhaseJump(Vector3 initialLocation, Vector3 targetLocation, float duration)
    {
        isJumping = true;
        miner.BroadcastMessage("StartMining");
        // Removed notification to PlayerController to disable movement
        // PlayerController playerController = Object.FindAnyObjectByType<PlayerController>();
        // if (playerController != null)
        // {
        //     playerController.SendMessage("OnPhaseJumpStart");
        // }

        // Slow down time
        float originalTimeScale = Time.timeScale;
        Time.timeScale = Mathf.Clamp(0.2f, 0.0f, 100.0f); // Ensure within valid range

        // Notify listeners that the phase jump has started
        Debug.Log($"Phase jump started from {initialLocation} to {targetLocation}");

        // Start alternating positions and blinking effect
        // yield return StartCoroutine(AlternatePositionAndBlink(initialLocation, targetLocation, duration));

        // Start phase jump effect
        yield return StartCoroutine(PhaseJumpEffect(duration));

        // Gradually restore time scale
        while (Time.timeScale < originalTimeScale)
        {
            Time.timeScale = Mathf.Clamp(Time.timeScale + (Time.unscaledDeltaTime / duration), 0.0f, originalTimeScale);
            yield return null;
        }
        Time.timeScale = originalTimeScale;

        // Notify listeners that the phase jump has ended
        Debug.Log("Phase jump ended");
        onPhaseJumpEnd?.Invoke();

        // Removed notification to PlayerController to re-enable movement
        // if (playerController != null)
        // {
        //     playerController.SendMessage("OnPhaseJumpEnd");
        // }

        isJumping = false;
        miner.BroadcastMessage("StopMining");
    }

    private IEnumerator PhaseJumpEffect(float duration)
    { 
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) yield break;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Calculate the alpha value based on the elapsed time
            float alpha = Mathf.PingPong(elapsedTime * 2f, 1f);
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
            // Wait for the next frame
            yield return null;
            elapsedTime += Time.unscaledDeltaTime;
        }
        // Ensure the sprite is fully visible at the end
        Color finalColor = spriteRenderer.color;
        finalColor.a = 1f;
        spriteRenderer.color = finalColor;
    }

    private IEnumerator AlternatePositionAndBlink(Vector3 initialLocation, Vector3 targetLocation, float duration)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) yield break;

        // Create a temporary duplicate sprite at the target location
        GameObject tempSprite = new GameObject("TempSprite");
        SpriteRenderer tempSpriteRenderer = tempSprite.AddComponent<SpriteRenderer>();
        tempSpriteRenderer.sprite = spriteRenderer.sprite;
        tempSpriteRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        tempSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder;

        tempSprite.transform.SetPositionAndRotation(targetLocation, transform.rotation);

        float elapsedTime = 0f;
        float blinkInterval = 0.01f;

        while (elapsedTime < duration)
        {
            // Alternate visibility between the original and temporary sprites
            spriteRenderer.enabled = !spriteRenderer.enabled;
            tempSpriteRenderer.enabled = !spriteRenderer.enabled;

            // Wait for the current blink interval
            yield return new WaitForSecondsRealtime(blinkInterval);

            // Gradually decrease the blink interval to increase frequency
            blinkInterval = Mathf.Max(0.05f, blinkInterval - (Time.unscaledDeltaTime / duration));
            elapsedTime += blinkInterval;
        }

        // Ensure visibility is restored at the end
        transform.position = targetLocation;
        spriteRenderer.enabled = true;

        // Clean up the temporary sprite
        Destroy(tempSprite);
    }


}
