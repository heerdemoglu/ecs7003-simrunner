using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{

    public GameObject startTile;
    public GameObject[] platformChunks; // Array used for different type of tiles (if available)
    public GameObject player;

    private List<GameObject> shardList = new List<GameObject>();

    public int noOfChunks;

    private List<GameObject> alivePlatforms = new List<GameObject>();
    private Vector3 lastPlatformEnding = new Vector3(0, 0, 0);

    void Start()
    {
        GameObject startPlatform = Instantiate(startTile, transform.forward, transform.rotation);
        alivePlatforms.Add(startPlatform);


        // Start with a game boundary:
        for (int i = 0; i < noOfChunks; i++)
        {
            SpawnPlatforms();
        }
    }

    void Update()
    {
        if (lastPlatformEnding.z - player.transform.position.z < 30)
        {
            for (int i = 0; i < 2; i++)
            {
                SpawnPlatforms();
                DestroyPlatforms();
            }

        }
    }

    public void SpawnPlatforms()
    {
        for (int i = 0; i < noOfChunks; i++)
        {

            int tiletype = Random.Range(0, platformChunks.Length);

            GameObject tile = Instantiate(platformChunks[tiletype], lastPlatformEnding, Quaternion.Euler(new Vector3(0, 0, 0)));

            lastPlatformEnding = tile.transform.GetChild(0).position; // by design first child is end
            alivePlatforms.Add(tile);


        }
    }

    private void DestroyPlatforms()
    {
        Destroy(alivePlatforms[0]);
        alivePlatforms.RemoveAt(0);
    }
}
