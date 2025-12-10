using UnityEngine;

public class EnvironmentalHazard : MonoBehaviour
{
    [Header("Hazard Type")]
    [SerializeField] private HazardType hazardType = HazardType.Pit;
    
    [Header("Damage Settings")]
    [SerializeField] private float damageAmount = 50f;
    [SerializeField] private float damageInterval = 0.5f; // For damage methods 
    
    private float lastDamageTime = -1f;

    public enum HazardType
    {
        Pit,           // instant respawn
        Lava,          // tick damage
        Spikes,        // one tiem damage
        Slowzone,      // reduce speed
        IceSlick       // reduce control
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            HandleHazardCollision(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (hazardType == HazardType.Lava && (other.CompareTag("Player") || other.CompareTag("Enemy")))
        {
            // tick damage for lava
            if (Time.time - lastDamageTime >= damageInterval)
            {
                HandleHazardCollision(other.gameObject);
                lastDamageTime = Time.time;
            }
        }
    }

    void HandleHazardCollision(GameObject marble)
    {
        marble marbleComponent = marble.GetComponent<marble>();
        CheckpointManager checkpointManager = FindAnyObjectByType<CheckpointManager>();
        Rigidbody rb = marble.GetComponent<Rigidbody>();

        switch (hazardType)
        {
            case HazardType.Pit:
                if (marbleComponent != null && checkpointManager != null)
                {
                    if(marbleComponent.tag == "Player" && AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlaySound(Resources.Load<AudioClip>("death"));
                    }
                    checkpointManager.RespawnMarble(marble);
                }
                break;

            case HazardType.Lava:
            case HazardType.Spikes:
                
                if (marbleComponent != null)
                {
                    marbleComponent.TakeDamage(damageAmount);
                }
                break;

            case HazardType.Slowzone:
                // reduce velocity
                if (rb != null)
                {
                    rb.linearVelocity *= 0.5f;
                }
                break;

            case HazardType.IceSlick:
                // increase drag to slow
                if (rb != null)
                {
                    rb.linearDamping *= 2f;
                }
                break;
        }
    }

    void OnDrawGizmosSelected()
    {
        // hazard in editor
        Gizmos.color = GetHazardColor();
        if (GetComponent<Collider>() != null)
        {
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
    }

    Color GetHazardColor()
    {
        switch (hazardType)
        {
            case HazardType.Pit: return Color.black;
            case HazardType.Lava: return Color.red;
            case HazardType.Spikes: return new Color(1, 0.5f, 0);
            case HazardType.Slowzone: return Color.cyan;
            case HazardType.IceSlick: return Color.white;
            default: return Color.gray;
        }
    }
}
