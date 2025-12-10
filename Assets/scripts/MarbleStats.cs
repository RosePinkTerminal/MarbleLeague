using UnityEngine;

public enum MarbleType
{
    Light,
    Medium,
    Heavy
}

[System.Serializable]
public class MarbleStats
{
    [Header("Marble Type")]
    public MarbleType type = MarbleType.Medium;

    [Header("Base Stats")]
    public float speed = 1f;
    public float control = 1f;
    public float durability = 1f;

    [Header("Calculated Values")]
    public float mass;
    public float drag;
    public float maxHealth;
    public float currentHealth;
    public MarbleStats()
    {
        SetTypeDefaults(MarbleType.Medium);
    }

    public MarbleStats(MarbleType marbleType)
    {
        SetTypeDefaults(marbleType);
    }
    // didnt have time to implement different marbles but have it :(
    public void SetTypeDefaults(MarbleType marbleType)
    {
        type = marbleType;

        switch (marbleType)
        {
            case MarbleType.Light:
                // Light: Fast, low control, fragile
                speed = 1.3f;
                control = 0.7f;
                durability = 0.6f;
                mass = 0.8f;
                drag = 0.05f;
                maxHealth = 50f;
                currentHealth = maxHealth;
                break;

            case MarbleType.Medium:
                // Medium: Balanced
                speed = 1f;
                control = 1f;
                durability = 1f;
                mass = 1f;
                drag = 0.1f;
                maxHealth = 100f;
                currentHealth = maxHealth;  
                break;

            case MarbleType.Heavy:
                // Heavy: Slow, high control, tanky
                speed = 0.7f;
                control = 1.3f;
                durability = 1.4f;
                mass = 1.3f;
                drag = 0.15f;
                maxHealth = 150f;
                currentHealth = maxHealth;  
                break;
        }
    }

    public float GetAdjustedMaxSpeed(float baseMaxSpeed)
    {
        return baseMaxSpeed * speed;
    }

    public float GetAdjustedForce(float baseForce)
    {
        return baseForce * control;
    }

    public float GetAdjustedHealth()
    {
        return maxHealth * durability;
    }

    public string GetDescription()
    {
        switch (type)
        {
            case MarbleType.Light:
                return "Fast but fragile. High speed, low control.";
            case MarbleType.Medium:
                return "Well-balanced in all areas.";
            case MarbleType.Heavy:
                return "Slow but durable. High control and durability.";
            default:
                return "";
        }
    }
}
