using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/ShamanTotemsAttack")]
public class ShamanTotemsAttack : SpecialAttack
{
    public PoolObjectData shamanTotemPoolData;
    public int numberOfTotems = 3;
    public float spawnDistance = 3f;
    public float totemLifetime = 10f;

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        for (int i = 0; i < numberOfTotems; i++)
        {
            Vector3 spawnPosition = CalculateSpawnPositionAroundPlayer(executor.GetPlayerTransform());
            GameObject totem = poolManager.GetObject(shamanTotemPoolData);
            if (totem != null)
            {
                totem.transform.position = spawnPosition;
                totem.SetActive(true);

                ShamanTotem shamanTotem = totem.GetComponent<ShamanTotem>();
                if (shamanTotem != null)
                {
                    shamanTotem.Initialize(executor.GetPlayerTransform(), totemLifetime, poolManager, shamanTotemPoolData);
                }
                else
                {
                    Debug.LogError("ShamanTotem component not found on the spawned prefab!");
                }
            }
        }
    }

    private Vector3 CalculateSpawnPositionAroundPlayer(Transform playerTransform)
    {
        float angle = UnityEngine.Random.Range(0, 360) * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * spawnDistance;
        return playerTransform.position + offset;
    }
}