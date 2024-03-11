using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Entangling Roots Attack")]
public class EntanglingRootsAttack : SpecialAttack
{
    [SerializeField] public PoolObjectData rootsPoolData;
    public float rootsSpawnDelay = 2f; // Delay before spawning the roots
    public float rootsDuration = 5f; // How long the roots will persist

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        Transform playerTransform = executor.GetPlayerTransform();
        if (playerTransform == null)
        {
            UnityEngine.Debug.LogError("Player Transform is not set!");
            return;
        }

        // Delay the spawning of roots
        executor.GetMonoBehaviour().StartCoroutine(SpawnRootsAfterDelay(playerTransform, poolManager, executor));
    }

    private IEnumerator SpawnRootsAfterDelay(Transform playerTransform, PoolManager poolManager, ISpecialAttackExecutor executor)
    {
        yield return new WaitForSeconds(rootsSpawnDelay);

        // Spawn the roots at the player's position
        GameObject rootsInstance = poolManager.GetObject(rootsPoolData);
        if (rootsInstance != null)
        {
            rootsInstance.transform.position = playerTransform.position;
            rootsInstance.transform.rotation = Quaternion.identity;

            // Schedule the roots to be returned to the pool after the duration
            executor.GetMonoBehaviour().StartCoroutine(ReturnRootsToPoolAfterDuration(rootsInstance, rootsDuration));
        }
        else
        {
            UnityEngine.Debug.LogError("Roots object not available in pool!");
        }
    }

    private IEnumerator ReturnRootsToPoolAfterDuration(GameObject rootsInstance, float duration)
    {
        yield return new WaitForSeconds(duration);
        rootsInstance.GetComponent<PoolMember>()?.ReturnToPool();
    }
}