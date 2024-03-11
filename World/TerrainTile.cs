using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTile : MonoBehaviour
{
    [SerializeField] Vector2Int tilePosition;
    [SerializeField] List<SpawnObject> spawnObjects;

    // Start is called before the first frame update
    void Start()
    {
        // We are no longer adding the tile to WorldScrolling
        // Instead, it's now handled by the MapGenerator

        // If you want to initialize the tile position off-screen, you can still keep this line.
        transform.position = new Vector3(-100, -100, 0);
    }

    public void Spawn()
    {
        for (int i = 0; i < spawnObjects.Count; i++)
        {
            spawnObjects[i].Spawn();
        }
    }

}