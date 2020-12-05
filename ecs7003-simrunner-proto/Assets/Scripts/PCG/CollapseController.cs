using System;
using System.Collections;
using UnityEngine;

public class CollapseController : MonoBehaviour
{
    #region Singleton

    private static CollapseController _instance;

    public static CollapseController Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<CollapseController>();

            return _instance;
        }
    }

    #endregion

    public float speed;
    public Player player;
    public Action OnPlayerCollapsed;


    public Player Player
    {
        get
        {
            if (player == null)
                player = FindObjectOfType<Player>();

            return player;
        }
    }

    void Start()
    {
        StartCoroutine(StartMoveCoroutine());
    }

    private IEnumerator StartMoveCoroutine()
    {
        while (true)
        {
            transform.position += new Vector3(0, 0, 0.005f) * speed;

            if (IsPassedPlayer())
            {
                //Debug.Log("[CollapseController] Passed Player");
                //fire some event
                OnPlayerCollapsed?.Invoke();
                break;
            }

            StartCoroutine(CollapseObjectsBehind());

            yield return null;
        }

        yield return null;
    }

    private IEnumerator CollapseObjectsBehind()
    {
        foreach (GameObject tile in TileManager.Instance.Tiles)
        {
            if (transform.position.z > (tile.transform.position.z + 1f))
            {
                foreach (Tile singleTile in tile.GetComponentsInChildren<Tile>())
                {
                    if (singleTile.gameObject.GetComponent<Rigidbody>() == null)
                    {
                        //TileManager.Instance.DestroyTile(tile);

                        singleTile.gameObject.AddComponent<Rigidbody>();
                        singleTile.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.down * 2, ForceMode.Impulse);
                        singleTile.gameObject.GetComponent<Rigidbody>().AddTorque(5, 0, 10);
                    }


                    yield return new WaitForSeconds(0.01f);
                }
            }
        }
    }

    private bool IsPassedPlayer()
    {
        return Player.transform.position.z <= transform.position.z;
    }
}