using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MechaGauntletWeapon : WeaponBase
{
    [SerializeField] private float maxChargeTime = 3f;
    [SerializeField] private float minDashDistance = 5f;
    [SerializeField] private float maxDashDistance = 15f;
    [SerializeField] private int minDamage = 50;
    [SerializeField] private int maxDamage = 150;
    [SerializeField] private float dashCooldownTime = 5f;
    [SerializeField] private GameObject gauntletPrefab; // Assign in inspector

    private Vector3 gauntletVectorOfAttack;
    private bool isCharging = false;
    private bool canDash = true;
    private float chargeTime = 0f;
    private GameObject instantiatedGauntlet;

    // For UI
    public TextMeshProUGUI chargeIndicator; // Will be found by name
    public float ChargeFraction => Mathf.Clamp01(chargeTime / maxChargeTime);

    protected override void Awake()
    {
        base.Awake();
        instantiatedGauntlet = Instantiate(gauntletPrefab, transform.position, Quaternion.identity, transform);
        instantiatedGauntlet.SetActive(false);

        // Find the GauntletChargeIndicator component by name
        chargeIndicator = GameObject.Find("ChargeIndicator")?.GetComponent<TextMeshProUGUI>();
        if (chargeIndicator != null)
        {
            chargeIndicator.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("ChargeIndicator object not found in the scene.");
        }
    }

    private void Update()
    {
        UpdateVectorOfAttack();

        if (playerMove.IsStationary() && canDash)
        {
            if (!isCharging)
            {
                StartCharging();
            }
            else
            {
                ContinueCharging();
            }
        }
        else if (isCharging)
        {
            ExecuteDash();
        }

        // Rotate the gauntlet towards the mouse cursor continuously while charging
        if (isCharging)
        {
            RotateGauntletTowards(gauntletVectorOfAttack);
        }
    }

    private void StartCharging()
    {
        isCharging = true;
        chargeTime = 0f;
        instantiatedGauntlet.SetActive(true);
        if (chargeIndicator != null)
        {
            chargeIndicator.gameObject.SetActive(true);
        }
    }

    private void ContinueCharging()
    {
        chargeTime += Time.deltaTime;
        chargeTime = Mathf.Clamp(chargeTime, 0, maxChargeTime);
        UpdateChargeIndicator();

        // Since you're charging, ensure the gauntlet is rotated towards the mouse cursor each frame
        RotateGauntletTowards(gauntletVectorOfAttack);
    }

    private void RotateGauntletTowards(Vector3 direction)
    {
        if (instantiatedGauntlet.activeSelf)
        {
            Vector2 directionToMouse = direction - instantiatedGauntlet.transform.position;
            float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
            float correctionAngle = -90f; // Adjust this if your sprite's "up" is not the sprite's right side
            instantiatedGauntlet.transform.rotation = Quaternion.Euler(0, 0, angle + correctionAngle);
        }
    }

    private void ExecuteDash()
    {
        isCharging = false;
        float dashDistance = Mathf.Lerp(minDashDistance, maxDashDistance, chargeTime / maxChargeTime);
        Vector3 dashDirection = gauntletVectorOfAttack == Vector3.zero ? transform.right : gauntletVectorOfAttack;
        StartCoroutine(DashMovement(dashDirection, dashDistance));
        StartCoroutine(DashCooldown());
        chargeTime = 0; // Reset the charge time
        UpdateChargeIndicator();
        if (chargeIndicator != null)
        {
            chargeIndicator.gameObject.SetActive(false);
        }
    }

    public override void Attack()
    {

    }
    private IEnumerator DashMovement(Vector3 direction, float distance)
    {
        float startTime = Time.time;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + direction * distance;



        while (Time.time < startTime + 0.5f)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, (Time.time - startTime) / 0.5f);
            yield return null;
        }

        transform.position = endPosition;
        instantiatedGauntlet.SetActive(false); // Disable gauntlet prefab after dash
    }
    private IEnumerator DashCooldown()
    {
        canDash = false;
        yield return new WaitForSeconds(dashCooldownTime);
        canDash = true;
    }
    // Added methods from MechaGauntletCombo
    private void UpdateVectorOfAttack()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        gauntletVectorOfAttack = (mousePosition - transform.position).normalized;
    }

    private void UpdateChargeIndicator()
    {
        if (chargeIndicator != null)
        {
            chargeIndicator.gameObject.SetActive(isCharging);
            if (isCharging)
            {
                int chargeLevel = Mathf.CeilToInt(Mathf.Lerp(1, 5, chargeTime / maxChargeTime));
                chargeIndicator.text = $"Charge Level: {chargeLevel}";
            }
        }
    }

    // Expose GetMessageColor for use in GauntletCollisionHandler
    public Color GetMessageColor(ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.Fire:
                return new Color(1f, 0.8f, 0f, 1f); // Brighter orange
            case ElementType.Poison:
                return Color.green;
            case ElementType.Ice:
                return Color.cyan;
            case ElementType.Lightning:
                return Color.yellow;
            default:
                return new Color(0.8f, 0.8f, 0.8f, 1f); // Light gray for default
        }
    }
    // The rest of your methods for dashing, cooldown, and collision handling...
}