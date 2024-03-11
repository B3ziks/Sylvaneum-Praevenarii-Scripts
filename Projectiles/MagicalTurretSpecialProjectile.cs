using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class MagicalTurretSpecialProjectile : TurretProjectile
{
    [SerializeField]
    private PoolObjectData flamePoolData; // Pool data for the flame effect
    private PoolManager poolManager;
    private Vector3 targetPosition;


    protected void Awake()
    {
        poolManager = FindObjectOfType<PoolManager>();
        SetTargetPosition();
    }

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
                LeaveFlameOnEnemy(enemy); // Special effect for the big projectile
                enemiesHit.Add(enemy);
                numOfHits -= 1;
            }
        }

        if (numOfHits <= 0)
        {
            DeactivateProjectile();
        }
    }
    private void SetTargetPosition()
    {
        targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0;
    }
    private void LeaveFlameOnEnemy(IDamageable enemy)
    {
        // Logic to leave flames on the enemy
        GameObject flameEffect = poolManager.GetObject(flamePoolData);
        if (flameEffect)
        {
            flameEffect.transform.position = (enemy as UnityEngine.Component).transform.position; // Cast to Component to access Transform
            flameEffect.transform.rotation = Quaternion.identity;
            FlameOnEnemy flameScript = flameEffect.GetComponent<FlameOnEnemy>();
            if (flameScript != null)
            {
                flameScript.damage = this.damage; // Assuming there's an Initialize method to setup the flame
            }
        }
        else
        {
            Debug.LogError("Failed to get flame effect from pool.");
        }
    }

    // Rest of the MagicalTurretSpecialProjectile implementation
    // ...
}