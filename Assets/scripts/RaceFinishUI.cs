using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class RaceFinishUI : MonoBehaviour
{
    [Header("Finish Panel")]
    public GameObject finishPanel;
    
    [Header("Winner Display")]
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI winnerTimeText;
    public TextMeshProUGUI rankText;
    
    [Header("Leaderboard Display")]
    public GameObject leaderboardContainer;
    public GameObject leaderboardEntryPrefab;
    public Transform leaderboardContent;
    
    [Header("Buttons")]
    public Button replayButton;
    public Button mainMenuButton;
    
    private bool raceFinished = false;
    private CheckpointManager checkpointManager;

    void Start()
    {
        checkpointManager = FindFirstObjectByType<CheckpointManager>();
        
        if (finishPanel != null)
            finishPanel.SetActive(false);
        
        if (replayButton != null)
            replayButton.onClick.AddListener(ReplayRace);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    void Update()
    {
        if (raceFinished || checkpointManager == null)
            return;

        // check if player finished
        marble player = FindFirstObjectByType<marble>();
        if (player != null && checkpointManager.HasFinished(player.gameObject))
        {
            OnRaceFinished(player.gameObject);
            return;
        }

        // check if AI is goated and actually finished
        MarbleAI[] aiMarbles = FindObjectsByType<MarbleAI>(FindObjectsSortMode.None);
        foreach (MarbleAI ai in aiMarbles)
        {
            if (checkpointManager.HasFinished(ai.gameObject))
            {
                OnRaceFinished(ai.gameObject);
                return;
            }
        }
    }

    void OnRaceFinished(GameObject winner)
    {
        raceFinished = true;
        
        float finalTime = checkpointManager.GetRaceTime(winner);
        bool isPlayer = winner.GetComponent<marble>() != null;
        string winnerName = isPlayer ? GameSettings.Instance.GetPlayerName() : winner.name;

        // add to leaderboard
        Leaderboard.Instance.AddEntry(winnerName, finalTime);

        // show da finish screen
        ShowFinishScreen(winnerName, finalTime, isPlayer);
        
        // unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if(winner.tag == "Player")
                {
                    AudioManager.Instance.PlaySound(Resources.Load<AudioClip>("yay"));
                    AudioManager.PlayDelayedSound(Resources.Load<AudioClip>("congrats"),1.8f);
            } else
                {
                    AudioManager.Instance.PlaySound(Resources.Load<AudioClip>("boo"));
                }

    }

    void ShowFinishScreen(string winnerName, float time, bool isPlayer)
    {
        if (finishPanel != null)
            finishPanel.SetActive(true);

        // Display winner info
        if (winnerText != null)
        {
            if (isPlayer)
                winnerText.text = "YOU WIN!";
            else
                winnerText.text = winnerName + " WINS!";
        }

        if (winnerTimeText != null)
        {
            string formattedTime = checkpointManager.FormatTime(time);
            winnerTimeText.text = "Time: " + formattedTime;
        }

        // display rank on da board
        if (rankText != null)
        {
            int rank = Leaderboard.Instance.GetRank(time);
            rank -= 1; // adjust bc index
            string rankSuffix = GetRankSuffix(rank);
            
            if (Leaderboard.Instance.IsTopTime(time))
            {
                rankText.text = "NEW RECORD! " + rank + rankSuffix + " Place";
                rankText.color = Color.yellow;
            }
            else
            {
                rankText.text = rank + rankSuffix + " Place";
                rankText.color = Color.white;
            }
        }


        UpdateLeaderboardDisplay();
    }

    void UpdateLeaderboardDisplay()
    {
        if (leaderboardContent == null)
            return;

        // Clear existing entries
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        // Get top entries
        List<LeaderboardEntry> entries = Leaderboard.Instance.GetTopEntries(10);

        // Create UI for each entry
        for (int i = 0; i < entries.Count; i++)
        {
            GameObject entryObj;
            
            if (leaderboardEntryPrefab != null)
            {
                entryObj = Instantiate(leaderboardEntryPrefab, leaderboardContent);
            }
            else
            {
                // Create simple text entry if no prefab
                entryObj = new GameObject("Entry_" + i);
                entryObj.transform.SetParent(leaderboardContent, false);
                
                // Add RectTransform and configure it
                RectTransform rectTransform = entryObj.AddComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.pivot = new Vector2(0.5f, 1);
                rectTransform.sizeDelta = new Vector2(0, 30); // Height of 30
                
                TextMeshProUGUI text = entryObj.AddComponent<TextMeshProUGUI>();
                text.fontSize = 18;
                text.color = i == 0 ? Color.yellow : Color.white;
                text.alignment = TextAlignmentOptions.Left;
                text.margin = new Vector4(10, 5, 10, 5);
            }

            // Set entry text
            TextMeshProUGUI entryText = entryObj.GetComponent<TextMeshProUGUI>();
            if (entryText != null)
            {
                string timeStr = checkpointManager.FormatTime(entries[i].time);
                entryText.text = string.Format("{0}. {1} - {2}", i + 1, entries[i].playerName, timeStr);
            }
        }
    }

    string GetRankSuffix(int rank)
    {
        if (rank == 1) return "st";
        if (rank == 2) return "nd";
        if (rank == 3) return "rd";
        return "th";
    }

    void ReplayRace()
    {
        // reload race scene
        AudioManager.Instance.FadeOutMusic(2);
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    void GoToMainMenu()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
        }
        SceneManager.LoadScene("MainMenu");
    }

    public void TriggerFinish(GameObject marble)
    {
        if (!raceFinished)
            OnRaceFinished(marble);
    }
}
