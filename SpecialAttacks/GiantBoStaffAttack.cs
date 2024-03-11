using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/GiantBoStaffAttack")]
public class GiantBoStaffAttack : SpecialAttack
{
    public PoolObjectData boStaffObjectData;
    public float boStaffLength; // Length of the Bo Staff
    public float offsetDistance;  // Distance in front of the boss to start spawning the Bo Staff.

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        if (poolManager == null)
        {
            Debug.LogError("PoolManager is not set!");
            return;
        }

        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        Vector3 directionToPlayer = (playerTransform.position - executor.transform.position).normalized;
        directionToPlayer.z = 0;  // Ensure z-coordinate remains consistent.

        // Calculate the starting position of the Bo Staff
        Vector3 startPosition = executor.transform.position + (directionToPlayer * offsetDistance);
        Vector3 endPosition = startPosition + directionToPlayer * boStaffLength;

        // Spawn the Bo Staff
        SpawnBoStaff(poolManager, startPosition, endPosition);
    }

    private void SpawnBoStaff(PoolManager poolManager, Vector3 startPosition, Vector3 endPosition)
    {
        GameObject boStaff = poolManager.GetObject(boStaffObjectData);
        if (boStaff == null)
        {
            Debug.LogError("Bo Staff object not available in pool!");
            return;
        }

        // Position and stretch the Bo Staff between the start and end positions
        Vector3 midPoint = (startPosition + endPosition) / 2;
        boStaff.transform.position = midPoint;
        boStaff.transform.right = (endPosition - startPosition).normalized; // Orient the staff
        boStaff.transform.localScale = new Vector3(boStaffLength, boStaff.transform.localScale.y, boStaff.transform.localScale.z);

        boStaff.SetActive(true);
        // Optionally, apply effects or set duration for the Bo Staff here
    }
}