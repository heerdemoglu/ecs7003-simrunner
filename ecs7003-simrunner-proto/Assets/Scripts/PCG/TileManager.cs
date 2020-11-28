using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    public GameObject BoundaryTile;
    public GameObject[] tiles; // Array used for different type of tiles (if available)

    public float fwdSpawn;
    public float tileLen;
    public Transform playerTransform;
    
    public int noOfAliveBounds;
    public int noOfTilesPerBound;

    public float spawnOffset;
    public Vector3 spawnTileValues;

    private List<GameObject> aliveBounds = new List<GameObject>();
    private List<GameObject> aliveTiles = new List<GameObject>();

    /**
     * Starts the game with a single game boundary. The rest is built as player moves.
     */
    void Start()
    {
        // Start with a game boundary:
        for (int i = 0; i < noOfAliveBounds; i++)
        {
            SpawnGameBoundary();
            SpawnTiles();
        }
    }

    /**
     * As player moves spawn new game boundary and kill the old ones.
     */
    void Update()
    {
        if (playerTransform.position.z - spawnOffset > (fwdSpawn - noOfAliveBounds * tileLen))
        {
            SpawnGameBoundary();
            SpawnTiles();

            DestroyBoundary();
            DestroyTiles();
        }
    }

    /**
     * Spawns a tile and adds its to the set of alive tiles.
     */
    public void SpawnGameBoundary()
    {
        GameObject boundary = Instantiate(BoundaryTile, transform.forward * fwdSpawn, transform.rotation);
        aliveBounds.Add(boundary);
        fwdSpawn += tileLen;
    }

/**
 * Randomly spawns tiles in the bounded area.
 * 
 */
    public void SpawnTiles()
    {
        for (int i = 0; i < noOfTilesPerBound; i++)
        {

            Vector3 tilePos = new Vector3(Random.Range(-spawnTileValues.x, spawnTileValues.x),
                Random.Range(0, spawnTileValues.y),
                Random.Range(-spawnTileValues.z, spawnTileValues.z));

            // Rotation can be added as well:
            GameObject tile = Instantiate(tiles[0], tilePos + transform.forward * (fwdSpawn-tileLen),
                Quaternion.Euler(new Vector3(0, 90, 0)));
            aliveTiles.Add(tile);
        }
    }


    /**
     * Destroys the boundary from the starting position to keep memory in check.
     */
    private void DestroyBoundary()
    {
        Destroy(aliveBounds[0]);
        aliveBounds.RemoveAt(0);
    }

    /**
     * Removes first N tiles we left behind.
     * 
     */
    public void DestroyTiles()
    {
        for (int i = 0; i < noOfTilesPerBound; i++)
        {
            Destroy(aliveTiles[0]);
            aliveTiles.RemoveAt(0);
        }
    }
}
