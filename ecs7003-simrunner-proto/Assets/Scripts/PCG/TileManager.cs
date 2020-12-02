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
    public Vector3 rotationValues;

    public int movingTiles;
    public int rotatingTiles;

    public Vector3 randomMovementValues;
    public Vector3 randomRotationValues;
    public float tileMovementSpeed;
    public float tileRotationSpeed;

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

        //UpdateTilePositionRotation();

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

            int tiletype = Random.Range(0, tiles.Length);

            Vector3 tilePos = new Vector3(Random.Range(-spawnTileValues.x, spawnTileValues.x),
                Random.Range(0, spawnTileValues.y),
                Random.Range(-spawnTileValues.z, spawnTileValues.z));

            if (tiletype != 1) {
                // Rotation can be added as well: - Not to horizontal tile which is set to 1 
                GameObject tile = Instantiate(tiles[tiletype], tilePos + transform.forward * (fwdSpawn - tileLen),
                    Quaternion.Euler(new Vector3(0, Random.Range(-rotationValues.y, rotationValues.y), Random.Range(-rotationValues.z, rotationValues.z))));
                aliveTiles.Add(tile);
            }else
            {
                GameObject tile = Instantiate(tiles[tiletype], tilePos + transform.forward * (fwdSpawn - tileLen),
                    Quaternion.Euler(new Vector3(0,0,0)));
                aliveTiles.Add(tile);
            }

        }
    }

    /**
     * Randomly changes the position and rotation of some tiles for increased complexity.
     * Flat tiles are not moved or rotated. -- BUGGY
     */
    private void UpdateTilePositionRotation()
    {
        // Pick random tiles to rotate from list of tiles:
        for(int i = 0; i < rotatingTiles; i++)
        {
            int idx = Random.Range(0, aliveTiles.Count);
            Quaternion to = Quaternion.Euler(0, 0, 0);


            if (to == aliveTiles[idx].transform.rotation)
            {
                to = Quaternion.Euler(Random.Range(-randomRotationValues.x, randomRotationValues.x), Random.Range(-randomRotationValues.y, randomRotationValues.y),
                Random.Range(-randomRotationValues.z, randomRotationValues.z));
            }
            
            aliveTiles[idx].transform.rotation = Quaternion.Slerp(aliveTiles[idx].transform.rotation, to, tileRotationSpeed * Time.deltaTime);
        }

        // Pick random tiles to move:
        // Pick random tiles to rotate from list of tiles:
        for (int i = 0; i < movingTiles; i++)
        {
            int idx = Random.Range(0, aliveTiles.Count);
            Vector3 to = new Vector3(0, 0, 0);


            if (aliveTiles[idx].transform.position + to == aliveTiles[idx].transform.position)
            {
                to = new Vector3(Random.Range(-randomMovementValues.x, randomMovementValues.x), Random.Range(-randomMovementValues.y, randomMovementValues.y),
                Random.Range(-randomMovementValues.z, randomMovementValues.z));
            }

            aliveTiles[idx].transform.position = Vector3.Slerp(aliveTiles[idx].transform.position, aliveTiles[idx].transform.position+to, tileMovementSpeed * Time.deltaTime);
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
