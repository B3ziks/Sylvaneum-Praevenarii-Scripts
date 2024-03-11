using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrapnelGrenadeWeapon : WeaponBase
{
    [SerializeField] private PoolObjectData shrapnelExplosivePoolData;
    public float throwForce = 5f;
    public float shrapnelSpreadRadius = 5f; // How far the shrapnels spread
    public int numberOfShrapnels = 4; // Number of shrapnels to spawn

    public override void Attack()
    {
        StartCoroutine(ThrowShrapnelExplosive());
    }

    private IEnumerator ThrowShrapnelExplosive()
    {
        GameObject explosive = SpawnProjectile(shrapnelExplosivePoolData, transform.position);
        Rigidbody2D rb = explosive.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            Vector2 directionToMouse = CalculateMouseDirection();
            rb.velocity = directionToMouse * throwForce;
        }
        else
        {
            Debug.LogError("No Rigidbody2D found on the shrapnel explosive! Please add one.");
        }

        yield return new WaitForSeconds(weaponData.stats.timeToAttack);
    }
}