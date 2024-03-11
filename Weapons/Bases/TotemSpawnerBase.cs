using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TotemSpawnerBase : WeaponBase
{
    public GameObject totemPrefab;
    public float totemSpawnDistance = 3f;

    protected List<GameObject> totemInstances = new List<GameObject>();

    [SerializeField] private LayerMask obstacleLayer;
    private const int MAX_SPAWN_ATTEMPTS = 10;

    // Reference to TotemSpiritCombo script
    protected TotemSpiritCombo totemSpiritCombo;
    private bool isFirstSpawnAttempt = true;

    protected void Start()
    {
        totemSpiritCombo = FindObjectOfType<TotemSpiritCombo>();
        Debug.Log($"[TotemSpawnerBase] Start called. Max Instances: {weaponData.stats.maxNumberOfSummonedInstances}");
        StartCoroutine(DelayFirstSpawn());

    }

    public override void Attack()
    {
        if (isFirstSpawnAttempt)
        {
            Debug.Log("[TotemSpawnerBase] Delaying first totem spawn.");
            return;
        }
        Debug.Log($"[TotemSpawnerBase] Attack called. Current Instances: {totemInstances.Count}, Max Instances: {weaponData.stats.maxNumberOfSummonedInstances}");

        if (totemInstances.Count < weaponData.stats.maxNumberOfSummonedInstances)
        {
            Vector2 spawnPosition = CalculateSpawnPositionAroundPlayer();
            if (spawnPosition != Vector2.zero)
            {
                GameObject totemInstance = Instantiate(totemPrefab, spawnPosition, Quaternion.identity);
                if (totemInstance == null)
                {
                    Debug.LogError("[TotemSpawnerBase] Failed to instantiate totem.");
                    return;
                }

                SetupTotem(totemInstance);
                totemInstances.Add(totemInstance);
                totemSpiritCombo?.AddTotem(totemInstance);

                Debug.Log($"[TotemSpawnerBase] Totem spawned at {spawnPosition}. Total Instances: {totemInstances.Count}");
            }
            else
            {
                Debug.LogWarning("[TotemSpawnerBase] Failed to find a valid spawn position for totem.");
            }
        }
        else
        {
            Debug.Log("[TotemSpawnerBase] Maximum number of totem instances reached.");
        }
    }
    private IEnumerator DelayFirstSpawn()
    {
        yield return new WaitForSeconds(0.5f); // Delay for 0.5 seconds (adjust as needed)
        isFirstSpawnAttempt = false;
    }

    protected abstract void SetupTotem(GameObject totemInstance);

    public Vector2 CalculateSpawnPositionAroundPlayer()
    {
        Vector2 spawnPosition = Vector2.zero;
        int attempts = MAX_SPAWN_ATTEMPTS;

        while (attempts > 0)
        {
            float angle = UnityEngine.Random.Range(0, 360) * Mathf.Deg2Rad;
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * totemSpawnDistance;
            spawnPosition = (Vector2)transform.position + offset;

            if (!IsPositionOnObstacle(spawnPosition) && !IsPositionOnOtherTotem(spawnPosition))
            {
                return spawnPosition;
            }

            attempts--;
        }

        Debug.LogWarning("Failed to find a valid spawn position for totem!");
        return Vector2.zero;
    }

    private bool IsPositionOnOtherTotem(Vector2 position)
    {
        foreach (var totem in totemInstances)
        {
            if (totem != null && Vector2.Distance(totem.transform.position, position) < 1f)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsPositionOnObstacle(Vector2 position)
    {
        float checkRadius = 0.5f; // Adjust based on turret/obstacle sizes
        return Physics2D.OverlapCircle(position, checkRadius, obstacleLayer);
    }

    public void ClearTotems()
    {
        foreach (var totem in totemInstances)
        {
            Destroy(totem);
        }
        totemInstances.Clear();
        totemSpiritCombo?.ClearTotems();
    }
}