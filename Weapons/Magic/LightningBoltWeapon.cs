using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBoltWeapon : WeaponBase
{
    [SerializeField] private PoolObjectData lightningBoltPoolData;
    private bool isAttacking = false;

    public override void Attack()
    {
        if (!isAttacking)
        {
            StartCoroutine(ThrowLightningBolt());
        }
    }

    private bool isFirstKnife = true;

    private IEnumerator ThrowLightningBolt()
    {
        isAttacking = true;

        GameObject lightningBolt;
        if (isFirstKnife)
        {
            lightningBolt = SpawnProjectile(lightningBoltPoolData, transform.position);
            lightningBolt.SetActive(false);
            isFirstKnife = false;
        }
        else
        {
            lightningBolt = SpawnProjectile(lightningBoltPoolData, transform.position);
            lightningBolt.SetActive(true);
        }

        yield return new WaitForSeconds(weaponData.stats.timeToAttack);
        isAttacking = false;
    }

    public override GameObject SpawnProjectile(PoolObjectData poolObjectData, Vector3 position)
    {
        GameObject projectileGO = base.SpawnProjectile(poolObjectData, position);
        LightningBoltProjectile projectile = projectileGO.GetComponent<LightningBoltProjectile>();

            SpriteRenderer spriteRenderer = projectile.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                projectile.SetLineSprite(spriteRenderer.sprite);
            }

            return projectileGO;
        }
    }