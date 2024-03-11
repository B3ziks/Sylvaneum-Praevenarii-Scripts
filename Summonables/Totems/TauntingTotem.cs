using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TauntingTotem : MonoBehaviour
{
    public float damageRadius = 3f;
    public float pullForce = 50f;
    public float pullRadius = 8f;
    private float damageCountdown;
    private float attackInterval;
    public WeaponBase weaponBaseReference;
    [SerializeField] private GameObject tauntEffect; // Effect GameObject
    //
    public Transform playerTransform;
    public float maxDistanceFromPlayer = 10f;
    public Vector3 offsetFromPlayer = new Vector3(2f, 2f, 0f);

    private void Start()
    {
        attackInterval = weaponBaseReference.weaponData.stats.timeToAttack;
        damageCountdown = attackInterval;
        tauntEffect.SetActive(false);
        if (playerTransform == null)
        {
            playerTransform = FindObjectOfType<PlayerMove>().transform;
        }
    }

    private void Update()
    {
        damageCountdown -= Time.deltaTime;
        if (damageCountdown <= 0f)
        {
            StartCoroutine(AttackProcess());
            damageCountdown = attackInterval;
        }
        TeleportToPlayerIfTooFar();

    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy")) // Ensure this matches your enemy tag
        {
            Rigidbody2D enemyRb = collider.GetComponent<Rigidbody2D>();
            if (enemyRb != null && ShouldPull())
            {
                Vector2 forceDirection = (transform.position - collider.transform.position).normalized;
                enemyRb.AddForce(forceDirection * pullForce);
            }
        }
    }

    private bool ShouldPull()
    {
        // Pull only during the first half of the attack interval
        return damageCountdown > attackInterval / 2;
    }

    private IEnumerator AttackProcess()
    {
        Vector3 effectPosition = transform.position;
        tauntEffect.transform.position = effectPosition;
        tauntEffect.transform.localScale = new Vector3(damageRadius, damageRadius, 1f);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(effectPosition, damageRadius);
        ApplyAreaDamage(hitEnemies);

        tauntEffect.SetActive(true);
        yield return new WaitForSeconds(attackInterval);

        tauntEffect.SetActive(false);
    }

    private void ApplyAreaDamage(Collider2D[] hitEnemies)
    {
        foreach (Collider2D enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(weaponBaseReference.weaponData.stats.damage);
                weaponBaseReference.PostDamage(weaponBaseReference.weaponData.stats.damage, enemy.transform.position);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, pullRadius);
    }
    //
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
                UnityEngine.Debug.LogWarning("Failed to find a new valid position for the totem!");
            }
        }
    }

    Vector2 FindNewPositionAroundPlayer()
    {
        TauntingTotemSpawner spawner = weaponBaseReference as TauntingTotemSpawner;
        if (spawner != null)
        {
            return spawner.CalculateSpawnPositionAroundPlayer();
        }
        return Vector2.zero;
    }
}