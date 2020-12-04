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
    }

    public void beginChase()
    {
        chasing = true;
    }

    public void pauseChase()
    {
        chasing = false;
    }
}   

