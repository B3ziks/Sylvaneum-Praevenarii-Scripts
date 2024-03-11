using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenWeapon : WeaponBase
{
    [SerializeField] private PoolObjectData shurikenPoolData;
    private bool isAttacking = false;

    public override void Attack()
    {
        if (!isAttacking)
        {
            StartCoroutine(ThrowShuriken());
        }
    }

    private bool isFirstShuriken = true;

    private IEnumerator ThrowShuriken()
    {
        isAttacking = true;

        if (isFirstShuriken)
        {
            // If it's the first knife, set it to inactive
            GameObject firstBullet = SpawnProjectile(shurikenPoolData, transform.position);
            firstBullet.SetActive(false);
            isFirstShuriken = false;
        }
        else
        {
            // For subsequent knives, set them to active
            GameObject bullet = SpawnProjectile(shurikenPoolData, transform.position);
            bullet.SetActive(true);
        }

        yield return new WaitForSeconds(weaponData.stats.timeToAttack);
        isAttacking = false;
    }

}
