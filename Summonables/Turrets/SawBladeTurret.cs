using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBladeTurret : MonoBehaviour
{
    public Transform bladeTransform; // Assign the blade GameObject's Transform

    public Transform playerTransform; // Drag and drop your player object here in the Inspector
    public float maxDistanceFromPlayer = 10f; // Set this to whatever distance you consider "too far"
    public Vector3 offsetFromPlayer = new Vector3(2f, 2f, 0f); // Offset so turret doesn't spawn directly on top of the player

    public WeaponBase weaponBaseReference; // Reference to your WeaponBase script

    private int damage; // Damage per attack
    private float attackRate; // How often the turret attacks per second
    private float attackCooldown; // Cooldown timer for attacks

    private void Start()
    {
        if (playerTransform == null) // Safety check in case you forgot to assign the player's transform
        {
            playerTransform = FindObjectOfType<PlayerMove>().transform; // Adjust as necessary
        }

        // Initialize weapon stats
        damage = weaponBaseReference.weaponData.stats.damage;
        attackRate = 1f / weaponBaseReference.weaponData.stats.timeToAttack; // Convert timeToAttack to an attack rate
        attackCooldown = 0f;
    }

    private void Update()
    {
        // Spin the blade
        bladeTransform.Rotate(Vector3.forward, (attackRate/3) * 360f * Time.deltaTime);

        // Teleport the turret back to the player if it's too far away
        TeleportToPlayerIfTooFar();

        // Check attack cooldown
        if (attackCooldown <= 0f)
        {
            // Damage enemies in contact
            DamageEnemiesInContact();
            // Reset attack cooldown
            attackCooldown = 1f / attackRate;
        }
        else
        {
            // Reduce attack cooldown over time
            attackCooldown -= Time.deltaTime;
        }
    }

    private void TeleportToPlayerIfTooFar()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > maxDistanceFromPlayer)
        {
            Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0).normalized;
            transform.position = playerTransform.position + offsetFromPlayer + randomOffset;
        }
    }

    private void DamageEnemiesInContact()
    {
        // Assuming bladeTransform is the transform of the blade game object
        float bladeRadius = bladeTransform.localScale.x; // Or another appropriate measure of the blade's size
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(bladeTransform.position, bladeRadius); // Make sure to define enemyLayerMask to filter only enemy colliders

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.isTrigger) continue; // Skip triggers if necessary

            IDamageable damageableEnemy = enemy.GetComponent<IDamageable>();
            if (damageableEnemy != null)
            {
                // Deal damage using the weapon's damage value
                damageableEnemy.TakeDamage(weaponBaseReference.weaponData.stats.damage);

                // Post a message with the damage amount
                if (MessageSystem.instance != null)
                {
                    MessageSystem.instance.PostMessage(weaponBaseReference.weaponData.stats.damage.ToString(), enemy.transform.position);
                }
            }
        }
    }

    //gizmos
    void OnDrawGizmosSelected()
    {
        // Draw a red wire sphere representing the blade's area
        Gizmos.color = Color.red;
        if (bladeTransform != null)
        {
            Gizmos.DrawWireSphere(bladeTransform.position, bladeTransform.localScale.x);
        }
    }
}