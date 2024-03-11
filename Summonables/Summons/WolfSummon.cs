using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfSummon : MonoBehaviour
{
    private IDamageable currentTarget;
    public float attackRange = 5f;
    private float attackCooldown;

    [Header("Weapon Data")]
    public WeaponBase weaponBaseReference;

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
        animator.SetBool("isAttacking", true);  // Set the animation trigger

        Attack(target);
        yield return new WaitForSeconds(weaponBaseReference.weaponData.stats.timeToAttack);
        isAttacking = false;
        animator.SetBool("isAttacking", false); // Reset the animation trigger
    }

    private void Attack(IDamageable target)
    {
        int damageValue = weaponBaseReference.weaponData.stats.damage;  // Storing the damage value in a variable
        target.TakeDamage(damageValue);
        animator.SetBool("isMoving", false);

        // Post the damage message
        weaponBaseReference.PostDamage(damageValue, ((UnityEngine.Component)target).transform.position);
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


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackArea);
    }
}