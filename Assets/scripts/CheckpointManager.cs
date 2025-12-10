using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CheckpointManager : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    public float respawnHeight = -20f;

    private Dictionary<GameObject, int> marbleCheckpoints = new Dictionary<GameObject, int>();
    private Dictionary<GameObject, Vector3> spawnPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, float> marbleStartTimes = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, float> marbleFinishTimes = new Dictionary<GameObject, float>();

    private Checkpoint[] checkpoints;
    private bool raceStarted = false;
    private float raceStartTime;

    void Start()
    {
        // get checkpoints in order
        checkpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
        System.Array.Sort(checkpoints, (a, b) => a.checkpointIndex.CompareTo(b.checkpointIndex));
    }

    void Update()
    {
        // respawn player
        marble playerMarble = FindFirstObjectByType<marble>();
        if (playerMarble != null && playerMarble.transform.position.y < respawnHeight)
            RespawnMarble(playerMarble.gameObject);

        // respawn AI
        MarbleAI[] aiMarbles = FindObjectsByType<MarbleAI>(FindObjectsSortMode.None);
        foreach (var ai in aiMarbles)
            if (ai.transform.position.y < respawnHeight)
                RespawnMarble(ai.gameObject);
    }

    public void CheckpointReached(GameObject marble, int checkpointIndex, Vector3 checkpointCenter)
    {
        bool firstTime = !marbleCheckpoints.ContainsKey(marble);

        // start timer on race start
        if (firstTime && raceStarted)
        {
            marbleStartTimes[marble] = raceStartTime;
        }

        if (firstTime || checkpointIndex > marbleCheckpoints[marble])
        {
            marbleCheckpoints[marble] = checkpointIndex;

            // save center
            spawnPositions[marble] = checkpointCenter;

            Debug.Log(marble.name + " reached checkpoint " + checkpointIndex);
        }
    }

    public async Task FinishRace(GameObject marble)
    {
        // only finish if race has started and marble hasn't finished yet
        if (raceStarted && !marbleFinishTimes.ContainsKey(marble))
        {
            float finalTime = Time.time - raceStartTime;
            marbleFinishTimes[marble] = finalTime;
            Debug.Log(marble.name + " finished the race in " + FormatTime(finalTime) + "!");
            
            
            RaceFinishUI finishUI = FindFirstObjectByType<RaceFinishUI>();
            if (finishUI != null)
            {
                finishUI.TriggerFinish(marble);
                
            }
        }
    }


    public void StartRace()
    {
        raceStarted = true;
        raceStartTime = Time.time;
        Debug.Log("CheckpointManager: Race timer started!");
    }



    public void RespawnMarble(GameObject marble)
    {
        Vector3 respawnPos;

        // use saved checkpoint position
        if (!spawnPositions.TryGetValue(marble, out respawnPos))
        {
            // use checkpoint 0 if there isnt nonen
            if (checkpoints.Length > 0)
                respawnPos = GetCheckpointCenter(checkpoints[0].gameObject);
            else
                respawnPos = marble.transform.position;
        }

        // apply respawn position
        Rigidbody rb = marble.GetComponent<Rigidbody>();
        marble.transform.position = respawnPos;

        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log(marble.name + " respawned!");

        // Sync AI to correct TrackNode segment
        MarbleAI ai = marble.GetComponent<MarbleAI>();
        if (ai)
        {
            int cpIndex = 0;
            marbleCheckpoints.TryGetValue(marble, out cpIndex);
            SyncAIToCheckpointNode(ai, cpIndex);
        }
    }


    // Return the center of a checkpoint collider
    private Vector3 GetCheckpointCenter(GameObject checkpointObj)
    {
        Collider col = checkpointObj.GetComponent<Collider>();
        if (col)
            return col.bounds.center;

        return checkpointObj.transform.position;
    }

    // Sync AI to the first node that belongs to the same checkpoint segment
    private void SyncAIToCheckpointNode(MarbleAI ai, int checkpointIndex)
    {
        TrackNode[] nodes = FindObjectsByType<TrackNode>(FindObjectsSortMode.None);

        foreach (TrackNode node in nodes)
        {
            if (node.checkpointIndex == checkpointIndex)
            {
                ai.currentNode = node;
                return;
            }
        }

        Debug.LogWarning("No TrackNode found for checkpoint " + checkpointIndex);
    }


    public int GetMarbleProgress(GameObject marble)
    {
        int index;
        return marbleCheckpoints.TryGetValue(marble, out index) ? index : -1;
    }

    public bool TryGetSpawnPosition(GameObject marble, out Vector3 spawnPos)
    {
        return spawnPositions.TryGetValue(marble, out spawnPos);
    }

    public bool TryGetCheckpointIndex(GameObject marble, out int checkpointIndex)
    {
        return marbleCheckpoints.TryGetValue(marble, out checkpointIndex);
    }

    public float GetRaceTime(GameObject marble)
    {
        // return finish time on finished   
        if (marbleFinishTimes.ContainsKey(marble))
            return marbleFinishTimes[marble];

        // return current time once race starts
        if (raceStarted)
            return Time.time - raceStartTime;

        // race hasn't started yet
        return 0f;
    }

    public bool HasFinished(GameObject marble)
    {
        return marbleFinishTimes.ContainsKey(marble);
    }

    public void SwapMarbleCheckpoints(GameObject marble1, GameObject marble2)
    {
        // swap checkpoint progress
        int checkpoint1 = GetMarbleProgress(marble1);
        int checkpoint2 = GetMarbleProgress(marble2);

        marbleCheckpoints[marble1] = checkpoint2;
        marbleCheckpoints[marble2] = checkpoint1;

        // swap spawn positions
        Vector3 spawnPos1;
        Vector3 spawnPos2;
        bool hasSavedPos1 = spawnPositions.TryGetValue(marble1, out spawnPos1);
        bool hasSavedPos2 = spawnPositions.TryGetValue(marble2, out spawnPos2);

        if (hasSavedPos1 && hasSavedPos2)
        {
            spawnPositions[marble1] = spawnPos2;
            spawnPositions[marble2] = spawnPos1;
        }

        Debug.Log($"Swapped checkpoints: {marble1.name} is now at {checkpoint2}, {marble2.name} is now at {checkpoint1}");
    }

    public string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 100f) % 100f);
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }
}
