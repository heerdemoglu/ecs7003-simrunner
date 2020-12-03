using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{


    public float speed;
    private bool chasing = false;
    // Update is called once per frame
    void Update()
    {
        if (chasing)
        {
            transform.position = transform.position + (Vector3.forward * speed * Time.deltaTime);
        }
        //RayCastHit rHit;
        ////if (Physics.Raycast(transform.position, transform.Transform.TransformDirection(vector3.forward), out rHit);
        ////send out ray cast from current position and determines the distance to the rHit()
        //if (Physics.Raycast(transform.position, vector3.forward, out rHit))
        //{
        //    DistanceToPlayer = rHit.distance;
        //}
    }

    public void beginChase()
    {
        chasing = true;
    }
}   

