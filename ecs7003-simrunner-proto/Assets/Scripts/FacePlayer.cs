using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject bot;
    public GameObject bullet;
    // public Vector3 spawnValues;
    public Transform spawnValues;

    public float startWait;
    public float spawnWait;
    //public float waveWait;
    private bool inRange = false;
    IEnumerator SpawnBullet()
    {
        yield return new WaitForSeconds(startWait);
        while ( true)//
        {
            //if (inRange == true)
            //{
                Vector3 spawnPosition = spawnValues.position;
                //UnityEngine.Debug.Log("spawn bullet");
                Quaternion spawnRotation = Quaternion.identity;
                Instantiate(bullet, spawnValues.position, spawnRotation);
                yield return new WaitForSeconds(spawnWait);
           // }
            
        }
        yield return new WaitForSeconds(spawnWait);
    }

    void Start()
    {

        BotMovement botM = GetComponent<BotMovement>();
        bot = GameObject.FindGameObjectWithTag("Bot");
        StartCoroutine(SpawnBullet());
    }

    //IEnumerator moveBot()
    // Update is called once per frame
    void Update()
    {
        //if(inRange == true)
        //    StartCoroutine(SpawnBullet());
        //else
        //    StopCoroutine(SpawnBullet());
    }
    void OnTriggerStay(Collider other)
    {
        

        if (other.gameObject.tag == "Player")
        {/*
            botM.InRange();*/
            //StartCoroutine(SpawnBullet());
            inRange = true;
            bot.transform.LookAt(other.gameObject.transform.position);
            UnityEngine.Debug.Log("in collider");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            inRange = false;
        }
    }
    //
}
