using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameplayStageData")]
public class GameplayStageData : ScriptableObject
{
    public int terrainTileHorizontalCount;
    public int terrainTileVerticalCount;
    public int fieldOfVisionHeight;
    public int fieldOfVisionWidth;
    public GameObject[] possibleTerrainTiles;
    public float tileSize = 20f;  // Add this line

}