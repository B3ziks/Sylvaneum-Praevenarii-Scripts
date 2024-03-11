using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShamanCreatureProjectile : MonoBehaviour, IPoolMember
{
    public float speed = 5.0f; // Speed of the projectile
    public int damage = 10; // Damage of the projectile
    public float ttl = 5.0f; // Time to live for the projectile

    private Transform target; // Target the projectile will move towards
    private Vector3 direction;
    private PoolMember poolMember;

    // Set the target from the spawner
    public void Initialize(Transform newTarget)
    {
        target = newTarget;
        CalculateDirectionTowardsTarget();
        ttl = 5.0f; // Reset the time to live
    }

    private void CalculateDirectionTowardsTarget()
    {
        if (target != null)
        {
            // Calculate the direction vector towards the player
            direction = (target.position - transform.position).normalized;
        }
    }

    void Update()
    {
        MoveProjectile();
        CountdownToLive();
    }

    private void MoveProjectile()
    {
        // Move the projectile in the set direction
        transform.position += direction * speed * Time.deltaTime;
    }

    private void CountdownToLive()
    {
        ttl -= Time.deltaTime;
        if (ttl <= 0f)
        {
            DeactivateProjectile();
        }
    }

    private void DeactivateProjectile()
    {
        if (poolMember != null)
        {
            poolMember.ReturnToPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPoolMember(PoolMember member)
    {
        poolMember = member;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it hits the player, apply damage, and deactivate
        Character player = other.GetComponent<Character>();
        if (player != null)
        {
            player.TakeDamage(damage);
            DeactivateProjectile();
        }
    }

    // This function is called when the object becomes enabled and active
    private void OnEnable()
    {
        // Assuming the target is set by the spawner before enabling the projectile
        CalculateDirectionTowardsTarget();
    }
}