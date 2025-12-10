using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio Sliders")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("Audio Value Texts")]
    public TextMeshProUGUI masterVolumeText;
    public TextMeshProUGUI musicVolumeText;
    public TextMeshProUGUI sfxVolumeText;

    [Header("Graphics Dropdowns")]
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown frameRateDropdown;

    [Header("Graphics Toggles")]
    public Toggle vSyncToggle;
    public Toggle fullscreenToggle;
    public Toggle showFPSToggle;

    [Header("Gameplay Sliders")]
    public Slider mouseSensitivitySlider;
    public Slider cameraShakeSlider;

    [Header("Gameplay Value Texts")]
    public TextMeshProUGUI mouseSensitivityText;
    public TextMeshProUGUI cameraShakeText;

    [Header("Gameplay Toggles")]
    public Toggle invertYToggle;

    [Header("Player Settings")]
    public TMP_InputField playerNameInput;


    [Header("Buttons")]
    public Button applyButton;
    public Button resetButton;
    public Button closeButton;
    public Button resetLeaderboardButton;

    public GameSettings settings;

    void Start()
    {
        settings = GameSettings.Instance;
        
        if (settings == null)
        {
            // Create GameSettings if it doesn't exist
            GameObject settingsObj = new GameObject("GameSettings");
            settings = settingsObj.AddComponent<GameSettings>();
            Debug.Log("GameSettings created automatically.");
        }
       
        SetupUI();
        LoadCurrentSettings();
        AddListeners();
    }

    void SetupUI()
    {
        // quality dropdown
        if (qualityDropdown != null)
        {
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new System.Collections.Generic.List<string> { "Low", "Medium", "High", "Ultra" });
        }

        // frame rate dropdown
        if (frameRateDropdown != null)
        {
            frameRateDropdown.ClearOptions();
            frameRateDropdown.AddOptions(new System.Collections.Generic.List<string> { "30 FPS", "60 FPS", "120 FPS", "144 FPS", "Unlimited" });
        }

        // resolution dropdown
        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();
            Resolution[] resolutions = Screen.resolutions;
            System.Collections.Generic.List<string> options = new System.Collections.Generic.List<string>();
            
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRateRatio.value.ToString("F0") + "Hz";
                options.Add(option);
            }
            
            resolutionDropdown.AddOptions(options);
        }
    }

    void LoadCurrentSettings()
    {
        // Audio
        if (masterVolumeSlider != null) masterVolumeSlider.value = settings.masterVolume;
        if (musicVolumeSlider != null) musicVolumeSlider.value = settings.musicVolume;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = settings.sfxVolume;

        // Graphics
        if (qualityDropdown != null) qualityDropdown.value = settings.qualityLevel;
        if (vSyncToggle != null) vSyncToggle.isOn = settings.vSyncEnabled;
        if (fullscreenToggle != null) fullscreenToggle.isOn = settings.fullscreen;
        if (resolutionDropdown != null) resolutionDropdown.value = settings.resolutionIndex;
        if (showFPSToggle != null) showFPSToggle.isOn = settings.showFPS;

        // Frame rate
        if (frameRateDropdown != null)
        {
            frameRateDropdown.value = settings.targetFrameRate switch
            {
                30 => 0,
                60 => 1,
                120 => 2,
                144 => 3,
                _ => 4
            };
        }

        // Gameplay
        if (mouseSensitivitySlider != null) mouseSensitivitySlider.value = settings.mouseSensitivity;
        if (cameraShakeSlider != null) cameraShakeSlider.value = settings.cameraShake;
        if (invertYToggle != null) invertYToggle.isOn = settings.invertY;
        if (playerNameInput != null) playerNameInput.text = settings.playerName;


        UpdateAllValueTexts();
    }

    void AddListeners()
    {
        // Audio sliders
        if (masterVolumeSlider != null) masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        if (musicVolumeSlider != null) musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (sfxVolumeSlider != null) sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        // Graphics
        if (qualityDropdown != null) qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        if (resolutionDropdown != null) resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        if (frameRateDropdown != null) frameRateDropdown.onValueChanged.AddListener(OnFrameRateChanged);
        if (vSyncToggle != null) vSyncToggle.onValueChanged.AddListener(OnVSyncChanged);
        if (fullscreenToggle != null) fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        if (showFPSToggle != null) showFPSToggle.onValueChanged.AddListener(OnShowFPSChanged);

        // Gameplay
        if (mouseSensitivitySlider != null) mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);
        if (cameraShakeSlider != null) cameraShakeSlider.onValueChanged.AddListener(OnCameraShakeChanged);
        if (invertYToggle != null) invertYToggle.onValueChanged.AddListener(OnInvertYChanged);
        if (playerNameInput != null) playerNameInput.onEndEdit.AddListener(OnPlayerNameChanged);

        // Buttons
        if (applyButton != null) applyButton.onClick.AddListener(OnApplyClicked);
        if (resetButton != null) resetButton.onClick.AddListener(OnResetClicked);
        if (closeButton != null) closeButton.onClick.AddListener(OnCloseClicked);
        if (resetLeaderboardButton != null) resetLeaderboardButton.onClick.AddListener(OnResetLeaderboardClicked);
    }

    // Audio stuff
    void OnMasterVolumeChanged(float value)
    {
        settings.masterVolume = value;
        if (masterVolumeText != null) masterVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    void OnMusicVolumeChanged(float value)
    {
        settings.musicVolume = value;
        if (musicVolumeText != null) musicVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    void OnSFXVolumeChanged(float value)
    {
        settings.sfxVolume = value;
        if (sfxVolumeText != null) sfxVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    // Graphics stuff
    void OnQualityChanged(int value)
    {
        settings.qualityLevel = value;
    }

    void OnResolutionChanged(int value)
    {
        settings.resolutionIndex = value;
    }

    void OnFrameRateChanged(int value)
    {
        settings.targetFrameRate = value switch
        {
            0 => 30,
            1 => 60,
            2 => 120,
            3 => 144,
            _ => -1 // UNLIMTED POWAHHHHHH
        };
    }

    void OnVSyncChanged(bool value)
    {
        settings.vSyncEnabled = value;
    }

    void OnFullscreenChanged(bool value)
    {
        settings.fullscreen = value;
    }

    void OnShowFPSChanged(bool value)
    {
        settings.showFPS = value;
    }

    // Gameplay stuff
    void OnMouseSensitivityChanged(float value)
    {
        settings.mouseSensitivity = value;
        if (mouseSensitivityText != null) mouseSensitivityText.text = value.ToString("F2");
    }

    void OnCameraShakeChanged(float value)
    {
        settings.cameraShake = value;
        if (cameraShakeText != null) cameraShakeText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    void OnInvertYChanged(bool value)
    {
        settings.invertY = value;
    }

    void OnPlayerNameChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            settings.playerName = value;
        }
        else
        {
            settings.playerName = "Player";
            if (playerNameInput != null) playerNameInput.text = "Player";
        }
    }


    // Button stuff
    void OnApplyClicked()
    {
        settings.ApplySettings();
        settings.SaveSettings();
        Debug.Log("Settings applied and saved!");
    }

    void OnResetClicked()
    {
        settings.ResetToDefaults();
        LoadCurrentSettings();
        Debug.Log("Settings reset to defaults!");
    }

    void OnCloseClicked()
    {
        gameObject.SetActive(false);
    }

    void OnResetLeaderboardClicked()
    {
        Leaderboard.Instance.ClearLeaderboard();
        Debug.Log("Leaderboard has been reset!");
    }

    void UpdateAllValueTexts()
    {
        if (masterVolumeText != null) masterVolumeText.text = Mathf.RoundToInt(settings.masterVolume * 100) + "%";
        if (musicVolumeText != null) musicVolumeText.text = Mathf.RoundToInt(settings.musicVolume * 100) + "%";
        if (sfxVolumeText != null) sfxVolumeText.text = Mathf.RoundToInt(settings.sfxVolume * 100) + "%";
        if (mouseSensitivityText != null) mouseSensitivityText.text = settings.mouseSensitivity.ToString("F2");
        if (cameraShakeText != null) cameraShakeText.text = Mathf.RoundToInt(settings.cameraShake * 100) + "%";
    }

    public void OpenMenu()
    {
        gameObject.SetActive(true);
        LoadCurrentSettings();
    }
}
