using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonerRoboGoblinSpawner : SummonerSpawnerBase
{
    protected override void Start()
    {
        base.Start();
        StartCoroutine(SpawnAndRespawnRoutine());
    }

    public override void Attack()
    {
        // The Attack logic is handled by the base class and SpawnAndRespawnRoutine.
    }

    protected override void InitializeSummon(GameObject summonInstance)
    {
        RobogoblinSummon robogoblinSummon = summonInstance.GetComponent<RobogoblinSummon>();
        if (robogoblinSummon != null)
        {
            robogoblinSummon.weaponBaseReference = this;
        }
        else
        {
            Debug.LogError("[SummonerRoboGoblinSpawner] RobogoblinSummon component not found on summonInstance!");
        }

        SummonComboBuff comboBuff = summonInstance.GetComponent<SummonComboBuff>();
        if (comboBuff != null)
        {
            comboBuff.SetWeaponBaseReference(this);
        }
    }

    private IEnumerator SpawnAndRespawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(weaponData.stats.timeToAttack);  // Check every second (can be adjusted)

            // Respawn logic
            if (summonInstances.Count < weaponData.stats.maxNumberOfSummonedInstances)
            {
                Vector2 spawnPosition = CalculateSpawnPositionAroundPlayer();
                if (spawnPosition != Vector2.zero)
                {
                    GameObject summonInstance = Instantiate(summonPrefab, spawnPosition, Quaternion.identity);
                    if (summonInstance != null)
                    {
                        InitializeSummon(summonInstance);
                        summonInstances.Add(summonInstance);
                        summonInstance.transform.SetParent(GetOrCreatePlayerSummonablesTransform(), true);
                    }
                }
            }

            // Remove null references from the list (clean up for destroyed summons)
            summonInstances.RemoveAll(item => item == null);
        }
    }
}