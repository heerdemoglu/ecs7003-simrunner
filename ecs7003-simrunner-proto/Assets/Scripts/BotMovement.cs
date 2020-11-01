using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;

public class BotMovement : MonoBehaviour
{
    // Start is called before the first frame update
    //public GameObject toMove;
    public float speed;

    //public float startWait;
    //public float turnWait;

    //public Transform target1;
    //public Transform target2;

    private float distance;
    private Vector3 destination;
    private Vector3 startPos;

    public GameObject zone;

    private bool playerInRange;
    private GameObject player;

    void Start()
    {
        playerInRange = false;
        player = GameObject.FindGameObjectWithTag("Player");

        //destination = new Vector3(10f, 0f, 0f);//target2.position;
        //startPos = new Vector3(-10f, 0f, 0f);//target1.position;

        distance = Vector3.Distance(startPos, destination);

        transform.position = startPos;
        transform.LookAt(destination);

        //StartCoroutine(move());
    }
    void Update()
    {
        while (playerInRange == false)
        {
            transform.Translate(destination * Time.deltaTime);
            float currentDist = Vector3.Distance(startPos, transform.position);
            if (currentDist >= distance)
            {
                transform.Rotate(0.0f, 180.0f, 0.0f, Space.Self);
                Vector3 temp = destination;
                destination = startPos;
                startPos = destination;
                transform.position = startPos;

            }
        }

    }


    private void isInRange(bool inRange)
    {
        playerInRange = inRange;
        UnityEngine.Debug.Log(" call range");
        //Vector3 playerpos = player.transform.position;
        //transform.LookAt(playerpos);
        //unityengine.Debug.log("inrange");
    }
    // Update is called once per frame

}
