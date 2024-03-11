using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingAura : Aura
{
    public int healingAmount = 2;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            ApplyAuraEffect(collision);
        }
    }

    protected override void ApplyAuraEffect(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy == null) return;

        enemy.stats.hp += healingAmount; // Heal the enemy.
    }

    protected override void RemoveAuraEffect(Collider2D collision)
    {
        // No effect removal for healing.
    }
}