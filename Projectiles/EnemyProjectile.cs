using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour, IPoolMember
{
    public float projectileSpeed = 5f;
    public int damage;
    public float ttl = 6f;  // Time To Live

    PoolMember poolMember;

    protected Transform playerTransform;
    protected float timeAlive;
    protected Rigidbody2D rb;
    private bool shouldSetVelocity;
    public EnemyData shootingEnemyData; // Reference to the enemy data that shot this projectile
                                        // New variables
    public float elementalPotency;
    public float elementalDamageOverTime;


    protected void OnEnable()
    {
        rb = rb ?? GetComponent<Rigidbody2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform == null)
        {
            UnityEngine.Debug.LogError("Player not found!");
            return;
        }

        shouldSetVelocity = true;
        timeAlive = 0;

        // Check if shootingEnemyData and its stats are not null before assigning values
        if (shootingEnemyData != null && shootingEnemyData.stats != null)
        {
            elementalPotency = shootingEnemyData.stats.elementalPotency;
            elementalDamageOverTime = shootingEnemyData.stats.elementalDamageOverTime;
           // UnityEngine.Debug.Log($"Projectile enabled with Elemental Potency: {elementalPotency} and DOT: {elementalDamageOverTime}");
        }
        else
        {
            elementalPotency = 0; // Default value
            elementalDamageOverTime = 0; // Default value
           // UnityEngine.Debug.LogWarning("shootingEnemyData or its stats are null. Using default values for elementalPotency and elementalDamageOverTime.");
        }
    }

    protected virtual void Update()
    {
        if (shouldSetVelocity)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            rb.velocity = direction * projectileSpeed;
            shouldSetVelocity = false;
            UnityEngine.Debug.DrawRay(transform.position, direction * 5f, Color.red, 2f);

            SetDirection(direction.x, direction.y); // Set the rotation after determining the direction
        }

        timeAlive += Time.deltaTime;
        if (timeAlive >= ttl)
        {
            DeactivateProjectile();
        }
    }

    public void SetDirection(float dir_x, float dir_y)
    {
        float angle = Mathf.Atan2(dir_y, dir_x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void SetPoolMember(PoolMember poolMember)
    {
        this.poolMember = poolMember;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // Check for null references first
        if (shootingEnemyData == null || shootingEnemyData.stats == null)
        {
            UnityEngine.Debug.LogWarning("ShootingEnemyData or its stats is null!");
            return;
        }

        //UnityEngine.Debug.Log($"Projectile triggered with: ElementType = {shootingEnemyData.stats.elementType}, ElementalPotency = {elementalPotency}, ElementalDamageOverTime = {elementalDamageOverTime}");

        if (other.CompareTag("Player"))
        {
            Character playerCharacter = other.GetComponent<Character>();
            playerCharacter.TakeDamage(damage);

            ElementalEffectManager effectManager = playerCharacter.GetComponent<ElementalEffectManager>();
            // Check elemental type and apply effect accordingly
            if (shootingEnemyData.stats.elementType == ElementType.Poison)
            {
                effectManager.ApplyElementalEffect(ElementType.Poison, elementalPotency, elementalDamageOverTime);
            }

            DeactivateProjectile();
        }
        else if (other.CompareTag("Obstacle"))
        {
            DeactivateProjectile();
        }
    }

    public void ResetProjectile()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    protected virtual void DeactivateProjectile()
    {
        if (poolMember != null)
        {
            ResetProjectile();
            poolMember.ReturnToPool();

            // We'll no longer reset shootingEnemyData here to ensure it's available the next time OnEnable is called.
        }
        else
        {
            Destroy(gameObject);
        }
    }
}