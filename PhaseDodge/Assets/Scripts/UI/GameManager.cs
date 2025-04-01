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
        int score = Mathf.FloorToInt(elapsedTime);
        gameplayUIManager.UpdateScore(score);
    }

    public void GameOver()
    {
        gameplayUIManager.ShowGameOverMenu();
        Time.timeScale = 0;
    }
}
