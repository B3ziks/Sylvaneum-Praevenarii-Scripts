using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBallWeapon : WeaponBase
{
    [SerializeField] private PoolObjectData lightningBallPoolData;
    private bool isAttacking = false;

    public override void Attack()
    {
        if (!isAttacking)
        {
            StartCoroutine(ThrowLightningBall());
        }
    }

    private bool isFirstLightningBall = true;

    public float GetAttackTime()
    {
        return weaponData.stats.timeToAttack;
    }
    private IEnumerator ThrowLightningBall()
    {
        isAttacking = true;

        if (isFirstLightningBall)
        {
            // If it's the first knife, set it to inactive
            GameObject firstLightningBall = SpawnProjectile(lightningBallPoolData, transform.position);
            firstLightningBall.SetActive(false);
            isFirstLightningBall = false;
        }
        else
        {
            // For subsequent knives, set them to active
            GameObject lightningBall = SpawnProjectile(lightningBallPoolData, transform.position);
            LightningBallProjectile projectileScript = lightningBall.GetComponent<LightningBallProjectile>();
            projectileScript.SetLightningBallWeapon(this);
        }

        yield return new WaitForSeconds(weaponData.stats.timeToAttack);
        isAttacking = false;
    }
}