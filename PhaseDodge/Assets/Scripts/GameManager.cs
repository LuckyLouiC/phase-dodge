using System.Threading;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameplayUIManager gameplayUIManager;
    [SerializeField]
    private ObstacleSpawner obstacleSpawner;

    private float elapsedTime;
    private int currentStage = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        elapsedTime = 0;
        SetGameStage(1);
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        string formattedTime = FormatElapsedTime(elapsedTime);
        gameplayUIManager.UpdateScore(formattedTime);

        // Update game stage based on elapsed time
        if (elapsedTime >= 15 && currentStage == 1) // Stage 2 starts at 20 seconds
        {
            SetGameStage(2);
        }
        else if (elapsedTime >= 20 && currentStage == 2) // Stage 3 starts at 40 seconds
        {
            SetGameStage(3);
        }
    }

    private string FormatElapsedTime(float time)
    {
        int seconds = Mathf.FloorToInt(time);
        int milliseconds = Mathf.FloorToInt((time * 10) % 10); // Get the first digit of milliseconds
        return string.Format("{0:0}.{1:0}", seconds, milliseconds);
    }

    public void GameOver()
    {
        gameplayUIManager.ShowGameOverMenu();
        Time.timeScale = 0;
    }

    private void SetGameStage(int stage)
    {
        currentStage = stage;
        obstacleSpawner.SetStage(stage);
    }
}