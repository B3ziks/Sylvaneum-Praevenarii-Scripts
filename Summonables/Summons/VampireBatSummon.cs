using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireBatSummon : MonoBehaviour
{
    private IDamageable currentTarget;
    public float attackRange = 5f;
    private float attackCooldown;

    [Header("Weapon Data")]
    public WeaponBase weaponBaseReference;

    [Header("Player Data")]
    public Character playerScript; // Assuming there is a Character script with a Heal method
    public Transform playerTransform;
    public float maxDistanceFromPlayer = 6f;
    public Vector3 offsetFromPlayer = new Vector3(2f, 2f, 0f);
    public float attackArea = 2f;
    private bool isAttacking = false;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (playerScript == null)
        {
            playerScript = FindObjectOfType<Character>();
        }
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
        if (currentTarget == null || ((UnityEngine.Component)currentTarget).gameObject == null)
        {
            currentTarget = null;
            animator.SetBool("isMoving", false);
            animator.SetBool("isAttacking", false);
        }
        else
        {
            ApproachAndAttackTarget(currentTarget);
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0f)
            {
                if (!isAttacking)
                {
                    StartCoroutine(AttackCoroutine(currentTarget));
                }
                attackCooldown = weaponBaseReference.weaponData.stats.timeToAttack;
            }
        }

        FollowPlayerIfTooFar();
    }

    private void ApproachAndAttackTarget(IDamageable target)
    {
        Vector2 direction = ((Vector2)((UnityEngine.Component)target).transform.position - (Vector2)transform.position).normalized;
        float distanceToTarget = Vector2.Distance(transform.position, ((UnityEngine.Component)target).transform.position);

        // Check if the bat is in attack area and attack cooldown is done
        if (distanceToTarget <= attackArea && attackCooldown <= 0f && !isAttacking)
        {
            StartCoroutine(AttackCoroutine(target));
        }
        else if (distanceToTarget > attackArea)
        {
            // Move towards the target if outside attack area
            transform.position += (Vector3)direction * Time.deltaTime * weaponBaseReference.weaponData.stats.moveSpeed;
            animator.SetBool("isMoving", true);
            animator.SetBool("isAttacking", false);
            FlipSpriteBasedOnDirection(direction);
        }
    }

    private IEnumerator AttackCoroutine(IDamageable target)
    {
        isAttacking = true;
        animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(0.2f); // Bat stands still for a second

        // Attack the target and heal the player
        AttackAndHeal(target);

        yield return new WaitForSeconds(weaponBaseReference.weaponData.stats.timeToAttack - 1f); // Remaining attack cooldown
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    private void AttackAndHeal(IDamageable target)
    {
        int damageValue = weaponBaseReference.weaponData.stats.damage;
        int healAmount = weaponBaseReference.weaponData.stats.damage/5;
        target.TakeDamage(damageValue);
        playerScript.Heal(healAmount); // Heal the player
        animator.SetBool("isMoving", false);

        // Post the damage message
        weaponBaseReference.PostDamage(damageValue, ((UnityEngine.Component)target).transform.position);
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
            animator.SetBool("isAttacking", false);  // <-- Set isAttacking to false here
            FlipSpriteBasedOnDirection(directionToPlayer);
        }
    }
}