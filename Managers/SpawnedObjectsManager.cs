using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SpawnedObjectsManager : MonoBehaviour
{
    public static SpawnedObjectsManager instance;

    [SerializeField] private Vector2 playerSpawnPoint = Vector2.zero;
    [SerializeField] private float safeZoneRadius = 5f;

    [SerializeField] private List<Vector2> allSpawnedObjectPositions = new List<Vector2>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Iterate over the objects and draw their safe zones
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.CompareTag("Obstacle")) // Replace with your specific tag
            {
                Vector3 objectPosition = obj.transform.position; // Use the object's world position
                Gizmos.DrawWireSphere(objectPosition, safeZoneRadius);
            }
        }
    }

    public void RegisterSpawnedObject(Vector2 position)
    {
        allSpawnedObjectPositions.Add(position);
        UnityEngine.Debug.Log($"Registered object at position {position}.");
    }

    public bool IsPositionFarEnoughFromAll(Vector2 position, float minDistance)
    {
        // Check against the player spawn point
        if (Vector2.Distance(position, playerSpawnPoint) < safeZoneRadius)
        {
            UnityEngine.Debug.LogWarning($"Position {position} is too close to the player spawn point!");
            return false;
        }

        // Check against all spawned objects
        foreach (Vector2 spawnedPosition in allSpawnedObjectPositions)
        {
            if (IsWithinDistance(position, spawnedPosition, minDistance))
            {
                UnityEngine.Debug.DrawLine(position, spawnedPosition, Color.red, 5f);
                UnityEngine.Debug.LogWarning($"Position {position} is too close to the object at {spawnedPosition}.");
                return false;
            }
        }

        return true;
    }

    private bool IsWithinDistance(Vector2 pos1, Vector2 pos2, float distance)
    {
        float actualDistance = Vector2.Distance(pos1, pos2);
        if (actualDistance < distance)
        {
            UnityEngine.Debug.Log($"Distance between {pos1} and {pos2} is {actualDistance}. It's less than {distance}.");
            return true;
        }
        return false;
    }
}