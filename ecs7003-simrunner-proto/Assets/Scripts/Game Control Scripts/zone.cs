using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{

    private GameObject player
    public float speed;
    private bool chasing = false;
    private float acceleration;
    // Update is called once per frame
    void Start()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        

    }
    void Update()
    {
        acceleration = player.GetComponent<PlayerController>().acceleration;
        if (chasing)
        {
            transform.position = transform.position + (Vector3.forward * speed *acceleration* Time.deltaTime);
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

