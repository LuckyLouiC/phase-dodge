using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button[] quitButtons;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;

    [SerializeField] private TextMeshProUGUI scoreText;

    void Start()
    {
        pauseButton.onClick.AddListener(PauseGame);
        retryButton.onClick.AddListener(RetryGame);
        resumeButton.onClick.AddListener(ResumeGame);
        foreach (Button quitButton in quitButtons)
        {
                quitButton.onClick.AddListener(ReturnToMainMenu);
        }
        pauseMenu.SetActive(false);
    }

    private void RetryGame()
    {
        SceneManager.LoadScene("PrototypeScene");
        Time.timeScale = 1;
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void ShowGameOverMenu()
    {
        gameOverMenu.SetActive(true);
    }

    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }
}
