using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/CrocodileTailAttack")]
public class CrocodileTailAttack : SpecialAttack
{
    [SerializeField] private PoolObjectData tailData; // Data for the crocodile tail
    public float attackDuration = 5f;
    public float timeBetweenAttacks = 0.5f;
    public float attackDelay = 0.5f; // Delay before the tail attacks the targeted position

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        Transform playerTransform = executor.GetPlayerTransform();
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not set!");
            return;
        }

        executor.GetMonoBehaviour().StartCoroutine(CrocodileTailSequence(executor, poolManager, playerTransform));
    }

    private IEnumerator CrocodileTailSequence(ISpecialAttackExecutor executor, PoolManager poolManager, Transform playerTransform)
    {
        float timer = 0f;

        while (timer < attackDuration)
        {
            Vector3 targetPosition = playerTransform.position;
            executor.GetMonoBehaviour().StartCoroutine(SpawnTailAfterDelay(poolManager, tailData, targetPosition, attackDelay));
            timer += timeBetweenAttacks;
            yield return new WaitForSeconds(timeBetweenAttacks);
        }
    }

    private IEnumerator SpawnTailAfterDelay(PoolManager poolManager, PoolObjectData tailData, Vector3 targetPosition, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject tail = poolManager.GetObject(tailData);
        if (tail != null)
        {
            tail.transform.position = targetPosition;
            tail.SetActive(true);
            // Add any additional logic specific to the Crocodile Tail's behavior here.
        }
    }
}