using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SummonerSpawnerBase : WeaponBase
{
    public GameObject summonPrefab;
    protected List<GameObject> summonInstances = new List<GameObject>();

    [SerializeField] private LayerMask obstacleLayer;
    protected const int MAX_SPAWN_ATTEMPTS = 10;
    protected bool isFirstSummonAttempt = true;
    public float summonSpawnDistance = 2f;
    protected const float SAFE_DISTANCE_BETWEEN_SUMMONS = 2f; // Define a safe distance between summons

    protected virtual void Start()
    {
        StartCoroutine(DelayFirstSummon());
    }

    public override void Attack()
    {
        if (isFirstSummonAttempt)
        {
            return; // Skipping the first summon attempt to allow for setup or cooldown
        }

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
                else
                {
                    Debug.LogError("Failed to instantiate summon prefab.");
                }
            }
            else
            {
                Debug.LogWarning("Failed to find a valid spawn position for summon.");
            }
        }
        else
        {
            Debug.Log("Maximum number of summons reached.");
        }
    }

    protected IEnumerator DelayFirstSummon()
    {
        yield return new WaitForSeconds(0.5f);
        isFirstSummonAttempt = false;
    }

    protected abstract void InitializeSummon(GameObject summonInstance);


    protected Vector2 CalculateSpawnPositionAroundPlayer()
    {
        Vector2 spawnPosition = Vector2.zero;
        int attempts = MAX_SPAWN_ATTEMPTS;

        while (attempts > 0)
        {
            float angle = UnityEngine.Random.Range(0, 360) * Mathf.Deg2Rad;
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * summonSpawnDistance;
            spawnPosition = (Vector2)playerMove.transform.position + offset;

            if (!IsPositionOnObstacle(spawnPosition) && !IsPositionOnOtherSummon(spawnPosition))
            {
                return spawnPosition;
            }

            attempts--;
        }

        Debug.LogWarning("Failed to find a valid spawn position for summon!");
        return Vector2.zero;
    }


    protected bool IsPositionOnOtherSummon(Vector2 position)
    {
        foreach (var summon in summonInstances)
        {
            if (summon != null && Vector2.Distance(summon.transform.position, position) < SAFE_DISTANCE_BETWEEN_SUMMONS)
            {
                return true;
            }
        }
        return false;
    }

    protected bool IsPositionOnObstacle(Vector2 position)
    {
        float checkRadius = 0.5f; // Adjust based on your game's scale
        return Physics2D.OverlapCircle(position, checkRadius, obstacleLayer);
    }

    public void ClearSummon()
    {
        foreach (var summon in summonInstances)
        {
            if (summon != null)
            {
                Destroy(summon);
            }
        }
        summonInstances.Clear();
    }
    protected Transform GetOrCreatePlayerSummonablesTransform()
    {
        string summonablesName = "---PLAYER_SUMMONABLES---";
        GameObject summonablesGO = GameObject.Find(summonablesName);
        if (!summonablesGO)
        {
            summonablesGO = new GameObject(summonablesName);
        }
        return summonablesGO.transform;
    }
    //
    public void ApplyBuffToSummons(float damageMultiplier, float attackSpeedMultiplier, float duration)
    {
        // Check if there are any summons in the list
        if (summonInstances != null)
        {
            foreach (GameObject summon in summonInstances)
            {
                // Make sure the summon is not null before trying to get the component
                if (summon != null)
                {
                    SummonComboBuff summonComponent = summon.GetComponent<SummonComboBuff>();
                    if (summonComponent != null)
                    {
                        summonComponent.ApplyBuff(damageMultiplier, attackSpeedMultiplier, duration);
                    }
                }
            }
        }
    }
}