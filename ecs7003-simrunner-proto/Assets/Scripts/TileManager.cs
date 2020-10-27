using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    public GameObject tilePrefab;
    public float xSpawn = 0; // Spawning tile
    public float tileLength;
    public int noOfTiles;

    public Transform playerTransform;
    public Vector3 pos;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < noOfTiles; i++)
        {
            spawnTile();
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform.position.x > xSpawn - noOfTiles*tileLength) // debug this
        {
            // Add mechanic to remove old tiles as well?
            // Reuse old tiles; like teleport gates?
            spawnTile();

        }
    }

    public void spawnTile()
    {
        Instantiate(tilePrefab, transform.right * xSpawn, transform.rotation); 
        xSpawn += tileLength;
    }

}
