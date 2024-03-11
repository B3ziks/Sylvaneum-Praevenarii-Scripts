using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Multishoot Attack")]
public class MultishootAttack : SpecialAttack
{
    public PoolObjectData projectilePoolData;
    public float projectileSpeed = 5f;
    private const float MULTISHOOT_ANGLE_OFFSET = 15f;

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        UnityEngine.Debug.Log("ExecuteAttack called");

        if (poolManager == null)
        {
            UnityEngine.Debug.LogError("PoolManager is not set!");
            return;
        }

        Vector3 spawnPosition = executor.transform.position;
        Vector3 centerDirection = (GameObject.FindGameObjectWithTag("Player").transform.position - spawnPosition).normalized;

        Vector3 leftDirection = Quaternion.Euler(0, 0, MULTISHOOT_ANGLE_OFFSET) * centerDirection;
        Vector3 rightDirection = Quaternion.Euler(0, 0, -MULTISHOOT_ANGLE_OFFSET) * centerDirection;

        PrepareAndActivateProjectile(centerDirection, spawnPosition, poolManager, executor);
        PrepareAndActivateProjectile(leftDirection, spawnPosition, poolManager, executor);
        PrepareAndActivateProjectile(rightDirection, spawnPosition, poolManager, executor);
    }

    private void PrepareAndActivateProjectile(Vector3 direction, Vector3 spawnPosition, PoolManager poolManager, ISpecialAttackExecutor executor)
    {
        GameObject projectile = poolManager.GetObject(projectilePoolData);
        if (projectile != null)
        {
            projectile.transform.position = spawnPosition;
            EnemyProjectileMultishoot multishootProjectile = projectile.GetComponent<EnemyProjectileMultishoot>();

            if (multishootProjectile != null)
            {
                multishootProjectile.Initialize(direction, projectileSpeed);

                // Initialize ShootingEnemyData and damage based on the type of the executor
                if (executor is EliteEnemyController eliteEnemy)
                {
                    if (eliteEnemy.enemyData != null && eliteEnemy.enemyData.stats != null)
                    {
                        multishootProjectile.damage = eliteEnemy.enemyData.stats.damage;
                        multishootProjectile.shootingEnemyData = eliteEnemy.enemyData;
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarning("EliteEnemy's enemyData or stats is null!");
                    }
                }
                else if (executor is BossController boss)
                {
                    if (boss.enemyData != null && boss.enemyData.stats != null)
                    {
                        multishootProjectile.damage = boss.enemyData.stats.damage;
                        multishootProjectile.shootingEnemyData = boss.enemyData;
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarning("Boss's enemyData or stats is null!");
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError("Executor is not of the expected type.");
                }
            }

            projectile.SetActive(true);
        }
        else
        {
            UnityEngine.Debug.LogError("Prefab is null. Make sure it's assigned in projectilePoolData.");
        }
    }
}