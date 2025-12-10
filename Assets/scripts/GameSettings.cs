using UnityEngine;
using UnityEngine.Audio;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    [Header("Audio Settings")]
    public AudioMixer audioMixer;
    [Range(0f, 1f)] public float masterVolume = 0.8f;
    [Range(0f, 1f)] public float musicVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;

    [Header("Graphics Settings")]
    public int targetFrameRate = 60;
    public bool vSyncEnabled = true;
    public int qualityLevel = 2; // 0=Low, 1=Medium, 2=High, 3=Ultra
    public bool fullscreen = true;
    public int resolutionIndex = 0;

    [Header("Gameplay Settings")]
    public string playerName = "Player";
    public float mouseSensitivity = 1.0f;
    public bool invertY = false;
    public float cameraShake = 1.0f;
    public bool showFPS = false;


    private Resolution[] availableResolutions;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
            ApplySettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        availableResolutions = Screen.resolutions;
    }

    public void ApplySettings()
    {
        // Audio
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(masterVolume, 0.0001f)) * 20f);
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(musicVolume, 0.0001f)) * 20f);
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(sfxVolume, 0.0001f)) * 20f);
        }

        // graphics
        Application.targetFrameRate = targetFrameRate;
        QualitySettings.vSyncCount = vSyncEnabled ? 1 : 0;
        QualitySettings.SetQualityLevel(qualityLevel);

        // res and fullscreen
        if (availableResolutions != null && resolutionIndex < availableResolutions.Length)
        {
            Resolution res = availableResolutions[resolutionIndex];
            Screen.SetResolution(res.width, res.height, fullscreen);
        }
        else
        {
            Screen.fullScreen = fullscreen;
        }
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);

        PlayerPrefs.SetInt("TargetFrameRate", targetFrameRate);
        PlayerPrefs.SetInt("VSync", vSyncEnabled ? 1 : 0);
        PlayerPrefs.SetInt("QualityLevel", qualityLevel);
        PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);

        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
        PlayerPrefs.SetInt("InvertY", invertY ? 1 : 0);
        PlayerPrefs.SetFloat("CameraShake", cameraShake);
        PlayerPrefs.SetInt("ShowFPS", showFPS ? 1 : 0);



        PlayerPrefs.Save();
        Debug.Log("Settings saved!");
    }

    public void LoadSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);

        targetFrameRate = PlayerPrefs.GetInt("TargetFrameRate", 60);
        vSyncEnabled = PlayerPrefs.GetInt("VSync", 1) == 1;
        qualityLevel = PlayerPrefs.GetInt("QualityLevel", 2);
        fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);

        playerName = PlayerPrefs.GetString("PlayerName", "Player");
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        invertY = PlayerPrefs.GetInt("InvertY", 0) == 1;
        cameraShake = PlayerPrefs.GetFloat("CameraShake", 1.0f);
        showFPS = PlayerPrefs.GetInt("ShowFPS", 0) == 1;

    }

    public void ResetToDefaults()
    {
        masterVolume = 0.8f;
        musicVolume = 0.7f;
        sfxVolume = 0.8f;

        targetFrameRate = 60;
        vSyncEnabled = true;
        qualityLevel = 2;
        fullscreen = true;
        resolutionIndex = 0;

        playerName = "Player";
        mouseSensitivity = 1.0f;
        invertY = false;
        cameraShake = 1.0f;
        showFPS = false;



        ApplySettings();
        SaveSettings();
    }

    //  getters for other scripts to use
    public string GetPlayerName() => playerName;
    public float GetMouseSensitivity() => mouseSensitivity;
    public bool GetInvertY() => invertY;
    public float GetCameraShake() => cameraShake;
    public bool GetShowFPS() => showFPS;

}
