using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chased : MonoBehaviour
{
    private GameController gameController;
    private GameObject zone;
    public float distanceToZone;

    private float farTier;
    private float midTier;
    private float nearTier;
    public float deathTier;
    public float range;
    // Start is called before the first frame update
    void Start()
    {   GameObject gameC = GameObject.FindGameObjectWithTag("GameController");
        zone = GameObject.FindGameObjectWithTag("Zone");
        gameController = gameC.GetComponent<GameController>();
        nearTier = deathTier + range;
        midTier = nearTier + range;
        farTier = midTier + range;
    }
    void Update()
    {
        float zoneZ = zone.transform.position.z;
        distanceToZone = Mathf.Abs(transform.position.z - zoneZ);

        if (distanceToZone < deathTier)
        {
            Debug.Log("made contact");

            gameController.gameOver();
        }
        else if (distanceToZone < nearTier)
        {
            Debug.Log("Within nearTier");
        }
        else if (distanceToZone < midTier)
        {
            Debug.Log("Within midTier");
        }
        else if (distanceToZone < farTier)
        {
            Debug.Log("Within farTier");
        }
        Debug.DrawLine(transform.position, zone.transform.position, Color.red);
    }

    private void OnDrawGizmos()
    {
        GameObject zon = GameObject.FindGameObjectWithTag("Zone");
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, zon.transform.position);
    }
    // Update is called once per frame
    //void FixedUpdate()
    //{
    //    int layerMask = 9;//9 is zone layer
    //    RaycastHit hit;
    //    if (Physics.Raycast(transform.position, Vector3.back, out hit, Mathf.Infinity, layerMask))
    //    {
    //        Debug.DrawRay(transform.position, Vector3.back, Color.red);

    //        distanceToZone = transform.position.z - chasingZone.transform.position.z;

    //        if (distanceToZone < deathTier)
    //        {
    //            Debug.Log("made contact");

    //            gameController.gameOver();
    //        }
    //        else if (distanceToZone < nearTier)
    //        {
    //            Debug.Log("Within nearTier");
    //        }
    //        else if (distanceToZone < midTier)
    //        {
    //            Debug.Log("Within midTier");
    //        }
    //        else if (distanceToZone < farTier)
    //        {
    //            Debug.Log("Within farTier");
    //        }


    //    }
    //}
}
