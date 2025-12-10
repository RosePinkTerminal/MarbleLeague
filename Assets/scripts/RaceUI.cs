using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class RaceUI : MonoBehaviour
{
    [Header("Speed Display")]
    public TextMeshProUGUI speedText;
    public Image speedBar;

    [Header("Health Display")]
    public Text healthText;
    public Image healthBar;
    public Color healthGood = Color.green;
    public Color healthWarn = Color.yellow;
    public Color healthCritical = Color.red;

    [Header("Abilities Display")]
    public TextMeshProUGUI ability1Text;
    public TextMeshProUGUI ability2Text;
    public TextMeshProUGUI ability3Text;
    public Image ability1Icon;
    public Image ability2Icon;
    public Image ability3Icon;

    [Header("Timer")]
    public TextMeshProUGUI timerText;
    
    private marble playerMarble;
    private float maxDisplaySpeed = 100f;

    void Start()
    {
        playerMarble = FindFirstObjectByType<marble>();
        if (playerMarble == null)
        {
            Debug.LogError("Player marble not found!");
            enabled = false;
            return;
        }
        // Initialize UI elements if not assigned
        if (speedText == null)
            speedText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        if (playerMarble == null)
            return;

        UpdateSpeedDisplay();
        UpdateHealthDisplay();
        UpdateAbilitiesDisplay();
        UpdateTimerDisplay();
    }

    void UpdateSpeedDisplay()
    {
        if (speedText == null && speedBar == null)
            return;

        Rigidbody rb = playerMarble.GetComponent<Rigidbody>();
        if (rb == null)
            return;

        float currentSpeed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;

        if (speedText != null)
            speedText.text = currentSpeed.ToString("F1") + " m/s";

        if (speedBar != null)
            speedBar.fillAmount = Mathf.Clamp01(currentSpeed / maxDisplaySpeed);
    }

    void UpdateHealthDisplay()
    {
        if (healthText == null && healthBar == null)
            return;

        float healthPercent = playerMarble.GetHealthPercent();

        if (healthText != null)
            healthText.text = "Health: " + (healthPercent * 100).ToString("F0") + "%";

        if (healthBar != null)
        {
            healthBar.fillAmount = healthPercent;
            healthBar.color = healthPercent > 0.5f ? healthGood : (healthPercent > 0.25f ? healthWarn : healthCritical);
        }
    }

    void UpdateAbilitiesDisplay()
    {
        if (ability1Text != null)
            ability1Text.text = "Q: Dash\n(Ready)";

        if (ability2Text != null)
            ability2Text.text = "E: Shrink\n(Ready)";

        if (ability3Text != null)
            ability3Text.text = "R: Swap\n(Ready)";

        
        marble.AbilityCooldowns cooldowns = playerMarble.GetAbilityCooldowns();

        if (ability1Text != null)
        {
            if (cooldowns.ability1Cooldown > 0)
                ability1Text.text = "Q: Dash\n(" + cooldowns.ability1Cooldown.ToString("F1") + "s)";
        }

        if (ability2Text != null)
        {
            if (cooldowns.ability2Cooldown > 0)
                ability2Text.text = "E: Shrink\n(" + cooldowns.ability2Cooldown.ToString("F1") + "s)";
        }
        if (ability3Text != null)
        {
            if (cooldowns.ability3Cooldown > 0)
                ability3Text.text = "R: Swap\n(" + cooldowns.ability3Cooldown.ToString("F1") + "s)";
        }
        

    }

    void UpdateTimerDisplay()
    {
        if (timerText == null)
            return;

        CheckpointManager checkpointMgr = FindFirstObjectByType<CheckpointManager>();
        if (checkpointMgr == null)
            return;

        float raceTime = checkpointMgr.GetRaceTime(playerMarble.gameObject);
        bool finished = checkpointMgr.HasFinished(playerMarble.gameObject);

        if (raceTime > 0)
        {
            string timeStr = checkpointMgr.FormatTime(raceTime);
            if (finished)
                timerText.text = "Final Time: " + timeStr;
            else
                timerText.text = "Time: " + timeStr;
        }
        else
        {
            timerText.text = "Time: 00:00.00";
        }
    }
}
