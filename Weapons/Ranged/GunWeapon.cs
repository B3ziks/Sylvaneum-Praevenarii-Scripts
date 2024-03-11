using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunWeapon : WeaponBase
{
    [SerializeField] private PoolObjectData bulletPoolData;
    private bool isAttacking = false;

    public override void Attack()
    {
        if (!isAttacking)
        {
            StartCoroutine(ShootGun());
        }
    }

    private bool isFirstKnife = true;

    private IEnumerator ShootGun()
    {
        isAttacking = true;

        if (isFirstKnife)
        {
            // If it's the first knife, set it to inactive
            GameObject firstBullet = SpawnProjectile(bulletPoolData, transform.position);
            firstBullet.SetActive(false);
            isFirstKnife = false;
        }
        else
        {
            // For subsequent knives, set them to active
            GameObject bullet = SpawnProjectile(bulletPoolData, transform.position);
            bullet.SetActive(true);
        }

        yield return new WaitForSeconds(weaponData.stats.timeToAttack);
        isAttacking = false;
    }
}
