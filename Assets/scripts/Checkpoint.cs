using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    public int checkpointIndex = 0;
    public bool isFinishLine = false;

    private CheckpointManager manager;
    private Collider col;

    void Start()
    {
        manager = FindFirstObjectByType<CheckpointManager>();

        // ensure we have a trigger collider
        col = GetComponent<Collider>();
        if (col == null)
        {
            SphereCollider sphere = gameObject.AddComponent<SphereCollider>();
            sphere.isTrigger = true;
            sphere.radius = 6f;
            col = sphere;
        }
        else
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (!other.CompareTag("Player") && !other.CompareTag("Enemy"))
            return;

        if (manager != null)
        {
            if (isFinishLine)
            {
                _ = manager.FinishRace(other.gameObject);
            }
            else
            {
                // Pass checkpoint 
                Vector3 checkpointCenter = col.bounds.center;

                manager.CheckpointReached(
                    other.gameObject,
                    checkpointIndex,
                    checkpointCenter
                );
            }
        }

        // Ai checkpoints
        MarbleAI ai = other.GetComponent<MarbleAI>();
        if (ai != null)
        {
            // only advance if the AI is currently on this checkpoint segment
            if (ai.currentNode != null &&
                ai.currentNode.checkpointIndex == checkpointIndex)
            {
                ai.AdvanceCheckpoint();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isFinishLine ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 6f);
    }
}
