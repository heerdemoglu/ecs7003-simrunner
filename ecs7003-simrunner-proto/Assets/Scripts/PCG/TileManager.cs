using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    public GameObject BoundaryTile;
    public float fwdSpawn;
    public float tileLen;
    public Transform playerTransform;
    public int noOfAliveBounds;
    public float spawnOffset;

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
        }
    }

    /**
     * As player moves spawn new game boundary and kill the old ones.
     */
    void Update()
    {
        Debug.Log(playerTransform.position.z);
        if (playerTransform.position.z - spawnOffset > (fwdSpawn - noOfAliveBounds * tileLen))
        {
            SpawnGameBoundary();
            DeleteBoundary();
        }
    }

    /**
     * Spawns a tile and adds its to the set of alive tiles.
     */
    public void SpawnGameBoundary()
    {
        GameObject boundary = Instantiate(BoundaryTile, transform.forward * fwdSpawn, transform.rotation);
        aliveTiles.Add(boundary);
        fwdSpawn += tileLen;
    }

    /**
     * Destroys the boundary from the starting position to keep memory in check.
     */
    private void DeleteBoundary()
    {
        Destroy(aliveTiles[0]);
        aliveTiles.RemoveAt(0);
    }
}
