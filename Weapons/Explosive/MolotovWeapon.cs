using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MolotovWeapon : WeaponBase
{
    [SerializeField] private PoolObjectData explosivePoolData;
    private bool isAttacking = false;

    [Header("Lobbed Projectile Settings")]
    public float throwForce = 5f;
    public Vector2 throwDirection = new Vector2(1, 2); // A default upward and forward direction
    public float explosionRadius = 5f; // Adjust based on your game's needs

    public override void Attack()
    {
        if (!isAttacking)
        {
            throwForce = weaponData.stats.projectileSpeed; // Assign the speed from weaponData to throwForce
            StartCoroutine(ThrowExplosive());
        }
    }

    private IEnumerator ThrowExplosive()
    {
        isAttacking = true;

        GameObject explosive = SpawnProjectile(explosivePoolData, transform.position);
        MolotovProjectile explosiveProjectile = explosive.GetComponent<MolotovProjectile>();

        // Here, you can set some properties of the explosive projectile if needed.
        // For example: explosiveProjectile.SetExplosionRadius(explosionRadius);

        Rigidbody2D rb = explosive.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 directionToMouse = CalculateMouseDirection();  // New line
            rb.velocity = directionToMouse * throwForce;           // Updated line
        }
        else
        {
            UnityEngine.Debug.LogError("No Rigidbody2D found on the explosive! Please add one.");
        }

        yield return new WaitForSeconds(weaponData.stats.timeToAttack);
        isAttacking = false;
    }


}