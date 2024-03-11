using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Slime Ball Attack")]
public class SlimeBallAttack : SpecialAttack
{
    public PoolObjectData slimeBallData;
    public float attackDelay = 1f;
    public float projectileSpeed = 5f;
    public int numberOfProjectiles = 3;
    public float arcHeight = 2f; // Height of the arc

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        MonoBehaviour monoBehaviour = executor.GetMonoBehaviour();
        if (monoBehaviour != null)
        {
            monoBehaviour.StartCoroutine(AttackCoroutine(executor, poolManager));
        }
        else
        {
            Debug.LogError("Executor MonoBehaviour is null!");
        }
    }

    private IEnumerator AttackCoroutine(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        Enemy enemyComponent = executor.GetMonoBehaviour().GetComponent<Enemy>();
        if (enemyComponent == null)
        {
            Debug.LogError("Enemy component not found on executor!");
            yield break;
        }

        float originalMoveSpeed = enemyComponent.stats.moveSpeed;
        enemyComponent.stats.moveSpeed = 0;
        yield return new WaitForSeconds(attackDelay);

        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform != null)
        {
            for (int i = 0; i < numberOfProjectiles; i++)
            {
                GameObject slimeBall = poolManager.GetObject(slimeBallData);
                if (slimeBall != null)
                {
                    slimeBall.transform.position = executor.transform.position;
                    slimeBall.SetActive(true);

                    Vector3 targetPosition = playerTransform.position + UnityEngine.Random.insideUnitSphere * 2;
                    targetPosition.z = slimeBall.transform.position.z;

                    // Adjust the target position for the artillery effect
                    MonoBehaviour monoBehaviour = executor.GetMonoBehaviour();
                    if (monoBehaviour != null)
                    {
                        monoBehaviour.StartCoroutine(ArtilleryProjectileMotion(slimeBall, targetPosition, arcHeight));
                    }
                    else
                    {
                        Debug.LogError("Executor MonoBehaviour is null!");
                    }
                }
            }
        }
        enemyComponent.stats.moveSpeed = originalMoveSpeed;
    }

    private IEnumerator ArtilleryProjectileMotion(GameObject projectile, Vector3 target, float arcHeight)
    {
        Vector3 startPosition = projectile.transform.position;
        float distance = Vector3.Distance(startPosition, target);
        float increment = 0;

        while (increment < 1)
        {
            increment += Time.deltaTime * projectileSpeed / distance;
            Vector3 currentPosition = Vector3.Lerp(startPosition, target, increment);
            currentPosition.y += arcHeight * Mathf.Sin(Mathf.PI * increment);
            projectile.transform.position = currentPosition;

            yield return null;
        }
    }
}