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
        int seconds = Mathf.FloorToInt(time);
        int milliseconds = Mathf.FloorToInt((time * 10) % 10); // Get the first digit of milliseconds
        return string.Format("{0:0}.{1:0}", seconds, milliseconds);
    }

    public void GameOver()
    {
        gameplayUIManager.ShowGameOverMenu();
        Time.timeScale = 0;
    }
}
