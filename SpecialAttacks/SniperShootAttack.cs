using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Sniper Shoot Attack")]
public class SniperShootAttack : SpecialAttack
{
    public PoolObjectData laserIndicatorData;
    public float laserIndicatorThickness = 0.05f; // Adjust as needed for your visual preference
    public float laserIndicatorDuration = 2.0f; // How long the laser indicator will show before firing
    public PoolObjectData sniperProjectileData;

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        if (poolManager == null)
        {
            UnityEngine.Debug.LogError("PoolManager is not set!");
            return;
        }

        Transform playerTransform = executor.GetPlayerTransform();

        if (playerTransform == null)
        {
            UnityEngine.Debug.LogError("Player not found!");
            return;
        }

        Vector3 directionToPlayer = (playerTransform.position - executor.transform.position).normalized;

        float distanceToPlayer = Vector3.Distance(executor.transform.position, playerTransform.position);
        Vector3 midPoint = (playerTransform.position + executor.transform.position) / 2;
        Vector3 scale = new Vector3(distanceToPlayer, laserIndicatorThickness, 1);

        GameObject laserIndicator = poolManager.GetObject(laserIndicatorData);
        if (laserIndicator == null)
        {
            UnityEngine.Debug.LogError("Laser indicator object not available in pool!");
            return;
        }

        laserIndicator.transform.position = midPoint;
        laserIndicator.transform.localScale = scale;
        laserIndicator.transform.right = directionToPlayer;
        laserIndicator.SetActive(true);

        MonoBehaviour mono = executor.GetMonoBehaviour();
        if (mono != null)
        {
            mono.StartCoroutine(ShootAfterDelay(laserIndicator, executor, poolManager));
        }
        else
        {
            UnityEngine.Debug.LogError("MonoBehaviour from executor is null!");
        }
    }

    private IEnumerator ShootAfterDelay(GameObject laserIndicator, ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        float elapsedTime = 0f;

        Transform playerTransform = executor.GetPlayerTransform();
        if (playerTransform == null)
        {
            UnityEngine.Debug.LogError("Player not found!");
            yield break;
        }

        while (elapsedTime < laserIndicatorDuration)
        {
            Vector3 directionToPlayer = (playerTransform.position - executor.transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(executor.transform.position, playerTransform.position);
            Vector3 midPoint = (playerTransform.position + executor.transform.position) /2;
            Vector3 scale = new Vector3(distanceToPlayer, laserIndicatorThickness, 1);

            laserIndicator.transform.position = midPoint;
            laserIndicator.transform.localScale = scale;
            laserIndicator.transform.right = directionToPlayer;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        laserIndicator.SetActive(false);

        GameObject sniperProjectile = poolManager.GetObject(sniperProjectileData);
        if (sniperProjectile == null)
        {
            UnityEngine.Debug.LogError("Sniper projectile object not available in pool!");
            yield break;
        }

        SniperProjectile sniperProjComponent = sniperProjectile.GetComponent<SniperProjectile>();
        if (sniperProjComponent != null)
        {
            // Initialize damage and shootingEnemyData based on the executor's type
            if (executor is EliteEnemyController eliteEnemy)
            {
                if (eliteEnemy.enemyData != null && eliteEnemy.enemyData.stats != null)
                {
                    sniperProjComponent.damage = eliteEnemy.enemyData.stats.damage;
                    sniperProjComponent.shootingEnemyData = eliteEnemy.enemyData;
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
                    sniperProjComponent.damage = boss.enemyData.stats.damage;
                    sniperProjComponent.shootingEnemyData = boss.enemyData;
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

        sniperProjectile.transform.position = executor.transform.position;
        sniperProjectile.SetActive(true);
    }
}
