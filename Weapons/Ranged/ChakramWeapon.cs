using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChakramWeapon : WeaponBase
{
    [SerializeField] private PoolObjectData chakramPoolData;
    private bool isAttacking = false;

    public override void Attack()
    {
        if (!isAttacking)
        {
            StartCoroutine(ThrowShuriken());
        }
    }

    private bool isFirstChakram = true;

    private IEnumerator ThrowShuriken()
    {
        isAttacking = true;

        if (isFirstChakram)
        {
            // If it's the first knife, set it to inactive
            GameObject firstChakram = SpawnProjectile(chakramPoolData, transform.position);
            firstChakram.SetActive(false);
            isFirstChakram = false;
        }
        else
        {
            // For subsequent knives, set them to active
            GameObject chakram = SpawnProjectile(chakramPoolData, transform.position);
            chakram.SetActive(true);
        }

        yield return new WaitForSeconds(weaponData.stats.timeToAttack);
        isAttacking = false;
    }
}
