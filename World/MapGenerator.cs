using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator instance;

    [SerializeField] public GameplayStageData gameplayStageData;
    public GameObject[,] terrainTiles;
    public float tileSize = 20f;

    private void Awake()
    {
        // Singleton Pattern Check
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(WaitForDataInitialization());
    }

    IEnumerator WaitForDataInitialization()
    {
        while (gameplayStageData == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        terrainTiles = new GameObject[gameplayStageData.terrainTileHorizontalCount, gameplayStageData.terrainTileVerticalCount];
        GenerateMap();
    }

    public void GenerateMap()
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                GenerateTileAt(x, y);
            }
        }
    }

    public void GenerateTileAt(int x, int y)
    {
        // Null Checks
        if (gameplayStageData == null || gameplayStageData.possibleTerrainTiles == null || gameplayStageData.possibleTerrainTiles.Length == 0)
        {
            UnityEngine.Debug.Log("gameplayStageData or possibleTerrainTiles is not set or empty in MapGenerator");
            return;
        }

        if (WorldScrolling.instance == null)
        {
            UnityEngine.Debug.Log("WorldScrolling instance is null");
            return;
        }

        UnityEngine.Debug.Log("WorldScrolling instance: " + WorldScrolling.instance);
        UnityEngine.Debug.Log("tileSize in WorldScrolling: " + WorldScrolling.instance.tileSize);

        GameObject tilePrefab = gameplayStageData.possibleTerrainTiles[UnityEngine.Random.Range(0, gameplayStageData.possibleTerrainTiles.Length)];

        UnityEngine.Debug.Log("tilePrefab: " + tilePrefab);

        GameObject tileInstance = Instantiate(tilePrefab, new Vector3(x, y, 0f) * WorldScrolling.instance.tileSize, Quaternion.identity, transform);

        int arrayPosX = x + (gameplayStageData.terrainTileHorizontalCount / 2);
        int arrayPosY = y + (gameplayStageData.terrainTileVerticalCount / 2);

        // Bounds Check
        if (arrayPosX >= 0 && arrayPosX < gameplayStageData.terrainTileHorizontalCount && arrayPosY >= 0 && arrayPosY < gameplayStageData.terrainTileVerticalCount)
        {
            terrainTiles[arrayPosX, arrayPosY] = tileInstance;
        }
    }

    public void SetGameplayStageData(GameplayStageData data)
    {
        if (data != null)
            gameplayStageData = data;
    }

    public Vector3 CalculateTilePosition(int x, int y)
    {
        return new Vector3(x * tileSize, y * tileSize, 0f);
    }

    public int CalculatePositionOnAxis(float currentValue, bool horizontal)
    {
        if (horizontal)
        {
            if (currentValue >= 0)
            {
                return (int)(currentValue % gameplayStageData.terrainTileHorizontalCount);
            }
            else
            {
                currentValue += 1;
                return (int)(gameplayStageData.terrainTileHorizontalCount - 1 + currentValue % gameplayStageData.terrainTileHorizontalCount);
            }
        }
        else
        {
            if (currentValue >= 0)
            {
                return (int)(currentValue % gameplayStageData.terrainTileVerticalCount);
            }
            else
            {
                currentValue += 1;
                return (int)(gameplayStageData.terrainTileVerticalCount - 1 + currentValue % gameplayStageData.terrainTileVerticalCount);
            }
        }
    }
}