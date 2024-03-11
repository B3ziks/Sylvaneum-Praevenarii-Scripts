using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingKnifeWeapon : WeaponBase
{
    [SerializeField] private PoolObjectData knifePoolData;
    private bool isAttacking = false;

    public override void Attack()
    {
        if (!isAttacking)
        {
            StartCoroutine(ThrowKnife());
        }
    }

    private bool isFirstKnife = true;

    private IEnumerator ThrowKnife()
    {
        isAttacking = true;

        if (isFirstKnife)
        {
            // If it's the first knife, set it to inactive
            GameObject firstKnife = SpawnProjectile(knifePoolData, transform.position);
            firstKnife.SetActive(false);
            isFirstKnife = false;
        }
        else
        {
            // For subsequent knives, set them to active
            GameObject knife = SpawnProjectile(knifePoolData, transform.position);
            knife.SetActive(true);
        }

        yield return new WaitForSeconds(weaponData.stats.timeToAttack);
        isAttacking = false;
    }
}