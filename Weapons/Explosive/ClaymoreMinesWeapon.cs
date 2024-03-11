using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClaymoreMinesWeapon : WeaponBase
{
    public PoolObjectData minePoolData; // Assign this in the inspector
    public float mineSpawnRadius = 1.0f; // Radius around the player to spawn mines
    public float shrapnelRange = 10f; // Distance the shrapnel will travel

    private List<GameObject> activeMines = new List<GameObject>();

    private void Awake()
    {
        // Initialize the list of active mines with the maximum number allowed
        activeMines = new List<GameObject>(weaponData.stats.maxNumberOfSummonedInstances);
    }
    private void Update()
    {
        // Check if new mines can be spawned
        if (activeMines.Count < weaponData.stats.maxNumberOfSummonedInstances)
        {
            if (weaponData.stats.timeToAttack <= 0f)
            {
                Attack();
                weaponData.stats.timeToAttack = weaponData.baseStats.timeToAttack; // Reset attack timer
            }
            else
            {
                weaponData.stats.timeToAttack -= Time.deltaTime;
            }
        }

        // Remove null references from the list (mines that have exploded)
        activeMines.RemoveAll(item => item == null);
    }
    public override void Attack()
    {
        Vector2 spawnPosition = CalculateSpawnPosition();
        if (spawnPosition != Vector2.zero)
        {
            GameObject mine = SpawnProjectile(minePoolData, spawnPosition);
            mine.GetComponent<ClaymoreMine>().SetWeapon(this);
            activeMines.Add(mine);
            mine.GetComponent<ClaymoreMine>().OnExplode += RemoveMineFromActiveList;
        }
    }

    private Vector2 CalculateSpawnPosition()
    {
        Vector2 mousePosition = CalculateMouseDirection() + (Vector2)transform.position;
        Vector2 spawnPosition;
        int attemptCount = 0;

        do
        {
            spawnPosition = mousePosition + UnityEngine.Random.insideUnitCircle * mineSpawnRadius;
            attemptCount++;
        }
        while (IsPositionOccupied(spawnPosition) && attemptCount < 10);

        return attemptCount < 10 ? spawnPosition : Vector2.zero;
    }

    private bool IsPositionOccupied(Vector2 position)
    {
        // Check for obstacles or other mines
        foreach (GameObject mine in activeMines)
        {
            if (Vector2.Distance(mine.transform.position, position) <= mineSpawnRadius)
                return true;
        }

        // You can also check for obstacles using Physics2D.OverlapCircle
        return Physics2D.OverlapCircle(position, mineSpawnRadius);
    }

    private void RemoveMineFromActiveList(GameObject mine)
    {
        activeMines.Remove(mine);
    }
}