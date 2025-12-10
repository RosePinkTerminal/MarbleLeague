using UnityEngine;



public class Abilities : MonoBehaviour
{
   


    public static void ActivateAbility(int abilityNumber, Rigidbody marble, Vector3 moveDirection)
    {
        switch (abilityNumber) // Replace 1 with the ability number to activate
        {
            case 1:
                // Ability 1
                Debug.Log("Dash Ability Activated");
                DashAbility(marble, moveDirection);
                break;
            case 2:
                // Ability 2
                ShrinkAbility(marble);
                Debug.Log("Shrink Ability Activated");
                break;
            case 3:
                // Ability 3
                SwapAbility(marble);
                Debug.Log("Swap Ability Activated");
                break;
            default:
                Debug.Log("No Ability Assigned");
                break;
        }
    }

    private static void SwapAbility(Rigidbody marble)
    {
        // find checkpoint manager
        CheckpointManager checkpointMgr = FindFirstObjectByType<CheckpointManager>();
        if (checkpointMgr == null)
        {
            Debug.Log("CheckpointManager not found!");
            return;
        }

        // find all AI marbles
        MarbleAI[] aiMarbles = FindObjectsByType<MarbleAI>(FindObjectsSortMode.None);
        
        if (aiMarbles.Length == 0)
        {
            Debug.Log("No AI marbles found to swap with!");
            return;
        }

        // Get player's checkpoint progress
        int playerProgress = checkpointMgr.GetMarbleProgress(marble.gameObject);

        // find the AI with highest checkpoint progress 
        MarbleAI leadingAI = null;
        int maxProgress = playerProgress; // only swap with AI ahead of player

        foreach (MarbleAI ai in aiMarbles)
        {
            int aiProgress = checkpointMgr.GetMarbleProgress(ai.gameObject);
            if (aiProgress > maxProgress)
            {
                maxProgress = aiProgress;
                leadingAI = ai;
            }
        }

        if (leadingAI == null)
        {
            Debug.Log("No AI ahead of you to swap with!");
            return;
        }

        // store positions, velocities, and checkpoints
        Vector3 playerPos = marble.position;
        Vector3 aiPos = leadingAI.transform.position;
        Vector3 playerVel = marble.linearVelocity;
        
        Rigidbody aiRb = leadingAI.GetComponent<Rigidbody>();
        Vector3 aiVel = aiRb != null ? aiRb.linearVelocity : Vector3.zero;

        Debug.Log($"Before swap - Player at {playerPos}, AI at {aiPos}");
        Debug.Log($"Before swap - Player vel {playerVel}, AI vel {aiVel}");

        // swap positions 
        marble.position = aiPos;
        if (aiRb != null)
        {
            aiRb.position = playerPos;
        }
        else
        {
            leadingAI.transform.position = playerPos;
        }

        Debug.Log($"After position swap - Player at {marble.position}, AI at {(aiRb != null ? aiRb.position : leadingAI.transform.position)}");

        // reset velocities to zero
        marble.linearVelocity = Vector3.zero;
        if (aiRb != null)
        {
            aiRb.linearVelocity = Vector3.zero;
        }

        //  swap velocities
        marble.linearVelocity = aiVel;
        if (aiRb != null)
        {
            aiRb.linearVelocity = playerVel;
        }

        Debug.Log($"After velocity swap - Player vel {marble.linearVelocity}, AI vel {aiRb.linearVelocity}");


        
        // swap the checkpoints in the checkpoint manager 
        checkpointMgr.SwapMarbleCheckpoints(marble.gameObject, leadingAI.gameObject);

        // make sure AI is now using the correct node
        TrackNode[] allNodes = FindObjectsByType<TrackNode>(FindObjectsSortMode.None);
        TrackNode closestNodeForAI = null;
        TrackNode closestNodeForPlayer = null;
        float minDistAI = float.MaxValue;
        float minDistPlayer = float.MaxValue;

        foreach (TrackNode node in allNodes)
        {
            float distToAI = Vector3.Distance(playerPos, node.transform.position);
            float distToPlayer = Vector3.Distance(aiPos, node.transform.position);
            
            if (distToAI < minDistAI)
            {
                minDistAI = distToAI;
                closestNodeForAI = node;
            }
            if (distToPlayer < minDistPlayer)
            {
                minDistPlayer = distToPlayer;
                closestNodeForPlayer = node;
            }
        }

        if (closestNodeForAI != null)
        {
            leadingAI.currentNode = closestNodeForAI;
        }
        
        if (AudioManager.Instance != null)
        {
            AudioClip teleportClip = Resources.Load<AudioClip>("teleport_sound");
            if (teleportClip != null)
            {
                AudioManager.Instance.PlaySound(teleportClip);
            }
        }
        Debug.Log("Swapped with " + leadingAI.name + " (checkpoint " + maxProgress + " vs your " + playerProgress + ")");
    }




    private static void ShrinkAbility(Rigidbody marble)
    {
        
        marble.transform.localScale = marble.transform.localScale * 0.5f;

        // coroutine to restore scale after 5 seconds
        if (marble.gameObject.TryGetComponent<MonoBehaviour>(out var mono))
        {
            mono.StartCoroutine(RestoreScaleAfterDelay(marble, 5f));
        }
       
        AudioManager.Instance.PlaySound(Resources.Load<AudioClip>("shrink"));
        Debug.Log("Shrinking!");    
    }

    private static System.Collections.IEnumerator RestoreScaleAfterDelay(Rigidbody marble, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        marble.transform.localScale = marble.transform.localScale * 2f;
        AudioManager.Instance.PlaySound(Resources.Load<AudioClip>("shrink"));
        Debug.Log("Restored scale!");
    }




    static void DashAbility(Rigidbody marble, Vector3 moveDirection)
    {
 
        AudioManager.Instance.PlaySound(Resources.Load<AudioClip>("dash"));
        marble.AddForce( marble.transform.up * 10f, ForceMode.Impulse);
        marble.AddForce( moveDirection * 650f, ForceMode.Acceleration);
     

        Debug.Log("Dashing!");
        

    }























}