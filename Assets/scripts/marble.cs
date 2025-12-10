using UnityEngine;
using static Abilities;


[RequireComponent(typeof(Rigidbody))]
public class marble : MonoBehaviour
{
    [Header("Camera")]
    public Camera MainCamera;
    public Camera FPSCamera;
    public bool toggle = false;
    public Transform FPSCameraTransform;
    public Vector3 moveDirection = Vector3.zero;

    [Header("Movement")]
    public float moveForce = 50f;
    public float maxSpeed = 20f;
    public float groundDrag = 0.1f;
    public float airDrag = 0.05f;

    [Header("Abilities")]
    public KeyCode AbilityKey1 = KeyCode.Q;
    public KeyCode AbilityKey2 = KeyCode.E;
    public KeyCode AbilityKey3 = KeyCode.R;



    public int Abilitiy1 = 1;

    public int Ability2 = 2;
    public int Ability3 = 3;


    

    public float abilityCooldown = 5; // seconds
    private Rigidbody rb;
    public bool isGrounded;
    private float horizontalInput;
    private float verticalInput;
    public AudioManager audioPlayer;


    [Header("Player Stats")]
    public MarbleStats marbleStats = new MarbleStats(MarbleType.Medium); 
    


    
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Make sure gravity is enabled
        rb.useGravity = true;

        // Set initial drag
        rb.linearDamping = groundDrag;

        FPSCamera.enabled = true;
        MainCamera.enabled = false;

        audioPlayer = FindFirstObjectByType<AudioManager>();

    }

    void Update()
    {
        // Get movement input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Switch cameras 'C' 
        if (Input.GetKeyDown(KeyCode.C))
        {
            toggle = !toggle;
            MainCamera.enabled = toggle;
            FPSCamera.enabled = !toggle;
        }

        // Check if grounded
        isGrounded = Physics.Raycast(FPSCameraTransform.position, Vector3.down, 0.6f);



        abilityCooldown -= 1 * Time.deltaTime;
        if(abilityCooldown < 0)
        {
            abilityCooldown = 0;
            Debug.Log("Ability Ready");
        }


        // Handle ability inputs
        if (Input.GetKeyDown(AbilityKey1))
        {
            if(abilityCooldown <= 0){
            ActivateAbility(Abilitiy1, rb, moveDirection); 
            // audioPlayer.PlayAbilitySound();
            abilityCooldown = 5;
            }
        }       


        if (Input.GetKeyDown(AbilityKey2))
        {   
            if(abilityCooldown <= 0) {
            ActivateAbility(Ability2, rb, moveDirection);
            abilityCooldown = 5;
            }
        }


        if (Input.GetKeyDown(AbilityKey3))
        {
            if(abilityCooldown <= 0) {   
            ActivateAbility(Ability3, rb, moveDirection);
            abilityCooldown = 5;
            }
        }



        // speed limiting
        LimitSpeed();
    }

    void FixedUpdate()
    {
        // apply movement force
        if (isGrounded)
        {
            rb.linearDamping = groundDrag;
            ApplyMovementForce();
        }
        else
        {
            rb.linearDamping = airDrag;
        }
    }

    void ApplyMovementForce()
    {
        // get camera forward direction
        Vector3 cameraForward = FPSCamera.enabled ? FPSCamera.transform.forward : MainCamera.transform.forward;
        Vector3 cameraRight = FPSCamera.enabled ? FPSCamera.transform.right : MainCamera.transform.right;

        // ignore the vertical 
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // calc movement direction based off the camera
        moveDirection = (cameraForward * verticalInput + cameraRight * horizontalInput).normalized;

        // use the force!!!
        if (moveDirection.magnitude > 0)
        {
            rb.AddForce(moveDirection * moveForce, ForceMode.Acceleration);
        }

        // BOINGGGGGGGGG (jump) 

        if (Input.GetKey(KeyCode.Space))
        {
            audioPlayer.PlaySound(Resources.Load<AudioClip>("Jump"));
            rb.AddForce(Vector3.up * 150f, ForceMode.Force);
        }
    }

    void LimitSpeed()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    
    // again a bunch of stuff i didnt have time to fully implement
    public float GetHealthPercent()
    {
        return marbleStats.currentHealth / marbleStats.maxHealth;
    }

    public void TakeDamage(float amount)
    {
        marbleStats.currentHealth -= amount;
        marbleStats.currentHealth = Mathf.Clamp(marbleStats.currentHealth, 0, marbleStats.maxHealth);
        Debug.Log("Marble took " + amount + " damage. Current health: " + marbleStats.currentHealth);

        if (marbleStats.currentHealth <= 0)
        {
            Debug.Log("Marble has been destroyed!");
        }
    }

    internal AbilityCooldowns GetAbilityCooldowns()
    {
        AbilityCooldowns cooldowns = new AbilityCooldowns();
        cooldowns.ability1Cooldown = abilityCooldown; 
        cooldowns.ability2Cooldown = abilityCooldown; 
        cooldowns.ability3Cooldown = abilityCooldown; 
        return cooldowns;
    }

    internal class AbilityCooldowns
    {
        
        public float ability1Cooldown;
        public float ability2Cooldown;
        public float ability3Cooldown;  


        
    }
}