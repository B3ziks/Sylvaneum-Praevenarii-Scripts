using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperProjectile : EnemyProjectile
{
    protected override void Update()
    {
        base.Update();

        // SniperProjectile might have additional behavior or logic in Update, add here if needed
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // We can add specialized behavior for the sniper projectile when it hits something.
        // For now, I will just call the base (EnemyProjectile's) behavior.
        base.OnTriggerEnter2D(other);

        // For example, if we want the sniper's projectiles to pierce through obstacles without deactivating:
        if (other.CompareTag("Obstacle"))
        {
            // Just comment out or remove the DeactivateProjectile line from the base class
        }
    }

    // Add any additional methods or logic specific to the sniper's projectile here.
}