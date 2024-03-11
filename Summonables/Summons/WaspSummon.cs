using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class WaspSummon : MonoBehaviour
{
    private IDamageable currentTarget;
    public PoolObjectData projectilePoolData;
    public Transform waspBarrelTransform;  // Position from which projectiles are fired.
    public float attackRange = 5f;
    private float fireCountdown;
    PoolManager poolManager;
    public WeaponBase weaponBaseReference;
    private Animator animator;
    private bool isAttacking = false;

    [Header("Player Data")]
    public Transform playerTransform;
    public float maxDistanceFromPlayer = 10f;
    public Vector3 offsetFromPlayer = new Vector3(2f, 2f, 0f);

    private void Start()
    {
        poolManager = FindObjectOfType<PoolManager>();
        animator = GetComponent<Animator>();  // Get the Animator component reference

        CircleCollider2D detectionCollider = gameObject.AddComponent<CircleCollider2D>();
        detectionCollider.radius = attackRange;
        detectionCollider.isTrigger = true;

        fireCountdown = weaponBaseReference.weaponData.stats.timeToAttack;

        if (playerTransform == null)
        {
            playerTransform = FindObjectOfType<PlayerMove>().transform;
        }
        // Check if the wasp is too far from the player immediately after game starts
        FollowPlayerIfTooFar();
    }

    private void Update()
    {
        if (currentTarget == null || ((UnityEngine.Component)currentTarget).gameObject == null)
        {
            currentTarget = null;
            animator.SetBool("isMoving", false);
            animator.SetBool("isAttacking", false);
            return;
        }

        AimAtTarget(currentTarget);
        fireCountdown -= Time.deltaTime;
        if (fireCountdown <= 0f)
        {
            ShootAtTarget(currentTarget);
            fireCountdown = weaponBaseReference.weaponData.stats.timeToAttack;
        }

        FollowPlayerIfTooFar();
    }

    private void AimAtTarget(IDamageable target)
    {
        Vector2 direction = ((Vector2)((UnityEngine.Component)target).transform.position - (Vector2)waspBarrelTransform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        waspBarrelTransform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void ShootAtTarget(IDamageable target)
    {
        GameObject projectileGO = SpawnProjectileFromPool(projectilePoolData, waspBarrelTransform.position, target);
        WaspProjectile projectile = projectileGO.GetComponent<WaspProjectile>();
        Vector2 direction = ((UnityEngine.Component)target).transform.position - waspBarrelTransform.position;
        projectile.SetDirection(direction.x, direction.y);
        projectile.SetStats(weaponBaseReference);
        //
        animator.SetBool("isMoving", false);
        animator.SetBool("isAttacking", true);

    }

    private GameObject SpawnProjectileFromPool(PoolObjectData poolObjectData, Vector3 position, IDamageable target)
    {
        GameObject projectileGO = poolManager.GetObject(poolObjectData);
        projectileGO.transform.position = position;
        WaspProjectile projectile = projectileGO.GetComponent<WaspProjectile>();
        Vector2 direction = ((UnityEngine.Component)target).transform.position - position;
        projectile.SetDirection(direction.x, direction.y);
        projectile.SetStats(weaponBaseReference);
        return projectileGO;
    }

    public void SetPoolManager(PoolManager poolManager)
    {
        this.poolManager = poolManager;
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
   

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}