using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement")]
    public Vector3 startPosition;
    public Vector3 endPosition;
    public float moveSpeed = 5f;
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Platform Type")]
    public PlatformType platformType = PlatformType.BackAndForth;

    private float journeyLength;
    private float startTime;
  

    public enum PlatformType
    {
        BackAndForth,      // move between two points
        Continuous,        // loop continuously
        OnDemand           // only moves when player is on it
    }

    void Start()
    {
        startPosition = transform.position;
        journeyLength = Vector3.Distance(startPosition, endPosition);
        startTime = Time.time;
    }

    void Update()
    {
        switch (platformType)
        {
            case PlatformType.BackAndForth:
                MoveBackAndForth();
                break;
            case PlatformType.Continuous:
                MoveContinuous();
                break;
            case PlatformType.OnDemand:
                MoveOnDemand();
                break;
        }
    }

    void MoveBackAndForth()
    {
        float distanceCovered = (Time.time - startTime) * moveSpeed;
        float journeyFraction = distanceCovered / journeyLength;

        // ping pong between start and end
        float pingpong = Mathf.PingPong(journeyFraction, 1f);
        Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, easeCurve.Evaluate(pingpong));
        transform.position = newPosition;
    }
    // so mad i wrote these methods and i didnt even get to use them </3
    void MoveContinuous()
    {
        float distanceCovered = (Time.time - startTime) * moveSpeed;
        float journeyFraction = Mathf.Repeat(distanceCovered / journeyLength, 1f);

        Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, easeCurve.Evaluate(journeyFraction));
        transform.position = newPosition;
    }

    void MoveOnDemand()
    {
        // check if any marble is on the platform
        Collider platformCollider = GetComponent<Collider>();
        Collider[] objectsOnPlatform = Physics.OverlapBox(
            platformCollider.bounds.center,
            platformCollider.bounds.extents * 0.9f
        );

        bool hasMarbleOnPlatform = false;
        foreach (Collider col in objectsOnPlatform)
        {
            if (col.CompareTag("Player") || col.CompareTag("Enemy"))
            {
                hasMarbleOnPlatform = true;
                break;
            }
        }

        if (hasMarbleOnPlatform)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float journeyFraction = distanceCovered / journeyLength;
            float pingpong = Mathf.PingPong(journeyFraction, 1f);

            Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, easeCurve.Evaluate(pingpong));
            transform.position = newPosition;
        }
    }

    void OnDrawGizmosSelected()
    {
        // draw path in editor
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(startPosition, endPosition);
        Gizmos.DrawWireSphere(startPosition, 0.5f);
        Gizmos.DrawWireSphere(endPosition, 0.5f);
    }
}
