using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Ground Slam")]
public class GroundSlamAttack : SpecialAttack
{
    public PoolObjectData groundHoleEffectData;
    public float effectRadius = 5f;
    public float slamDelay = 2f;

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        Transform bossTransform = executor.transform;
        Transform playerTransform = executor.GetPlayerTransform();

        if (bossTransform == null || playerTransform == null)
        {
            UnityEngine.Debug.LogError("Transforms not set!");
            return;
        }

        Vector3 directionToPlayer = (playerTransform.position - bossTransform.position).normalized;
        Vector3 groundHolePosition = bossTransform.position + directionToPlayer * effectRadius;
        groundHolePosition.z = 1f;

        // Create the ground hole instance
        GameObject groundHoleInstance = poolManager.GetObject(groundHoleEffectData);
        groundHoleInstance.transform.position = groundHolePosition;
        groundHoleInstance.transform.rotation = Quaternion.identity;

        GroundSlamDamage groundSlamDamage = groundHoleInstance.GetComponent<GroundSlamDamage>();
        if (groundSlamDamage != null)
        {
            groundSlamDamage.damage = this.damage;
        }

        executor.GetMonoBehaviour().StartCoroutine(ReturnToPoolAfterDelay(groundHoleInstance));
    }

    private IEnumerator ReturnToPoolAfterDelay(GameObject groundHoleInstance)
    {
        yield return new WaitForSeconds(slamDelay);
        groundHoleInstance.GetComponent<PoolMember>()?.ReturnToPool();
    }
}