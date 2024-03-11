using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketSledgeHammerWeapon : WeaponBase
{
    [SerializeField] private GameObject groundImpactEffect; // Prefab for the ground impact effect
    [SerializeField] private float attackRadius = 2f; // Area of effect as a circle
    [SerializeField] private float effectDistanceFromCharacter = 2.0f; // How far the effect appears from the character
    [SerializeField] private float chargeSpeed = 20f; // Speed at which the player charges forward
    [SerializeField] private float chargeDuration = 0.2f; // Duration of the charge

    private Rigidbody2D rb;
    private bool isCharging = false;

    protected override void Awake()
    {
        // Assuming this WeaponBase is a child of the character GameObject,
        // or you have some way to find your player character.
        owner = GetComponentInParent<Character>();

        // Safety check to ensure you actually found the owner
        if (owner == null)
        {
            UnityEngine.Debug.LogError("Owner not found on " + gameObject.name);
        }
        else
        {
            rb = owner.GetComponent<Rigidbody2D>();
            // as long as you ensure it's not null first.
        }
    }

    public override void Attack()
    {
        StartCoroutine(ChargeAttack());
    }

    private IEnumerator ChargeAttack()
    {
        // Calculate direction towards the mouse position to determine the charge direction
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        Vector3 directionToMouse = (mouseWorldPosition - transform.position).normalized;

        // Set the position and activate the ground impact effect
        Vector3 effectPosition = transform.position + directionToMouse * effectDistanceFromCharacter;

        if (groundImpactEffect != null)
        {
            // Set the scale of the ground impact effect based on the attack radius
            float scaleFactor = weaponData.stats.explosionRadius; // Assuming attackRadius is the original size
            groundImpactEffect.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

            groundImpactEffect.transform.position = effectPosition;
            groundImpactEffect.SetActive(true);
        }

        // Perform the charge
        Vector3 chargeDirection = directionToMouse.normalized;
        isCharging = true;
        float endTime = Time.time + chargeDuration;
        while (Time.time < endTime)
        {
            rb.velocity = chargeDirection * chargeSpeed;
            yield return null;
        }
        rb.velocity = Vector2.zero;
        isCharging = false;

        // Apply damage to enemies in the attack radius
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(effectPosition, weaponData.stats.explosionRadius);
        ApplyDamage(hitEnemies);

        // Optionally deactivate the ground impact effect
        if (groundImpactEffect != null)
        {
            StartCoroutine(DeactivateEffectAfterDelay(groundImpactEffect, 1f)); // 1 second delay before deactivating
        }
    }

    // Coroutine to deactivate the effect after a delay
    private IEnumerator DeactivateEffectAfterDelay(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        effect.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        // Draw gizmos in the editor to visualize the attack radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.up * effectDistanceFromCharacter, attackRadius);
    }
}