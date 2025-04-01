using System.Threading;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameplayUIManager gameplayUIManager;

    private float elapsedTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        elapsedTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        string formattedTime = FormatElapsedTime(elapsedTime);
        gameplayUIManager.UpdateScore(formattedTime);
    }

    private string FormatElapsedTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        int milliseconds = Mathf.FloorToInt((time * 1000F) % 1000F);
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    public void GameOver()
    {
        gameplayUIManager.ShowGameOverMenu();
        Time.timeScale = 0;
    }
}
