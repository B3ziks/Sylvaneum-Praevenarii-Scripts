using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IceTotem : MonoBehaviour
{
    public float damageRadius = 5f;
    private float damageCountdown;
    public WeaponBase weaponBaseReference;
    [SerializeField] private GameObject groundHoleEffect; // Effect GameObject
    //
    public Transform playerTransform; // Drag and drop your player object here in the Inspector
    public float maxDistanceFromPlayer = 10f; // Set this to whatever distance you consider "too far"
    public Vector3 offsetFromPlayer = new Vector3(2f, 2f, 0f); // Offset so turret doesn't spawn directly on top of the player


    private void Start()
    {
        damageCountdown = weaponBaseReference.weaponData.stats.timeToAttack;
        if (playerTransform == null) // Safety check in case you forgot to assign the player's transform
        {
            playerTransform = FindObjectOfType<PlayerMove>().transform; // This assumes you have a PlayerMove component on your player. Adjust as necessary.
        }
        // Ensure groundHoleEffect is already a child of EarthTotem in the scene and deactivate it
        groundHoleEffect.SetActive(false);
    }

    private void Update()
    {
        damageCountdown -= Time.deltaTime;
        if (damageCountdown <= 0f)
        {

            Vector3 effectPosition = transform.position;

            StartCoroutine(AttackProcess(effectPosition));
            damageCountdown = weaponBaseReference.weaponData.stats.timeToAttack;
        }
        TeleportToPlayerIfTooFar();

    }

    private IEnumerator AttackProcess(Vector3 effectPosition)
    {
        groundHoleEffect.transform.position = effectPosition;
        groundHoleEffect.transform.localScale = new Vector3(damageRadius, damageRadius, 1f);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(effectPosition, damageRadius);
        ApplyAreaDamage(hitEnemies);

        groundHoleEffect.SetActive(true);
        yield return new WaitForSeconds(weaponBaseReference.weaponData.stats.timeToAttack);

        // Assuming the groundHoleEffect manages its own active state or is a one-time effect
        // If you want to deactivate it manually, uncomment the following line
        // groundHoleEffect.SetActive(false);
    }

    private void ApplyAreaDamage(Collider2D[] hitEnemies)
    {
        foreach (Collider2D enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(weaponBaseReference.weaponData.stats.damage);
                // Optionally, spawn a visual effect from a pool here
                // Applying additional effects
                weaponBaseReference.ApplyAdditionalEffects(damageable, enemy.transform.position);

                // Post the damage message
                weaponBaseReference.PostDamage(weaponBaseReference.weaponData.stats.damage, enemy.transform.position);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
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
                // In case no valid position is found after several attempts, we can optionally destroy the turret or log a warning.
                UnityEngine.Debug.LogWarning("Failed to find a new valid position for the turret!");
            }
        }
    }

    Vector2 FindNewPositionAroundPlayer()
    {
        IceTotemSpawner spawner = weaponBaseReference as IceTotemSpawner;
        if (spawner != null)
        {
            return spawner.CalculateSpawnPositionAroundPlayer();
        }
        return Vector2.zero;
    }
}