using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] spawnObjects;
    [SerializeField] private int numberOfObjectsToSpawn = 3;
    [SerializeField] private float minDistanceBetweenObjects = 4f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private Vector2 playerSpawnPoint = Vector2.zero; // Set this in the inspector to where your player spawns, or calculate it
    [SerializeField] private float safeZoneRadius = 5f; // The radius around the player's spawn point where objects can't spawn
    private bool hasSpawned = false;
    [SerializeField] private bool enableSpawning = true;

    private const int MAX_RETRIES = 10;

    private Vector2 spawnAreaMin;
    private Vector2 spawnAreaMax;
    private List<Vector2> spawnedObjectPositions = new List<Vector2>();

    private void Start()
    {
        UnityEngine.Debug.Log($"Player Spawn Point: {playerSpawnPoint}, Safe Zone Radius: {safeZoneRadius}");

        if (enableSpawning && !hasSpawned)
        {
            CalculateSpawnArea();

            // Check if any child objects are present. If not, spawn objects.
            if (transform.childCount == 0)
            {
                SpawnObjects();
            }

            hasSpawned = true;
        }
    }

    private void CalculateSpawnArea()
    {
        // Assumes the TerrainTile is a square and has a SpriteRenderer component
        float tileSize = GetComponent<SpriteRenderer>().bounds.size.x;
        Vector3 tileCenter = transform.position;

        spawnAreaMin = new Vector2(tileCenter.x - tileSize / 2, tileCenter.y - tileSize / 2);
        spawnAreaMax = new Vector2(tileCenter.x + tileSize / 2, tileCenter.y + tileSize / 2);
    }

    private void SpawnObjects()
    {
        for (int i = 0; i < numberOfObjectsToSpawn; i++)
        {
            int retryCount = 0;
            bool success = false;

            while (retryCount < MAX_RETRIES && !success)
            {
                Vector2 spawnPosition = new Vector2(
                    UnityEngine.Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                    UnityEngine.Random.Range(spawnAreaMin.y, spawnAreaMax.y)
                );

                // Debug Log for spawn position validation
                UnityEngine.Debug.Log($"Attempting to spawn object #{i + 1} at {spawnPosition}. Retry count: {retryCount}");

                if (IsValidSpawnPosition(spawnPosition))
                {
                    int selectedObject = UnityEngine.Random.Range(0, spawnObjects.Length);

                    if (spawnObjects[selectedObject] == null)
                    {
                        UnityEngine.Debug.LogError("Spawn object is null!");
                        break;
                    }

                    GameObject newObj = Instantiate(spawnObjects[selectedObject], spawnPosition, Quaternion.identity, transform);

                    spawnedObjectPositions.Add(spawnPosition); // Store the local position
                    SpawnedObjectsManager.instance.RegisterSpawnedObject(spawnPosition);
                    success = true;
                }
                else
                {
                    retryCount++;
                }
            }

            if (retryCount == MAX_RETRIES)
            {
                UnityEngine.Debug.LogWarning($"Max retries reached for object #{i + 1}. Object not spawned.");
            }
        }
    }
    private bool IsWithinSafeZone(Vector2 position)
    {
        return Vector2.Distance(position, playerSpawnPoint) < safeZoneRadius;
    }
    private bool IsValidSpawnPosition(Vector2 position)
    {
        if (IsWithinSafeZone(position))
        {
            // Debug log for visual verification
            UnityEngine.Debug.Log($"Spawn position {position} is within the player's safe zone.");
            return false;
        }

        bool isOccupied = IsPositionOccupied(position);
        bool isSafe = IsPositionSafe(position);
        UnityEngine.Debug.Log($"Position {position} validation: Occupied={isOccupied}, Safe={isSafe}");
        return !isOccupied && isSafe;
    }


    private bool IsPositionSafe(Vector2 position)
    {
        bool withinSafeZone = Vector2.Distance(position, playerSpawnPoint) < safeZoneRadius;
        bool tooCloseToSpawned = spawnedObjectPositions.Exists(pos => Vector2.Distance(pos, position) < minDistanceBetweenObjects);
        bool tooCloseToOthers = !SpawnedObjectsManager.instance.IsPositionFarEnoughFromAll(position, minDistanceBetweenObjects);

        UnityEngine.Debug.Log($"Position {position} within safe zone: {withinSafeZone}");
        UnityEngine.Debug.Log($"Position {position} too close to spawned objects: {tooCloseToSpawned}");
        UnityEngine.Debug.Log($"Position {position} too close to other objects: {tooCloseToOthers}");

        return !withinSafeZone && !tooCloseToSpawned && !tooCloseToOthers;
    }

    private bool IsPositionOccupied(Vector2 position)
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(position, 0.5f, obstacleMask);
        bool isOccupied = hitCollider != null;
        UnityEngine.Debug.Log($"Position {position} is occupied: {isOccupied}");
        return isOccupied;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        // Draw a gizmo at the player's spawn point with the correct safe zone radius
        Gizmos.DrawWireSphere(playerSpawnPoint, safeZoneRadius);
    }
}