using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBombWeapon : WeaponBase
{
    public PoolObjectData fireBombPoolData; // Assign this in the inspector
    public float throwForce = 5f;
    
    private void Awake()
    {
        throwForce = weaponData.stats.projectileSpeed;
    }
    public override void Attack()
    {
        StartCoroutine(ThrowFireBomb());
    }

    private IEnumerator ThrowFireBomb()
    {
        GameObject fireBomb = SpawnProjectile(fireBombPoolData, transform.position);
        Rigidbody2D rb = fireBomb.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            Vector2 directionToMouse = CalculateMouseDirection();
            rb.velocity = directionToMouse * throwForce;
        }
        else
        {
            Debug.LogError("No Rigidbody2D found on the firebomb! Please add one.");
        }

        yield return new WaitForSeconds(weaponData.stats.timeToAttack);
    }
}