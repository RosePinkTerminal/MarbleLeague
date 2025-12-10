using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button PlayButton;
    public Button QuitButton;
    public Text GameTitleText;
    public Button SettingsButton; 
    public SettingsMenu settingsMenu; 
    
    void Start()
    {
        // unlock cursor in menu
        Cursor.lockState = CursorLockMode.None;

        // set up button listeners
        if (PlayButton != null)
            PlayButton.onClick.AddListener(PlayGame);
        
        if (QuitButton != null)
            QuitButton.onClick.AddListener(QuitGame);

        if (SettingsButton != null)
            SettingsButton.onClick.AddListener(OpenSettings);

        // set game title
        if (GameTitleText != null)
            GameTitleText.text = "Marble League";
    }

    private void OpenSettings()
    {
        if (settingsMenu != null)
        {
            settingsMenu.OpenMenu();
            Debug.Log("Settings menu opened.");
        }
        else
        {
            Debug.LogError("SettingsMenu not found in the scene.");
        }
    }

    void PlayGame()
    {
        // stop music if AudioManager exists
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
        }
        
        SceneManager.LoadScene("Level1");
    }

    void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
