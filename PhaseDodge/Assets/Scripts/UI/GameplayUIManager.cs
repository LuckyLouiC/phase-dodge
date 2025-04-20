using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameplayUIManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button[] quitButtons;

    [Header("Menus")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Fuel")]
    [SerializeField] private Slider fuelGauge;

    void Start()
    {
        AddButtonListeners();
    }

    private void AddButtonListeners()
    {
        pauseButton.onClick.AddListener(PauseGame);
        resumeButton.onClick.AddListener(ResumeGame);
        retryButton.onClick.AddListener(RetryGame);
        foreach (Button button in quitButtons)
        {
            button.onClick.AddListener(ReturnToMainMenu);
        }
        pauseMenu.SetActive(false);
    }

    private void RetryGame()
    {
        SceneManager.LoadScene("BuildTestScene");
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

    public void UpdateScore(string formattedTime)
    {
        scoreText.text = "Score: " + formattedTime;
    }

    public void ShowGameOverMenu()
    {
        gameOverMenu.SetActive(true);
    }

    public void UpdateFuelGauge(float currentFuel, float maxFuel)
    {
        if (fuelGauge != null)
        {
            fuelGauge.value = currentFuel / maxFuel;
        }
    }
}
