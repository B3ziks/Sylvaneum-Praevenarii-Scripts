using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExplodingMine : MonoBehaviour
{
    private int damage;  // Damage dealt by the mine when it explodes
    public delegate void ExplodeAction(GameObject mine);
    public event ExplodeAction OnExplode;
    [SerializeField] private PoolObjectData explosionEffectPoolData;
    [SerializeField] private float explosionRadius = 1.0f; // radius of the explosion

    public Transform playerTransform; // Reference to the player's transform
    public float maxDistanceFromPlayer = 10.0f; // Maximum distance mine can be from the player

    private PoolManager poolManager;

    private void Start()
    {
        poolManager = FindObjectOfType<PoolManager>();
        if (playerTransform == null) // Safety check in case you forgot to assign the player's transform
        {
            playerTransform = FindObjectOfType<PlayerMove>().transform; // This assumes you have a PlayerMove component on your player. Adjust as necessary.
        }
    }

    private void Update()
    {
        TeleportToPlayerIfTooFar();
    }

    public void SetDamage(int damageAmount)
    {
        damage = damageAmount;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Explode();
        }
    }

    void TeleportToPlayerIfTooFar()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > maxDistanceFromPlayer)
        {
            Vector2 newPosition = FindNewPositionAroundPlayer();
            if (newPosition != Vector2.zero)
            {
                transform.position = newPosition;
            }
            else
            {
                UnityEngine.Debug.LogWarning("Failed to find a new valid position for the mine!");
            }
        }
    }

    Vector2 FindNewPositionAroundPlayer()
    {
        // This is an example approach to find a new position around the player.
        // Adjust as needed for your game's specifics.
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
        return playerTransform.position + (Vector3)randomDirection * maxDistanceFromPlayer * 0.5f;
    }

    void Explode()
    {
        // Area damage
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hitCollider in hitColliders)
        {
            IDamageable target = hitCollider.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damage);
                PostDamage(damage, hitCollider.transform.position);
            }
        }

        SpawnExplosionEffect();
        OnExplode?.Invoke(gameObject);
        Destroy(gameObject);
    }

    private void SpawnExplosionEffect()
    {
        if (poolManager && explosionEffectPoolData)
        {
            GameObject explosionEffect = poolManager.GetObject(explosionEffectPoolData);
            if (explosionEffect)
            {
                explosionEffect.transform.position = transform.position;
                explosionEffect.transform.rotation = Quaternion.identity;
                explosionEffect.transform.localScale = Vector3.one; // Adjust scale if needed
            }
            else
            {
                UnityEngine.Debug.LogError("Failed to get explosion effect from pool.");
            }
        }
        else
        {
            UnityEngine.Debug.LogError("PoolManager or explosionEffectPoolData is not set.");
        }
    }

    public void PostDamage(int damageDealt, Vector3 targetPosition)
    {
        MessageSystem.instance.PostMessage(damageDealt.ToString(), targetPosition);
    }
}