using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostTurretProjectile : TurretProjectile
{

    // Override the DetectHits method to apply elemental effects.
    protected override void DetectHits()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, attackArea);
        foreach (Collider2D c in hit)
        {
            if (numOfHits <= 0)
            {
                DeactivateProjectile();
                break;
            }
            IDamageable enemy = c.GetComponent<IDamageable>();
            if (enemy != null && !AlreadyHit(enemy))
            {
                weapon.ApplyDamage(c.transform.position, damage, enemy);
                ApplyElementalEffect(enemy); // Apply the elemental effect.
                enemiesHit.Add(enemy);
                numOfHits -= 1;
            }
        }

        if (numOfHits <= 0)
        {
            DeactivateProjectile();
        }
    }

    // This method applies the elemental effect to the enemy.
    private void ApplyElementalEffect(IDamageable enemy)
    {
        // Since we have elementalPotency, we could use it as either duration or potency for the effect.
        enemy.ApplyElementalEffect(elementType, elementalPotency, damage);
    }




}