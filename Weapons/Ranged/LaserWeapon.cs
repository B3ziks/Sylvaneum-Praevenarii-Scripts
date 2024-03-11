using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWeapon : WeaponBase
{
    public GameObject laser; // Assign this in the inspector
    private int hitCounter = 0;
    private const int MaxHitsBeforeCooldown = 5;
    private bool isLaserActive = true;
    private bool isCooldownActive = false;
    public float laserIndicatorThickness = 0.05f;

    protected override void Awake()
    {
        base.Awake();
        if (laser == null)
        {
            Debug.LogError("Laser GameObject is not assigned!");
            return;
        }
        ActivateLaser();
    }
    private void Update()
    {
        if (isLaserActive)
        {
            UpdateLaserPosition();
            
        }
    }
    public override void Attack()
    {
        if (!isLaserActive && !isCooldownActive)
        {
            ActivateLaser();
        }
    }

    public void HandleLaserHit(Collider2D collider)
    {
        if (isLaserActive)
        {

            ApplyDamage(collider.transform.position, weaponData.stats.damage, collider.GetComponent<IDamageable>());
            hitCounter++;
            Debug.Log("Hit Counter: " + hitCounter);
            if (hitCounter >= MaxHitsBeforeCooldown)
            {
                StartCoroutine(DeactivateLaserAfterCooldown());
            }
        }
    }

    private void ActivateLaser()
    {
        isLaserActive = true;
        hitCounter = 0;
        laser.SetActive(true);
        Debug.Log("Laser Activated");
    }

    private IEnumerator DeactivateLaserAfterCooldown()
    {
        isCooldownActive = true;
        isLaserActive = false;
        laser.SetActive(false);
        Debug.Log("Laser Deactivated, starting cooldown");

        yield return new WaitForSeconds(weaponData.stats.timeToAttack);

        isCooldownActive = false;
        ActivateLaser();
        Debug.Log("Cooldown ended, laser reactivated");
    }
    private void UpdateLaserPosition()
    {
        // Get the mouse position in world space, ensuring that the z value is set to 0 since we're in a 2D space.
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        // Calculate the direction from the character to the mouse cursor.
        Vector3 directionToMouse = (mousePosition - transform.position).normalized;

        // Set the laser's rotation to point towards the mouse cursor.
        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
        laser.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Calculate the distance from the character to the mouse cursor, which determines the laser's length.
        float laserLength = Vector3.Distance(transform.position, mousePosition);

        // Set the laser's localScale to match the desired length and thickness.
        // The 'x' component of localScale will control the length since the laser's 'right' should be its lengthwise direction.
        laser.transform.localScale = new Vector3(laserLength, laserIndicatorThickness, 1f);

        // Position the laser's starting point at the character's position.
        // Since the pivot is at one end of the laser, setting its position to the character's position will make it start there.
        laser.transform.position = transform.position + directionToMouse * laserLength * 0.5f;
    }


    private void DeactivateLaser()
    {
        isLaserActive = false;
        laser.SetActive(false);
    }
}