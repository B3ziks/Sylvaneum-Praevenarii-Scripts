using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TurretSpawnerBase : WeaponBase
{
    public GameObject turretPrefab;
    public float turretSpawnDistance = 2f;

    protected List<GameObject> turretInstances = new List<GameObject>();

    [SerializeField] private LayerMask obstacleLayer;
    private const int MAX_SPAWN_ATTEMPTS = 10;
    private const float SAFE_DISTANCE_BETWEEN_TURRETS = 2f; // Increased safe distance

    private bool isFirstSpawnAttempt = true;

    protected void Start()
    {
        StartCoroutine(DelayFirstSpawn());
    }

    private IEnumerator DelayFirstSpawn()
    {
        yield return new WaitForSeconds(0.5f); // Delay for 0.5 seconds (adjust as needed)
        isFirstSpawnAttempt = false;
    }
    public override void Attack()
    {
        if (isFirstSpawnAttempt)
        {
            Debug.Log("[TurretSpawnerBase] Delaying first turret spawn.");
            return;
        }

        if (turretInstances.Count < weaponData.stats.maxNumberOfSummonedInstances)
        {
            Vector2 spawnPosition = CalculateSpawnPositionAroundPlayer();
            if (spawnPosition != Vector2.zero)
            {
                GameObject turretInstance = Instantiate(turretPrefab, spawnPosition, Quaternion.identity);
                SetupTurret(turretInstance);
                turretInstances.Add(turretInstance);

                // Set the parent of the instantiated turret to ---PlayerSummonables---
                Transform playerSummonablesTransform = GetOrCreatePlayerSummonablesTransform();
                turretInstance.transform.SetParent(playerSummonablesTransform, true);
            }
        }
    }


    protected abstract void SetupTurret(GameObject turretInstance);

    public Vector2 CalculateSpawnPositionAroundPlayer()
    {
        Vector2 spawnPosition = Vector2.zero;
        int attempts = MAX_SPAWN_ATTEMPTS;

        while (attempts > 0)
        {
            float angle = UnityEngine.Random.Range(0, 360) * Mathf.Deg2Rad;
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * turretSpawnDistance;
            spawnPosition = (Vector2)transform.position + offset;

            if (!IsPositionOnObstacle(spawnPosition) && !IsPositionOnOtherTurret(spawnPosition))
            {
                return spawnPosition;
            }

            attempts--;
        }

        UnityEngine.Debug.LogWarning("Failed to find a valid turret spawn position!");
        return Vector2.zero;
    }

    protected bool IsPositionOnObstacle(Vector2 position)
    {
        float checkRadius = 0.5f; // You might want to adjust this based on the size of your turret and obstacles
        return Physics2D.OverlapCircle(position, checkRadius, obstacleLayer);
    }

    protected bool IsPositionOnOtherTurret(Vector2 position)
    {
        foreach (var turret in turretInstances)
        {
            if (turret != null && Vector2.Distance(turret.transform.position, position) < SAFE_DISTANCE_BETWEEN_TURRETS)
            {
                return true;
            }
        }
        return false;
    }


    public void ClearTurrets()
    {
        foreach (var turret in turretInstances)
        {
            Destroy(turret);
        }
        turretInstances.Clear();
    }
    private Transform GetOrCreatePlayerSummonablesTransform()
    {
        string summonablesName = "---PLAYER_SUMMONABLES---";
        GameObject summonablesGO = GameObject.Find(summonablesName);
        if (!summonablesGO)
        {
            summonablesGO = new GameObject(summonablesName);
        }
        return summonablesGO.transform;
    }
}