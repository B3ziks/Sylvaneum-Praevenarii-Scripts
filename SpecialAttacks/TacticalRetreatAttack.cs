using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/TacticalRetreatAttack")]
public class TacticalRetreatAttack : SpecialAttack
{
    public PoolObjectData spikePrefab; // Assign this in the inspector
    public float recoilForce = 3000f; // Force applied for recoil
    public float recoilDuration = 0.1f; // Duration of the recoil

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        if (executor == null)
        {
            Debug.LogError("Executor is not set!");
            return;
        }

        Rigidbody2D bossRigidbody = executor.GetMonoBehaviour().GetComponent<Rigidbody2D>();
        if (bossRigidbody == null)
        {
            Debug.LogError("Rigidbody2D not found on the boss!");
            return;
        }

        // Calculate recoil direction (opposite to the boss's current facing direction)
        Vector2 recoilDirection = -executor.GetMonoBehaviour().transform.right.normalized;

        // Store boss's current position for spike drop
        Vector3 currentPosition = executor.GetMonoBehaviour().transform.position;

        // Start recoil coroutine
        executor.GetMonoBehaviour().StartCoroutine(RecoilAndDropSpikes(bossRigidbody, recoilDirection, currentPosition, poolManager));
    }

    private IEnumerator RecoilAndDropSpikes(Rigidbody2D bossRigidbody, Vector2 recoilDirection, Vector3 spikeDropPosition, PoolManager poolManager)
    {
        // Apply recoil force
        float endTime = Time.time + recoilDuration;
        while (Time.time < endTime)
        {
            bossRigidbody.AddForce(recoilDirection * (recoilForce / recoilDuration) * Time.fixedDeltaTime, ForceMode2D.Force);
            yield return new WaitForFixedUpdate();
        }

        // Drop spikes at the initial position
        DropSpikes(spikeDropPosition, poolManager);

        // Reset boss velocity after recoil
        bossRigidbody.velocity = Vector2.zero;
    }

    private void DropSpikes(Vector3 position, PoolManager poolManager)
    {
        GameObject spike = poolManager.GetObject(spikePrefab);
        if (spike != null)
        {
            spike.transform.position = position;
            spike.SetActive(true);
        }
    }
}