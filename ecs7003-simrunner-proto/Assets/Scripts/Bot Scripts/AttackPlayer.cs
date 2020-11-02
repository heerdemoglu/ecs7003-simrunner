using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AttackPlayer : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject bot;
    public GameObject bullet;

    public Transform spawnValues;

    public float spawnWait;
    private bool inRange = false;

    void Start()
        {

            BotMovement botM = GetComponent<BotMovement>();
            bot = GameObject.FindGameObjectWithTag("Bot");
            StartCoroutine(SpawnBullet());
        }

    IEnumerator SpawnBullet()
    {
        
        while ( true)
        {
            //while player in range spawn bullet from the bulletbarrel(spawnValues)
            yield return new WaitUntil(() => inRange==true);

            Vector3 spawnPosition = spawnValues.position;
            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(bullet, spawnValues.position, spawnRotation);
            yield return new WaitForSeconds(spawnWait);
            
        }
        
    }

    

    void Update()
    {
       
    }

    void OnTriggerStay(Collider other)
    {
        
        if (other.gameObject.tag == "Player")
        {//player in range - bot faces player
            inRange = true;
            bot.transform.LookAt(other.gameObject.transform.position);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inRange = false;
        }
    }
    
}
