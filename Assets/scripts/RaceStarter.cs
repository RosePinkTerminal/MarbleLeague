using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;


public class RaceStarter : MonoBehaviour
{
    [Header("Starting Positions")]
    public Transform[] startingPositions; 
    
    public string startPositionTag = "StartPosition"; // spawn points
    
    [Header("Countdown")]
    public float countdownDuration = 3f;
    
    [Header("Race Music")]
    public AudioClip raceMusic;
    public AudioClip CountdownSound;

    
    private bool raceStarted = false;
    private marble player;
    private MarbleAI[] aiMarbles;

    void Start()
    {
        // Stop any currently playing music
        if (AudioManager.Instance != null && AudioManager.Instance.MusicSource != null)
        {
            AudioManager.Instance.MusicSource.Stop();
        }
        
        // wait for scene to fully load so that the sounds actually play at the right time and my project dosent explode and combust into flames
        StartCoroutine(WaitAndStart());
    }
    
    private System.Collections.IEnumerator WaitAndStart()
    {
        yield return new WaitForEndOfFrame();
        gameStart();
    }



    public void gameStart(){
        if (startingPositions == null || startingPositions.Length == 0)
        {
            GameObject[] positionObjects = GameObject.FindGameObjectsWithTag(startPositionTag);
            startingPositions = positionObjects.Select(go => go.transform).ToArray();
            
            if (startingPositions.Length == 0)
            {
                Debug.LogWarning("RaceStarter: No starting positions found. ");
            }
        }

        // find all marbles,,, all of them,,,
        player = FindFirstObjectByType<marble>();
        aiMarbles = FindObjectsByType<MarbleAI>(FindObjectsSortMode.None);

        // position marbles at start
        PositionMarbles();

     
        // freeze marbles during countdown
        FreezeMarbles(true);
        Invoke(nameof(StartRace), countdownDuration);
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(CountdownSound);
        }
    }



    void PositionMarbles()
    {
        if (startingPositions == null || startingPositions.Length == 0)
        {
            Debug.LogWarning("RaceStarter: No starting positions assigned!");
            return;
        }

        // create list of positions
        List<Transform> availablePositions = new List<Transform>(startingPositions);

        // randomize AI positions
        if (aiMarbles != null)
        {
            foreach (MarbleAI ai in aiMarbles)
            {
                if (availablePositions.Count == 0)
                {
                    Debug.LogWarning("RaceStarter: Not enough starting positions for all marbles!");
                    break;
                }

                // pick random position
                int randomIndex = Random.Range(0, availablePositions.Count);
                Transform spawnPos = availablePositions[randomIndex];
                availablePositions.RemoveAt(randomIndex);

                // set AI to position
                ai.transform.position = spawnPos.position;
                ai.transform.rotation = spawnPos.rotation;

                // set velocity
                Rigidbody rb = ai.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }

        // Position player
        if (player != null)
        {
            Transform playerSpawn;
            
            if (availablePositions.Count > 0)
            {
                // Random position for player
                int randomIndex = Random.Range(0, availablePositions.Count);
                playerSpawn = availablePositions[randomIndex];
                availablePositions.RemoveAt(randomIndex);
            }
           
            else if (startingPositions.Length > 0)
            {
                // in case my code exploderr uhmm js set to OG position
                playerSpawn = startingPositions[0];
            }
            else
            {
                return;
            }

            player.transform.position = playerSpawn.position;
            player.transform.rotation = playerSpawn.rotation;

            // reset velocity
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    void FreezeMarbles(bool freeze)
    {
        // defrost/uhm anti-defrost da playa
        if (player != null)
        {
            marble marbleScript = player.GetComponent<marble>();
            if (marbleScript != null)
            {
                marbleScript.enabled = !freeze;
            }

        }

        // defrost/uhm anti-defrost all da AI
        if (aiMarbles != null)
        {
            foreach (MarbleAI ai in aiMarbles)
            {
                if (ai != null)
                {
                    ai.enabled = !freeze;
                    
                }
            }
        }
    }

    void StartRace()
    {
        if (raceStarted)
            return;

        raceStarted = true;

        // start race timer
        CheckpointManager checkpointManager = FindFirstObjectByType<CheckpointManager>();
        if (checkpointManager != null)
        {
            checkpointManager.StartRace();
        }

        // defrost all da marbles
        FreezeMarbles(false);
        
        // race music tiem
        if (raceMusic != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(raceMusic, true);
        }
        
        Debug.Log("Race started! GO!");
    }

}
