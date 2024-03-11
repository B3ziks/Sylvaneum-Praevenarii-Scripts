using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Spider Legs Attack")]
public class SpiderLegsAttack : SpecialAttack
{
    [SerializeField] private PoolObjectData leftLegData;
    [SerializeField] private PoolObjectData rightLegData;
    public float attackDuration = 5f;
    public float timeBetweenAttacks = 0.5f;
    public float attackDelay = 0.5f;  // Delay before the leg attacks the targeted position

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        Transform playerTransform = executor.GetPlayerTransform();
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not set!");
            return;
        }

        executor.GetMonoBehaviour().StartCoroutine(SpiderLegsSequence(executor, poolManager, playerTransform));
    }

    private IEnumerator SpiderLegsSequence(ISpecialAttackExecutor executor, PoolManager poolManager, Transform playerTransform)
    {
        float timer = 0f;
        bool isLeftLeg = true;

        while (timer < attackDuration)
        {
            Vector3 targetPosition = playerTransform.position;
            executor.GetMonoBehaviour().StartCoroutine(SpawnLegAfterDelay(poolManager, isLeftLeg ? leftLegData : rightLegData, targetPosition, attackDelay));
            isLeftLeg = !isLeftLeg; // Alternate legs
            timer += timeBetweenAttacks;
            yield return new WaitForSeconds(timeBetweenAttacks);
        }
    }

    private IEnumerator SpawnLegAfterDelay(PoolManager poolManager, PoolObjectData legData, Vector3 targetPosition, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject leg = poolManager.GetObject(legData);
        if (leg != null)
        {
            leg.transform.position = targetPosition;
            leg.SetActive(true);
            // Optionally, add logic to check if the player is still in the targeted position
            // and apply damage or effects accordingly.
        }
    }
}