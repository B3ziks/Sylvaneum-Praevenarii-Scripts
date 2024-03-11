using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Charge Attack")]
public class ChargeAttack : SpecialAttack
{
    public float chargeSpeed = 20f;
    public float chargeDuration = 1.5f;
    public int chargeDamage = 10;

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
            mono.StartCoroutine(ExecuteCharge(executor.transform, playerTransform.position));
        }
        else
        {
            UnityEngine.Debug.LogError("BossController cannot be casted to MonoBehaviour!");
        }
    }

    private IEnumerator ExecuteCharge(Transform bossTransform, Vector3 targetPosition)
    {
        Vector3 initialPosition = bossTransform.position;
        float startTime = Time.time;

        while (Time.time < startTime + chargeDuration)
        {
            float step = chargeSpeed * Time.deltaTime;
            bossTransform.position = Vector3.MoveTowards(bossTransform.position, targetPosition, step);
            yield return null; // wait for the next frame
        }

        // Optionally, move boss back to initial position or implement additional logic.
    }
}