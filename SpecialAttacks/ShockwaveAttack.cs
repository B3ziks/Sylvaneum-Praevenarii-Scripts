using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Shockwave Attack")]
public class ShockwaveAttack : SpecialAttack
{
    public PoolObjectData shockwaveObjectData;
    public float distanceBetweenShockwaves;
    public int numberOfShockwaves;
    public float offsetDistance;  // Distance in front of the boss to start spawning shockwaves.
    public float shockwaveLifetime = 4f;  // Lifetime of the shockwave before it gets returned to the pool.

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        if (poolManager == null)
        {
            UnityEngine.Debug.LogError("PoolManager is not set!");
            return;
        }

        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform == null)
        {
            UnityEngine.Debug.LogError("Player not found!");
            return;
        }

        Vector3 directionToPlayer = (playerTransform.position - executor.transform.position).normalized;
        directionToPlayer.z = 0;  // Ensure z-coordinate remains consistent.

        // Modifying the starting position to be offsetDistance units in front of the boss.
        Vector3 startPosition = executor.transform.position + (directionToPlayer * offsetDistance);

        for (int i = 0; i < numberOfShockwaves; i++)
        {
            Vector3 position = startPosition + i * directionToPlayer * distanceBetweenShockwaves;
            position.z = executor.transform.position.z;  // Ensure z-coordinate remains consistent.

            GameObject shockwave = poolManager.GetObject(shockwaveObjectData);

            if (shockwave == null)
            {
                UnityEngine.Debug.LogError("Shockwave object not available in pool!");
                return;
            }

            shockwave.transform.position = position;
            shockwave.transform.rotation = Quaternion.identity;
            shockwave.SetActive(true);

            GroundSlamDamage groundSlamDamage = shockwave.GetComponent<GroundSlamDamage>();
            if (groundSlamDamage != null)
            {
                groundSlamDamage.damage = this.damage;
            }

            // Start the coroutine to deactivate the shockwave after a delay using the executor's MonoBehaviour
            executor.GetMonoBehaviour().StartCoroutine(DeactivateAfterDelay(shockwave, shockwaveLifetime));
        }
    }

    private IEnumerator DeactivateAfterDelay(GameObject shockwave, float delay)
    {
        yield return new WaitForSeconds(delay);
        shockwave.GetComponent<PoolMember>()?.ReturnToPool();
    }
}