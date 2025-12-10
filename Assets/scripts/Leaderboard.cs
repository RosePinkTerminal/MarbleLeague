using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public float time;
    public string date;

    public LeaderboardEntry(string name, float raceTime, string raceDate)
    {
        playerName = name;
        time = raceTime;
        date = raceDate;
    }
}

[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

public class Leaderboard : MonoBehaviour
{
    private const string LEADERBOARD_KEY = "RaceLeaderboard";
    private const int MAX_ENTRIES = 10;

    private static Leaderboard instance;
    public static Leaderboard Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("Leaderboard");
                instance = obj.AddComponent<Leaderboard>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    private LeaderboardData leaderboardData;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLeaderboard();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void AddEntry(string playerName, float time)
    {
        if (leaderboardData == null)
            leaderboardData = new LeaderboardData();

        LeaderboardEntry newEntry = new LeaderboardEntry(
            playerName, 
            time, 
            DateTime.Now.ToString("yyyy-MM-dd HH:mm")
        );

        leaderboardData.entries.Add(newEntry);

        // sort by time ascending   
        leaderboardData.entries.Sort((a, b) => a.time.CompareTo(b.time));

        // keep only top entries
        if (leaderboardData.entries.Count > MAX_ENTRIES)
        {
            leaderboardData.entries.RemoveRange(MAX_ENTRIES, leaderboardData.entries.Count - MAX_ENTRIES);
        }

        SaveLeaderboard();
    }

    public List<LeaderboardEntry> GetTopEntries(int count = 10)
    {
        if (leaderboardData == null || leaderboardData.entries == null)
            return new List<LeaderboardEntry>();

        int entriesToReturn = Mathf.Min(count, leaderboardData.entries.Count);
        return leaderboardData.entries.GetRange(0, entriesToReturn);
    }

    public bool IsTopTime(float time)
    {
        if (leaderboardData == null || leaderboardData.entries.Count == 0)
            return true;

        if (leaderboardData.entries.Count < MAX_ENTRIES)
            return true;

        return time < leaderboardData.entries[leaderboardData.entries.Count - 1].time;
    }

    public int GetRank(float time)
    {
        if (leaderboardData == null || leaderboardData.entries.Count == 0)
            return 1;

        for (int i = 0; i < leaderboardData.entries.Count; i++)
        {
            if (time < leaderboardData.entries[i].time)
                return i + 1;
        }

        return leaderboardData.entries.Count + 1;
    }

    private void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(leaderboardData);
        PlayerPrefs.SetString(LEADERBOARD_KEY, json);
        PlayerPrefs.Save();
    }

    private void LoadLeaderboard()
    {
        if (PlayerPrefs.HasKey(LEADERBOARD_KEY))
        {
            string json = PlayerPrefs.GetString(LEADERBOARD_KEY);
            leaderboardData = JsonUtility.FromJson<LeaderboardData>(json);
        }
        else
        {
            leaderboardData = new LeaderboardData();
        }
    }

    public void ClearLeaderboard()
    {
        leaderboardData = new LeaderboardData();
        PlayerPrefs.DeleteKey(LEADERBOARD_KEY);
        PlayerPrefs.Save();
        Debug.Log("Leaderboard cleared!");
    }
}
