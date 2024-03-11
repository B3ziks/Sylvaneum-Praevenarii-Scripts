using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class Aura : MonoBehaviour
{
    public float radius = 1f;
    private List<Enemy> affectedEnemies = new List<Enemy>();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        ApplyAuraEffect(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        RemoveAuraEffect(collision);
    }

    protected abstract void ApplyAuraEffect(Collider2D collision);
    protected abstract void RemoveAuraEffect(Collider2D collision);

    private void OnDestroy()
    {
        foreach (var enemy in affectedEnemies)
        {
            if (enemy != null)
            {
                RemoveAuraEffectFromEnemy(enemy);
            }
        }
    }

    protected void ApplyAuraEffectToEnemy(Enemy enemy, List<BuffAura> buffAuras, List<DebuffAura> debuffAuras)
    {
        if (!affectedEnemies.Contains(enemy))
        {
            affectedEnemies.Add(enemy);
        }

        // The aura application logic goes here...
        // This can be the logic you had previously for buffing/debuffing the enemy
    }

    protected void RemoveAuraEffectFromEnemy(Enemy enemy)
    {
        // The aura removal logic goes here...
        // This can be the logic you had previously for removing buffs/debuffs from the enemy
    }
}