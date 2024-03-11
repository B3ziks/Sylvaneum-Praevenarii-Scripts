using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BearSummon : MonoBehaviour
{
    private IDamageable currentTarget;
    public float attackRange = 5f;
    private float attackCooldown;

    [Header("Weapon Data")]
    public WeaponBase weaponBaseReference;
    public GameObject groundSlamEffect; // Prefab for the ground slam effect.
    public float slamRadius = 2f; // AoE radius

    [Header("Player Data")]
    public Transform playerTransform;
    public float maxDistanceFromPlayer = 6f;
    public Vector3 offsetFromPlayer = new Vector3(2f, 2f, 0f);
    public float attackArea = 2f;
    private bool isAttacking = false;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        CircleCollider2D detectionCollider = gameObject.AddComponent<CircleCollider2D>();
        detectionCollider.radius = attackRange;
        detectionCollider.isTrigger = true;

        attackCooldown = weaponBaseReference.weaponData.stats.timeToAttack;

        if (playerTransform == null)
        {
            playerTransform = FindObjectOfType<PlayerMove>().transform;
        }
    }

    private void Update()
    {
        // Existing code for targeting and attacking...
        FollowPlayerIfTooFar();

        // Check for attack cooldown
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
        else
        {
            // Detect targets within attackRange and select one to attack
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
            foreach (var hitCollider in hitColliders)
            {
                IDamageable potentialTarget = hitCollider.GetComponent<IDamageable>();
                if (potentialTarget != null)
                {
                    currentTarget = potentialTarget;
                    break; // Break after finding the first damageable target
                }
            }

            // If we have a target, approach and attack
            if (currentTarget != null)
            {
                ApproachAndAttackTarget(currentTarget);
            }
        }
    }

    // ApproachAndAttackTarget method remains mostly the same...
    private void ApproachAndAttackTarget(IDamageable target)
    {
        Vector2 direction = ((Vector2)((UnityEngine.Component)target).transform.position - (Vector2)transform.position).normalized;
        float distanceToTarget = Vector2.Distance(transform.position, ((UnityEngine.Component)target).transform.position);

        if (distanceToTarget > attackArea)
        {
            transform.position += (Vector3)direction * Time.deltaTime * weaponBaseReference.weaponData.stats.moveSpeed;
            animator.SetBool("isMoving", true);
            animator.SetBool("isAttacking", false);  // <-- Set isAttacking to false here
        }
        else
        {
            if (!isAttacking)
            {
                StartCoroutine(AttackCoroutine(target));
            }
        }

        FlipSpriteBasedOnDirection(direction);
    }
    private IEnumerator AttackCoroutine(IDamageable target)
    {
        isAttacking = true;
        animator.SetBool("isAttacking", true);
        Attack(target);
        yield return new WaitForSeconds(weaponBaseReference.weaponData.stats.timeToAttack);
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    private void Attack(IDamageable target)
    {
        // Instead of single target damage, we use AoE
        ActivateAoEAttack(target);
    }

    private void ActivateAoEAttack(IDamageable target)
    {
        // Ensure that 'target' is cast to 'Component' to access the 'transform' property for position
        Vector3 effectPosition = ((UnityEngine.Component)target).transform.position + new Vector3(0, 0, -1); // Adjust if necessary
        StartCoroutine(GroundSlamEffect(effectPosition));
    }

    IEnumerator GroundSlamEffect(Vector3 effectPosition)
    {
        groundSlamEffect.transform.position = effectPosition;
        groundSlamEffect.transform.localScale = new Vector3(slamRadius, slamRadius, 1f);

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(effectPosition, slamRadius);
        foreach (var hitCollider in hitColliders)
        {
            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                int damageValue = weaponBaseReference.weaponData.stats.damage;
                damageable.TakeDamage(damageValue);
                weaponBaseReference.PostDamage(damageValue, hitCollider.transform.position);
            }
        }

        groundSlamEffect.SetActive(true);
        yield return new WaitForSeconds(weaponBaseReference.weaponData.stats.timeToAttack);
        groundSlamEffect.SetActive(false);
    }
    void FollowPlayerIfTooFar()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > maxDistanceFromPlayer)
        {
            Vector2 directionToPlayer = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
            transform.position += (Vector3)directionToPlayer * Time.deltaTime * weaponBaseReference.weaponData.stats.moveSpeed;
            animator.SetBool("isMoving", true);
           animator.SetBool("isAttacking", false);  // <-- Set isAttacking to false here
            FlipSpriteBasedOnDirection(directionToPlayer);
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
}