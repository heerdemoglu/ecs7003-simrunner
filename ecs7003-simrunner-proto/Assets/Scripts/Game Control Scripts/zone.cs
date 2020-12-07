using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{

    private GameObject player;
    public float speed;

    private bool chasing = false;
    private float acceleration;

    public float initialDistance = 15f;
    // Update is called once per frame
    void Start()
    {
        Vector3 initialVector = new Vector3(0,0, initialDistance);
        player = GameObject.FindGameObjectWithTag("Player");
        transform.position = player.transform.position - initialVector;

    }
    void Update()
    {
        acceleration = player.GetComponent<PlayerController>().acceleration;
        if (chasing)
        {
            transform.position = transform.position + (Vector3.forward * acceleration * speed * Time.deltaTime);
        }


        if (transform.position.z % 50 == 0)
        {
            speed += 0.01f;
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

