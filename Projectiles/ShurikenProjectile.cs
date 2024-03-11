using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenProjectile : Projectile, IPoolMember
{
    private int maxRicochets; // The max number of times the projectile can ricochet.
    private int currentRicochets = 0;


    // Override any methods from the base class (Projectile) that need specialized behavior
    public override void HitDetection()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, attackArea);
        foreach (Collider2D c in hit)
        {
            IDamageable enemy = c.GetComponent<IDamageable>();
            if (enemy != null && !CheckRepeatHit(enemy))
            {
                weapon.ApplyDamage(c.transform.position, damage, enemy);
                enemiesHit.Add(enemy);

                // Check if the projectile can still ricochet.
                if (currentRicochets < maxRicochets)
                {
                    currentRicochets++;
                    Ricochet();
                    return; // Exit the function after a successful hit.
                }
                else
                {
                    DestroyProjectile();
                    return;
                }
            }
        }
    }

    private void Ricochet()
    {
        if (maxRicochets <= 0)
        {
            DestroyProjectile();
            return;
        }

        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, 5f);
        float closestDistance = float.MaxValue;
        IDamageable closestEnemy = null;
        Transform closestEnemyTransform = null;

        foreach (Collider2D c in potentialTargets)
        {
            IDamageable potentialEnemy = c.GetComponent<IDamageable>();
            if (potentialEnemy != null && !CheckRepeatHit(potentialEnemy))
            {
                float currentDistance = Vector2.Distance(transform.position, c.transform.position);
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    closestEnemy = potentialEnemy;
                    closestEnemyTransform = c.transform;
                }
            }
        }

        if (closestEnemy != null && closestEnemyTransform != null)
        {
            Vector2 directionToEnemy = (closestEnemyTransform.position - transform.position).normalized;
            SetDirection(directionToEnemy.x, directionToEnemy.y);
            maxRicochets--; // Decrement maxRicochets
        }
        else
        {
            DestroyProjectile();
        }
    }


    public override void SetStats(WeaponBase weaponBase)
    {
        base.SetStats(weaponBase);
        maxRicochets = weaponBase.weaponData.stats.maxRicochets; // Set maxRicochets equal to numberOfHits
    }



    private void OnEnable()
    {
        base.OnEnable(); // Make sure to call base methods if they do important work
        currentRicochets = 0; // Reset ricochet count when the object is reused.
    }

   
}
