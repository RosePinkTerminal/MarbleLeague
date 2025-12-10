using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MarbleAI : MonoBehaviour
{
    [Header("Track")]
    public TrackNode currentNode;

    [Header("Movement (tweak for your feel)")]
    public float moveForce = 30f;           // forward acceleration 
    public float maxSpeed = 12f;            // top horizontal speed
    public float turnSpeed = 8f;            // how fast it rotates toward target direction 
    [Range(0f, 1f)] public float lookAheadBlend = 0.6f;  // 0 = aim at next node only, 1 = aim at the next next node only
    public float verticalLookAhead = 0.6f;  // vertical bias
    public float verticalDamping = 0.4f;    // reduce vert influence

    [Header("Avoidance")]
    public float avoidDistance = 1.5f;      // how far ahead to check for obstacles
    public float avoidWeight = 1.4f;        // how strong avoidance steers the marble
    public LayerMask obstacleMask;          // layers considered obstacles
    public float obstacleSphereRadius = 0.25f; // radius 

    [Header("Ground / Air")]
    public float groundedCheckDistance = 0.3f; // how far below the marble to check for ground
    public LayerMask groundMask;               // ground layers

    [Header("Anti-Stuck")]
    public float stuckVelocityThreshold = 0.6f;
    public float stuckTimeThreshold = 1.2f;
    public float unstuckImpulse = 6f;

    [Header("Debug")]
    public bool drawGizmos = true;

    Rigidbody rb;
    private float stuckTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;
    }

    void FixedUpdate()
    {
        if (currentNode == null || currentNode.next == null) return;

        // look a head a lil to change pathing to be more smmooth
        Vector3 nextPos = currentNode.next.transform.position;
        Vector3 lookTarget = nextPos;
        if (currentNode.next.next != null)
        {
            Vector3 nextNextPos = currentNode.next.next.transform.position;
            lookTarget = Vector3.Lerp(nextPos, nextNextPos, lookAheadBlend);
        }

        // wall and ramp checks 
        lookTarget += Vector3.up * verticalLookAhead;

        // ground checks
        bool grounded = Physics.Raycast(transform.position, Vector3.down, groundedCheckDistance, groundMask, QueryTriggerInteraction.Ignore);

        // where we going
        Vector3 desiredDir = (lookTarget - transform.position);
        if (desiredDir.sqrMagnitude < 0.0001f)
        {
            desiredDir = transform.forward;
        }
        else
        {
            desiredDir.Normalize();
        }

        // y damping
        desiredDir.y *= verticalDamping;
        desiredDir.Normalize();

        // avoid obstacles
        Vector3 avoidance = Vector3.zero;
        RaycastHit hit;

        // only check obstacles if close to ground 
        float effectiveAvoidDistance = avoidDistance;
        if (!grounded)
        {
            // reduce avoidance distance in air
            effectiveAvoidDistance *= 0.4f;
        }

        // central spherecast forward
        if (Physics.SphereCast(transform.position, obstacleSphereRadius, transform.forward, out hit, effectiveAvoidDistance, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            Vector3 away = hit.normal;
            away.y = 0f;
            if (away.sqrMagnitude > 0.001f) avoidance += away.normalized * (avoidWeight * (effectiveAvoidDistance - hit.distance) / Mathf.Max(0.01f, effectiveAvoidDistance));
        }

        // walls check
        Vector3 leftDir = Quaternion.AngleAxis(-25f, Vector3.up) * transform.forward;
        Vector3 rightDir = Quaternion.AngleAxis(25f, Vector3.up) * transform.forward;

        if (Physics.Raycast(transform.position, leftDir, out hit, effectiveAvoidDistance * 0.8f, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            Vector3 away = hit.normal; away.y = 0f;
            if (away.sqrMagnitude > 0.001f) avoidance += away.normalized * 0.9f;
        }
        if (Physics.Raycast(transform.position, rightDir, out hit, effectiveAvoidDistance * 0.8f, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            Vector3 away = hit.normal; away.y = 0f;
            if (away.sqrMagnitude > 0.001f) avoidance += away.normalized * 0.9f;
        }

        Vector3 finalDir = (desiredDir + avoidance).normalized;

        // turn to the target
        if (finalDir.sqrMagnitude > 0.0001f)
        {
            Quaternion desiredRot = Quaternion.LookRotation(finalDir, Vector3.up);
            Quaternion newRot = Quaternion.Slerp(rb.rotation, desiredRot, Mathf.Clamp01(turnSpeed * Time.fixedDeltaTime));
            rb.MoveRotation(newRot);
        }

        // speed cap
        Vector3 v = rb.linearVelocity;
        float horizontalSpeed = new Vector2(v.x, v.z).magnitude;

        if (horizontalSpeed < maxSpeed)
        {
            Vector3 forward = transform.forward;
            forward.y = 0f;
            forward.Normalize();
            rb.AddForce(forward * moveForce, ForceMode.Acceleration);
        }

        // are we there yet squidward?
        float distSqr = (transform.position - currentNode.next.transform.position).sqrMagnitude;
        float radiusSqr = currentNode.next.radius * currentNode.next.radius;

        if (distSqr <= radiusSqr)
        {
            currentNode = currentNode.next;
            // slight damping when passing a node
            rb.linearVelocity *= 0.85f;
            stuckTimer = 0f;
        }

        // if ai stuck do a lil nudge
        Vector3 velFlat = rb.linearVelocity;
        velFlat.y = 0f;
        if (velFlat.magnitude < stuckVelocityThreshold)
        {
            stuckTimer += Time.fixedDeltaTime;
            if (stuckTimer >= stuckTimeThreshold)
            {
                
                Vector3 nudge = Quaternion.AngleAxis(Random.Range(-90f, 90f), Vector3.up) * transform.forward;
                nudge.y = 0.25f;
                rb.AddForce(nudge.normalized * unstuckImpulse, ForceMode.Impulse);
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }
    }

  
    public void AdvanceCheckpoint()
    {
        if (currentNode == null)
        {
            Debug.LogWarning("MarbleAI.AdvanceCheckpoint: currentNode is NULL on " + name, this);
            return;
        }
        if (currentNode.next == null)
        {
            Debug.LogWarning("MarbleAI.AdvanceCheckpoint: currentNode.next is NULL on " + currentNode.name, this);
            return;
        }

        currentNode = currentNode.next;
    }
   

   
    void OnDrawGizmos()
    {
        if (!drawGizmos || currentNode == null) return;

        Gizmos.color = Color.cyan;
        if (currentNode.next != null) Gizmos.DrawWireSphere(currentNode.next.transform.position, currentNode.next.radius);

        if (currentNode.next != null && currentNode.next.next != null)
        {
            Gizmos.color = Color.magenta;
            Vector3 look = Vector3.Lerp(currentNode.next.transform.position, currentNode.next.next.transform.position, lookAheadBlend);
            look += Vector3.up * verticalLookAhead;
            Gizmos.DrawWireSphere(look, 0.25f);
            Gizmos.DrawLine(transform.position, look);
        }
        else if (currentNode.next != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, currentNode.next.transform.position + Vector3.up * verticalLookAhead);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * avoidDistance);
    }
 
}
