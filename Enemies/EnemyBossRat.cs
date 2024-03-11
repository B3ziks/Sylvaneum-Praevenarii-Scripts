using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossState
{
    Idle,
    Attacking,
    Cooldown
}

public class EnemyBossRat : EnemyRanged
{
    private Animator animator;
    private float attackTimer;
    public GameObject coin;
    private float ratOriginalMoveSpeed;

    private BossState currentState;
    [SerializeField] private bool playerInZone = false;

    protected override void Start()
    {
        base.Start();
        currentState = BossState.Idle;
        attackTimer = stats.timeToAttack;

        animator = GetComponentInChildren<Animator>();
        coin = transform.Find("BossRatSprite(Clone)/Coin").gameObject;
        if (coin)
        {
            coin.SetActive(false);
        }

        ratOriginalMoveSpeed = stats.moveSpeed; // Store the original movement speed here
    }

    private void Update()
    {
        base.Update();
        switch (currentState)
        {
            case BossState.Idle:
                HandleIdleState();
                break;
            case BossState.Attacking:
                HandleAttackingState();
                break;
            case BossState.Cooldown:
                HandleCooldownState();
                break;
        }
    }

    private void HandleIdleState()
    {
        if (playerInZone)
        {
            currentState = BossState.Attacking;
        }
    }

    private void HandleAttackingState()
    {
        stats.moveSpeed = 0;  // Make sure the boss doesn't move during attack

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            attackTimer = stats.timeToAttack;
            StartCoroutine(AttackSequence());
        }
    }

    private void HandleCooldownState()
    {
        if (!playerInZone)
        {
            animator.SetBool("isAttacking", false);
            coin.SetActive(false);
            stats.moveSpeed = ratOriginalMoveSpeed;  // Reset to original move speed after attack
            currentState = BossState.Idle;
        }
    }

    IEnumerator AttackSequence()
    {
        animator.SetBool("isAttacking", true);
        coin.SetActive(true);
        yield return new WaitForSeconds(2f);  // Wait for the attacking animation to complete.
        ShootProjectile();
        coin.SetActive(false);
        yield return new WaitForSeconds(0.5f);  // Cooldown period after shooting.
        currentState = BossState.Cooldown;
    }

    private new void ShootProjectile()  // Use 'new' keyword to explicitly hide the base class method.
    {
        base.ShootProjectile();
    }

    public void SetPlayerInZone(bool state)
    {
        playerInZone = state;
    }

    // This method might be redundant now, since we're managing states.
    public void SetIsAttacking(bool state)
    {
        // Consider removing if not used elsewhere.
    }
}