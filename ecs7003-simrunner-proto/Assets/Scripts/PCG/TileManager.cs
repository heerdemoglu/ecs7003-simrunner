using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    #region Singleton

    private static TileManager _instance;

    public static TileManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<TileManager>();

            return _instance;
        }
    }

    #endregion


    public int tileLength;
    public Transform tileContainer;
    public List<GameObject> Tiles { get; } = new List<GameObject>();
    [SerializeField] private GameObject tileReference = null;


    void Start()
    {
        CreateTiles();
    }

    private void CreateTiles()
    {
        Vector3 pivot = Vector3.forward;
        GameObject go = Instantiate(tileReference, Vector3.zero, Quaternion.identity, tileContainer);

        Tiles.Add(go);
        for (int i = 1; i < tileLength; i++)
        {
            go = Instantiate(tileReference, Tiles[Tiles.Count - 1].transform.position + pivot,
                Quaternion.identity, tileContainer);

            Tiles.Add(go);
        }
    }

    // public void DestroyTile(GameObject tile)
    // {
    // }
}