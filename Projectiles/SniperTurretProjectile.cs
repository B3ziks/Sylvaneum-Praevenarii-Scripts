using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperTurretProjectile : TurretProjectile
{
    private Transform currentTarget;

    protected override void Update()
    {
        // Seek target if no current target or the current target is no longer valid
        if (currentTarget == null || !IsValidTarget(currentTarget))
        {
            currentTarget = FindEliteOrBossTarget();
        }

        // Set projectile direction towards the current target
        if (currentTarget != null)
        {
            Vector3 targetDirection = (currentTarget.position - transform.position).normalized;
            SetDirection(targetDirection.x, targetDirection.y);
        }

        base.Update();
    }

    private Transform FindEliteOrBossTarget()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (var enemy in enemies)
        {
            if (enemy.enemyData != null && (enemy.enemyData.isElite || enemy.enemyData.isBoss))
            {
                return enemy.transform;
            }
        }
        return null; // No elite or boss targets found
    }

    private bool IsValidTarget(Transform target)
    {
        if (target == null) return false;
        Enemy enemy = target.GetComponent<Enemy>();
        return enemy != null && (enemy.enemyData.isElite || enemy.enemyData.isBoss);
    }

    protected override void DetectHits()
    {
        // Only detect hits if the current target is valid
        if (IsValidTarget(currentTarget))
        {
            base.DetectHits();
        }
    }
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsValidTarget(other.transform))
        {
            return; // Ignore collision if it's not with an elite or boss
        }

        base.OnTriggerEnter2D(other);
    }

    // Rest of the code remains the same as in TurretProjectile
}