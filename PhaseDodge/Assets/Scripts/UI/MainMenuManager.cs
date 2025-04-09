using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    /*[SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;*/

    void Start()
    {
        // Add listeners to the buttons
        playButton.onClick.AddListener(LoadGameScene);
        quitButton.onClick.AddListener(QuitGame);
        /*optionsButton.onClick.AddListener(LoadOptionsScene);
        creditsButton.onClick.AddListener(LoadCreditsScene);*/
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene("BuildTestScene");
    }

    /*void LoadOptionsScene()
    {
        SceneManager.LoadScene("OptionsScene");
    }

    void LoadCreditsScene()
    {
        SceneManager.LoadScene("CreditsScene");
    }*/

    void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop playing the game in the editor
#endif
    }
}
