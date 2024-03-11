using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBallProjectile : Projectile
{
    [SerializeField]
    private PoolObjectData lightningProjectilePoolData; // The PoolObjectData for the lightning projectile
    private float spawnInterval; // Adjust as needed for how often to spawn lightning projectiles
    private float timeSinceLastSpawn = 0f;
    PoolManager poolManager;
    private LightningBallWeapon lightningBallWeapon;


    private void Start()
    {
        poolManager = FindObjectOfType<PoolManager>();
        spawnInterval = lightningBallWeapon.GetAttackTime() * 0.08f;  // Assuming you want the bolts to spawn at twice the rate of the ball's attack time
    }
    protected override void Update()
    {
        base.Update();
        SpawnLightningProjectiles();
    }

    private void SpawnLightningProjectiles()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnInterval)
        {
            SpawnLightningProjectile(lightningProjectilePoolData);
            timeSinceLastSpawn = 0f;
        }
    }
    public void SetLightningBallWeapon(LightningBallWeapon weapon)
    {
        lightningBallWeapon = weapon;
    }
    private GameObject SpawnLightningProjectile(PoolObjectData poolObjectData)
    {
        // Set a random direction for the lightning projectile
        float randomAngle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float dir_x = Mathf.Cos(randomAngle);
        float dir_y = Mathf.Sin(randomAngle);

        Vector3 direction = new Vector3(dir_x, dir_y, 0);
        Vector3 offsetPosition = transform.position + direction * 0.5f; // Offset can be adjusted as needed

        GameObject lightningProjectileGO = poolManager.GetObject(poolObjectData);
        lightningProjectileGO.transform.position = offsetPosition;

        Projectile lightningProjectile = lightningProjectileGO.GetComponent<Projectile>();
        lightningProjectile.SetDirection(dir_x, dir_y);
        lightningProjectile.SetPartialStats(weapon, 1f / 3f); // Set 1/3 of the stats

      

        return lightningProjectileGO;
    }

}