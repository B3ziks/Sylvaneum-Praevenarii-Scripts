using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Slime Charge Attack")]
public class SlimeChargeAttack : SpecialAttack
{
    public float chargeSpeed = 20f;
    public float chargeDuration = 1.5f;
    public int chargeDamage = 10;
    public PoolObjectData slimePoolData; // Reference to the slime pool data
    public int poolSegments = 1; // Number of slime pools to leave behind
    public float segmentSpacing = 2f; // Space between slime pool segments

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        Transform playerTransform = executor.GetPlayerTransform();

        if (playerTransform == null)
        {
            UnityEngine.Debug.LogError("Player transform not found!");
            return;
        }

        MonoBehaviour mono = executor as MonoBehaviour;
        if (mono != null)
        {
            mono.StartCoroutine(ExecuteCharge(executor, playerTransform.position, poolManager));
        }
        else
        {
            UnityEngine.Debug.LogError("Executor cannot be cast to MonoBehaviour!");
        }
    }

    private IEnumerator ExecuteCharge(ISpecialAttackExecutor executor, Vector3 targetPosition, PoolManager poolManager)
    {
        Transform bossTransform = executor.GetMonoBehaviour().transform;
        Vector3 initialPosition = bossTransform.position;
        float startTime = Time.time;
        Vector3 lastSegmentPosition = initialPosition; // Keep track of the last segment's position

        while (Time.time < startTime + chargeDuration)
        {
            float step = chargeSpeed * Time.deltaTime;
            bossTransform.position = Vector3.MoveTowards(bossTransform.position, targetPosition, step);

            // Calculate the distance since the last slime pool segment
            if (Vector3.Distance(bossTransform.position, lastSegmentPosition) >= segmentSpacing)
            {
                // Create a slime pool segment at the current position
                CreateSlimePoolSegment(bossTransform.position, poolManager);
                lastSegmentPosition = bossTransform.position; // Update the last segment position
            }

            yield return null; // wait for the next frame
        }

        // Optionally, move boss back to initial position or implement additional logic.
    }

    private void CreateSlimePoolSegment(Vector3 position, PoolManager poolManager)
    {
        if (slimePoolData != null && poolManager != null)
        {
            GameObject slimePoolSegment = poolManager.GetObject(slimePoolData);
            if (slimePoolSegment != null)
            {
                slimePoolSegment.transform.position = position;
                slimePoolSegment.SetActive(true);

                SlimePoolBehavior poolBehavior = slimePoolSegment.GetComponent<SlimePoolBehavior>();
                if (poolBehavior != null)
                {
                    poolBehavior.ConfigurePool(chargeDamage);
                }
            }
        }
        else
        {
            UnityEngine.Debug.LogError("SlimePoolData not set or PoolManager not found!");
        }
    }
}