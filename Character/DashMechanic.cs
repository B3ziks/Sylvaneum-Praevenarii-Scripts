using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;

public class DashMechanic : MonoBehaviour
{
    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashForce = 150000f;
    [SerializeField] private float dashCooldownTime = 10f;
    [SerializeField] private LayerMask obstacleLayer; // Set this in the inspector to match your obstacle layer
    [SerializeField] private float checkRadius = 0.1f; // Radius for checking if inside an obstacle
    private int maxDashUsages =1; // Max number of dashes available
    private int currentDashUsages; // Current number of dashes available
    private Rigidbody2D rb;
    private Vector3 lastMoveDirection;
    [SerializeField] DataContainer dataContainer; // Reference to your DataContainer script
    [SerializeField] private GameObject dashEffectPrefab;
    //
    public Transform effectsUI; // Drag your EffectsUI object here in the Inspector.
    private GameObject instantiatedDashEffect; // To keep track of the instantiated dash effect
    private DashEffectUI dashEffectUI; // To control the instantiated dash effect
    private bool isCooldownRunning = false;

    private void Awake()
    {
       // currentDashUsages = maxDashUsages; // Start with full dash usages

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            UnityEngine.Debug.LogError("Rigidbody2D component is missing from the object this script is attached to.");
        }
    }
    private void Start()
    {
        // ... other start logic ...
        CreateDashEffectUI(); // Create the dash effect UI at start
        UpdateDashCounterUI();
        ApplyPersistentUpgrade();

    }
    private void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (moveHorizontal != 0 || moveVertical != 0)
        {
            lastMoveDirection = new Vector3(moveHorizontal, moveVertical, 0).normalized;
        }

        if (Input.GetKeyDown(KeyCode.Space) && currentDashUsages > 0)
        {
            /*
            if (IsInsideObstacle())
            {
                DashOutOfObstacle();
            }
            else
            {
                ApplyDashForce();
            }
            */
            ApplyDashForce();

        }
    }

    private bool IsInsideObstacle()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, checkRadius, obstacleLayer);
        return collider != null;
    }

    private void DashOutOfObstacle()
    {
        Vector3 dashDirection = lastMoveDirection == Vector3.zero ? Vector3.right : lastMoveDirection;
        transform.position += dashDirection * dashDistance; // Instant move out of the obstacle
        StartCoroutine(DashCooldown());
    }

    private void ApplyDashForce()
    {
        if (currentDashUsages > 0) // Check if we have dash usages available
        {
            Vector2 dashDirection = lastMoveDirection == Vector3.zero ? Vector2.right : new Vector2(lastMoveDirection.x, lastMoveDirection.y);
            rb.AddForce(dashDirection * dashForce, ForceMode2D.Force); // Apply force for dash
            currentDashUsages--; // Use one dash usage
            UpdateDashCounterUI(); // Update the UI
            if (!isCooldownRunning) // Start the cooldown only if it's not already running
            {
                StartCoroutine(DashCooldown());
            }

        }
    }

    private IEnumerator DashCooldown()
    {
        if (isCooldownRunning) // If the cooldown is already running, don't start another one
        {
            yield break;
        }

        isCooldownRunning = true;

        while (currentDashUsages < maxDashUsages)
        {
            yield return new WaitForSeconds(dashCooldownTime);

            if (currentDashUsages < maxDashUsages) // Check if we haven't hit the maximum yet
            {
                currentDashUsages++; // Recharge one dash usage
                UpdateDashCounterUI(); // Update the UI
            }
        }

        isCooldownRunning = false; // Cooldown finished, ready to be started again if needed
    }
    //perma upgrade
    public void ApplyPersistentUpgrade()
    {
        if (dataContainer != null)
        {
            // Assuming GetIncreasingValue returns a float representing the number of extra dashes
            float extraDashes = dataContainer.GetIncreasingValue(PlayerPersistentUpgrades.Dash);
            maxDashUsages += (int)(extraDashes * dataContainer.GetUpgradeLevel(PlayerPersistentUpgrades.Dash)); // Correctly add the extra dashes to maxDashUsages
            currentDashUsages = maxDashUsages; // Reset current usages to max
            UpdateDashCounterUI(); // Update UI with correct dash count after upgrade
        }
    }
    //effects ui
    private void CreateDashEffectUI()
    {
        if (effectsUI != null && dashEffectPrefab != null)
        {
            instantiatedDashEffect = Instantiate(dashEffectPrefab, effectsUI);
            dashEffectUI = instantiatedDashEffect.GetComponent<DashEffectUI>();
            // Set the initial dash count on the UI
            if (dashEffectUI != null)
            {
                dashEffectUI.SetDashCount(currentDashUsages);
            }
        }
    }

    private void UpdateDashCounterUI()
    {
        // Update the instantiated dash effect UI
        if (dashEffectUI != null)
        {
            dashEffectUI.SetDashCount(currentDashUsages);
        }
    }

}