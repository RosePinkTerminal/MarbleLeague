using UnityEngine;

public class TrackNode : MonoBehaviour
{
    public TrackNode next;  
    public float radius = 3f; 

    public int checkpointIndex;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
