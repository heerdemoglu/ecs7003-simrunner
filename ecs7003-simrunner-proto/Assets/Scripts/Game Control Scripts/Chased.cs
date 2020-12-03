using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chased : MonoBehaviour
{
    private float distanceToZone;
    private float farTier;
    private float midTier;
    private float nearTier;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Update()
    {
        //Debug.DrawLine(transform.position, Vector3.up);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int layerMask = 9;//9 is zone layer
        RaycastHit hit;
        //if (Physics.Raycast(transform.position, transform.Transform.TransformDirection(vector3.forward), out rHit);
        //send out ray cast from current position and determines the distance to the rHit()
        if (Physics.Raycast(transform.position, Vector3.back, out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, Vector3.back, Color.green);
            Debug.Log("Did Hit");
            distanceToZone = hit.distance;






            //if (distanceToZone<nearTier)
            //{

            //}
            //else if (distanceToZone < midTier)
            //{

            //}
            //else if (distanceToZone < farTier)
            //{

            //}

            
        }
    }
}
