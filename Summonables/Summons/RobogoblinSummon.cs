using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RobogoblinSummon : MonoBehaviour
{
    private IDamageable currentTarget;
    public float attackRange = 5f;

    [Header("Weapon Data")]
    public WeaponBase weaponBaseReference;
    public PoolObjectData explosionEffectPrefab;
    private PoolManager poolManager;

    [Header("Player Data")]
    public Transform playerTransform;
    public float maxDistanceFromPlayer = 6f;
    public float selfDestructArea = 2f;
    private bool isSelfDestructing = false;
    public Vector3 offsetFromPlayer = new Vector3(2f, 2f, 0f);

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        poolManager = FindObjectOfType<PoolManager>(); // Ensure poolManager is initialized

        CircleCollider2D detectionCollider = gameObject.AddComponent<CircleCollider2D>();
        detectionCollider.radius = attackRange;
        detectionCollider.isTrigger = true;

        if (playerTransform == null)
        {
            playerTransform = FindObjectOfType<PlayerMove>().transform;
        }

    }

    private void Update()
    {
        if (isSelfDestructing)
        {
            // Skip other actions while self-destructing
            return;
        }

        if (currentTarget == null || ((UnityEngine.Component)currentTarget).gameObject == null)
        {
            currentTarget = null;
            animator.SetBool("isMoving", false);
        }
        else
        {
            ApproachAndSelfDestruct(currentTarget);
        }

        FollowPlayerIfTooFar();
    }

    private void ApproachAndSelfDestruct(IDamageable target)
    {
        Vector2 direction = ((Vector2)((UnityEngine.Component)target).transform.position - (Vector2)transform.position).normalized;
        float distanceToTarget = Vector2.Distance(transform.position, ((UnityEngine.Component)target).transform.position);

        if (distanceToTarget <= selfDestructArea)
        {
            StartCoroutine(SelfDestructCoroutine());
        }
        else
        {
            transform.position += (Vector3)direction * Time.deltaTime * weaponBaseReference.weaponData.stats.moveSpeed;
            animator.SetBool("isMoving", true);
        }

        FlipSpriteBasedOnDirection(direction);
    }

    private IEnumerator SelfDestructCoroutine()
    {
        isSelfDestructing = true;
        animator.SetBool("isAttacking", true);

        // Explosion effect
        GameObject explosionEffect = poolManager.GetObject(explosionEffectPrefab);
        explosionEffect.transform.position = transform.position;
        explosionEffect.SetActive(true);

        ApplyAreaDamage();

        yield return new WaitForSeconds(0.5f); // Adjust delay as needed for the explosion effect

        Destroy(gameObject); // Destroy the object instead of deactivating
    }

    private void ApplyAreaDamage()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, selfDestructArea);
        foreach (var hitCollider in hitColliders)
        {
            IDamageable enemy = hitCollider.GetComponent<IDamageable>();
            if (enemy != null)
            {
                int damageValue = weaponBaseReference.weaponData.stats.damage;
                enemy.TakeDamage(damageValue);
                weaponBaseReference.PostDamage(damageValue, hitCollider.transform.position);
            }
        }
    }

    private void FlipSpriteBasedOnDirection(Vector3 direction)
    {
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        IDamageable enemy = col.GetComponent<IDamageable>();
        if (enemy != null)
        {
            if (currentTarget == null)
            {
                currentTarget = enemy;
            }
            else
            {
                float distanceToCurrentTarget = Vector2.Distance(transform.position, ((UnityEngine.Component)currentTarget).transform.position);
                float distanceToNewTarget = Vector2.Distance(transform.position, col.transform.position);
                if (distanceToNewTarget < distanceToCurrentTarget)
                {
                    currentTarget = enemy;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.GetComponent<IDamageable>() == currentTarget)
        {
            currentTarget = null;
        }
    }

    void FollowPlayerIfTooFar()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > maxDistanceFromPlayer)
        {
            Vector2 directionToPlayer = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
            transform.position += (Vector3)directionToPlayer * Time.deltaTime * weaponBaseReference.weaponData.stats.moveSpeed;
            animator.SetBool("isMoving", true);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, selfDestructArea);
    }
}