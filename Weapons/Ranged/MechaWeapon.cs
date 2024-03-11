using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MechaWeapon : WeaponBase
{
    [SerializeField] private PoolObjectData mechaWeaponPoolData;
    private bool isAttacking = false;
    [SerializeField] private GameObject comboMecha; // Reference to the ComboMecha child GameObject

    private void Awake()
    {

        poolManager = FindObjectOfType<PoolManager>(); // Or however you're supposed to get the PoolManager instance
    }

    public override void Attack()
    {
        if (!isAttacking)
        {
            StartCoroutine(FireMechaWeapon());
        }
    }
    // Enables the weapon, allowing it to attack.
    public void EnableWeapon()
    {
        if (comboMecha != null)
        {
            comboMecha.SetActive(true);
        }
        else
        {
            UnityEngine.Debug.LogError("comboMecha reference is null in EnableWeapon");
        }
    }

    // Disables the weapon, preventing it from attacking.
    public void DisableWeapon()
    {
        if (comboMecha != null)
        {
            comboMecha.SetActive(false);
        }
        else
        {
            UnityEngine.Debug.LogError("comboMecha reference is null in DisableWeapon");
        }
    }
    private IEnumerator FireMechaWeapon()
    {
        isAttacking = true;

        // Logic to fire the mecha's weapon. If it's similar to throwing a knife,
        // you might create projectiles from a pool and set them active, etc.
        GameObject projectile = SpawnProjectile(mechaWeaponPoolData, transform.position);
        projectile.SetActive(true);

        yield return new WaitForSeconds(weaponData.stats.timeToAttack);
        isAttacking = false;
    }

    public override GameObject SpawnProjectile(PoolObjectData poolObjectData, Vector3 position)
    {
        GameObject projectileGO = poolManager.GetObject(poolObjectData);
        projectileGO.transform.position = position;

        Projectile projectile = projectileGO.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.SetDirection(vectorOfAttack.x, vectorOfAttack.y);
            projectile.SetStats(this);
        }

        return projectileGO;
    }
}