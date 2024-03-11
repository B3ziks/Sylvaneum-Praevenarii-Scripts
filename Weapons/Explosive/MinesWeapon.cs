using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MinesWeapon : WeaponBase
{
    public PoolObjectData minePoolData;  // A pool object for the mine prefab
    public float spawnRadius = 1.0f;  // Radius around mouse to spawn mines
    private List<GameObject> activeMines = new List<GameObject>();  // List of currently active mines
    // Instead of using maxMines, we'll now reference the weaponData's weaponStats
    private int MaxMines
    {
        get { return weaponData.stats.maxNumberOfSummonedInstances; }
    }

    public override void Attack()
    {
        if (activeMines.Count >= MaxMines)
            return;

        Vector2 mousePosition = CalculateMouseDirection() + (Vector2)transform.position;

        // Spawn mines near mouse position
        Vector2 spawnPosition;
        int attemptCount = 0;
        do
        {
            spawnPosition = mousePosition + UnityEngine.Random.insideUnitCircle * spawnRadius;
            attemptCount++;
        }
        while (IsPositionOccupied(spawnPosition) && attemptCount < 10);

        if (attemptCount < 10)
        {
            GameObject mine = SpawnProjectile(minePoolData, spawnPosition);
            mine.GetComponent<ExplodingMine>().SetDamage(GetDamage());
            activeMines.Add(mine);
            mine.GetComponent<ExplodingMine>().OnExplode += RemoveMineFromActiveList;
        }
    }

    private bool IsPositionOccupied(Vector2 position)
    {
        // Check for obstacles or other mines
        foreach (GameObject mine in activeMines)
        {
            if (Vector2.Distance(mine.transform.position, position) <= spawnRadius)
                return true;
        }

        // You can also check for obstacles using Physics2D
        // Just make sure that obstacles have a specific layer or tag you can check against
        if (Physics2D.OverlapCircle(position, spawnRadius, LayerMask.GetMask("Obstacle")))
            return true;

        return false;
    }

    private void RemoveMineFromActiveList(GameObject mine)
    {
        activeMines.Remove(mine);
    }
}