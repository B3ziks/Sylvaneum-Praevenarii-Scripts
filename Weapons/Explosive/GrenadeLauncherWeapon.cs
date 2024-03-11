using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncherWeapon : WeaponBase
{
    public PoolObjectData grenadeData; // Assign this in the inspector
    public int numberOfProjectiles = 5; // Number of grenades to fire
    public float spreadAngle = 15f; // Spread angle in degrees

    public override void Attack()
    {
        StartCoroutine(FireGrenadeVolley());
    }

    private IEnumerator FireGrenadeVolley()
    {
        Vector2 mouseDirection = CalculateMouseDirection();

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            // Calculate a random deviation for each projectile
            float deviation = UnityEngine.Random.Range(-spreadAngle, spreadAngle);
            Vector2 deviationVector = Quaternion.Euler(0, 0, deviation) * mouseDirection;

            // Spawn the projectile and apply the deviation to its direction
            GameObject projectileGO = SpawnProjectile(grenadeData, transform.position);
            Projectile projectile = projectileGO.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.SetDirection(deviationVector.x, deviationVector.y);
            }

            // Optional delay between each shot
            yield return new WaitForSeconds(0.25f);
        }
    }

    // This method is inherited from WeaponBase and uses the WeaponData to spawn the projectile
    public override GameObject SpawnProjectile(PoolObjectData poolObjectData, Vector3 position)
    {
        // Call the base method to handle the instantiation and setup
        return base.SpawnProjectile(poolObjectData, position);
    }
}