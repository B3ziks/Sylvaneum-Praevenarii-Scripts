using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class WorldScrolling : MonoBehaviour
{
    public static WorldScrolling instance;

    Transform playerTransform;
    Vector2Int currentTilePosition = new Vector2Int(0, 0);
    [SerializeField] Vector2Int playerTilePosition;
    [SerializeField] public float tileSize = 20f;
    [SerializeField] private float bufferSize = 10f; // Buffer before updating the tiles

    private Vector2 lastUpdatedPosition; // Position when tiles were last updated

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerTransform = GameManager.instance.playerTransform;
        currentTilePosition.x = Mathf.FloorToInt(playerTransform.position.x / tileSize);
        currentTilePosition.y = Mathf.FloorToInt(playerTransform.position.y / tileSize);

        UpdateTilesOnScreen();

        // Set lastUpdatedPosition to a far off location initially
        lastUpdatedPosition = new Vector2(10000f, 10000f);
    }

    private void Update()
    {
        playerTilePosition.x = Mathf.FloorToInt(playerTransform.position.x / tileSize);
        playerTilePosition.y = Mathf.FloorToInt(playerTransform.position.y / tileSize);

        Vector2 bufferTopLeft = lastUpdatedPosition + new Vector2(-bufferSize, bufferSize);
        Vector2 bufferBottomRight = lastUpdatedPosition + new Vector2(bufferSize, -bufferSize);

        // Check if the player is outside the buffer zone
        bool isOutsideBuffer = playerTransform.position.x < bufferTopLeft.x ||
                               playerTransform.position.x > bufferBottomRight.x ||
                               playerTransform.position.y > bufferTopLeft.y ||
                               playerTransform.position.y < bufferBottomRight.y;

        if (isOutsideBuffer)
        {
            currentTilePosition = playerTilePosition;
            UpdateTilesOnScreen();
            lastUpdatedPosition = playerTransform.position; // Update the last position after updating tiles
        }
    }

    private void UpdateTilesOnScreen()
    {
        if (MapGenerator.instance == null)
        {
            UnityEngine.Debug.LogWarning("MapGenerator instance is null");
            return;
        }

        MapGenerator mapGen = MapGenerator.instance;

        if (mapGen.terrainTiles == null)
        {
            UnityEngine.Debug.LogWarning("terrainTiles is null");
            return;
        }

        for (int pov_x = -(mapGen.gameplayStageData.fieldOfVisionWidth / 2); pov_x <= mapGen.gameplayStageData.fieldOfVisionWidth / 2; pov_x++)
        {
            for (int pov_y = -(mapGen.gameplayStageData.fieldOfVisionHeight / 2); pov_y <= mapGen.gameplayStageData.fieldOfVisionHeight / 2; pov_y++)
            {
                int tileToUpdate_x = mapGen.CalculatePositionOnAxis(playerTilePosition.x + pov_x, true);
                int tileToUpdate_y = mapGen.CalculatePositionOnAxis(playerTilePosition.y + pov_y, false);

                if (tileToUpdate_x >= 0 && tileToUpdate_x < mapGen.terrainTiles.GetLength(0) &&
                    tileToUpdate_y >= 0 && tileToUpdate_y < mapGen.terrainTiles.GetLength(1))
                {
                    GameObject tile = mapGen.terrainTiles[tileToUpdate_x, tileToUpdate_y];

                    if (tile != null)
                    {
                        Vector3 newPosition = mapGen.CalculateTilePosition(
                            playerTilePosition.x + pov_x,
                            playerTilePosition.y + pov_y
                        );

                        if (newPosition != tile.transform.position)
                        {
                            tile.transform.position = newPosition;
                        }
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Tile at [" + tileToUpdate_x + "," + tileToUpdate_y + "] is null.");
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError($"Attempted out-of-bounds access: [{tileToUpdate_x}, {tileToUpdate_y}]");
                }
            }
        }
    }
}