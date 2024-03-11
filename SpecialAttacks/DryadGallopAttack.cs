using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/DryadGallopAttack")]
public class DryadGallopAttack : SpecialAttack
{
    public float recoilForce = 2000f; // Force applied for recoil
    public float recoilDuration = 0.1f; // Duration of each recoil

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

        // Start coroutine for the random directional jumps
        executor.GetMonoBehaviour().StartCoroutine(RandomDirectionalJumps(bossRigidbody));
    }

    private IEnumerator RandomDirectionalJumps(Rigidbody2D bossRigidbody)
    {
        // Perform three random jumps
        for (int i = 0; i < 3; i++)
        {
            Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
            yield return Recoil(bossRigidbody, randomDirection);
        }
    }

    private IEnumerator Recoil(Rigidbody2D bossRigidbody, Vector2 recoilDirection)
    {
        float endTime = Time.time + recoilDuration;
        while (Time.time < endTime)
        {
            bossRigidbody.AddForce(recoilDirection * (recoilForce / recoilDuration) * Time.fixedDeltaTime, ForceMode2D.Force);
            yield return new WaitForFixedUpdate();
        }

        // Reset boss velocity after each jump
        bossRigidbody.velocity = Vector2.zero;
    }
}