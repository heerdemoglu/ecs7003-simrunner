using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject bot;
    void Start()
    {
        BotMovement botM = GetComponent<BotMovement>();
        bot = GameObject.FindGameObjectWithTag("Bot");
    }

    //IEnumerator moveBot()
    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {/*
            botM.InRange();*/
            bot.transform.LookAt(other.gameObject.transform.position);
            UnityEngine.Debug.Log("in collider");
        }
    }
}
