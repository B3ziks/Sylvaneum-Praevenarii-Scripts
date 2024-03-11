using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Wall Of Flames")]
public class WallOfFlamesAttack : SpecialAttack
{
    public PoolObjectData flameObjectData;
    public float distanceBetweenFlames;
    public int numberOfFlames;
    public float flameBurningDuration;
    public float offsetDistance;  // Distance in front of the boss to start spawning flames.


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

        for (int i = 0; i < numberOfFlames; i++)
        {
            Vector3 position = startPosition + i * directionToPlayer * distanceBetweenFlames;
            position.z = executor.transform.position.z;  // Ensure z-coordinate remains consistent.

            GameObject flame = poolManager.GetObject(flameObjectData);

            if (flame == null)
            {
                UnityEngine.Debug.LogError("Flame object not available in pool!");
                return;
            }

            flame.transform.position = position;
            flame.transform.rotation = Quaternion.identity;
            flame.SetActive(true);

            FlameOnCharacter flameScript = flame.GetComponent<FlameOnCharacter>();
           
        }
    }
}