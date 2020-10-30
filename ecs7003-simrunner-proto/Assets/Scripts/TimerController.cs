using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimerController : MonoBehaviour
{
    public Text timerText;
    public float time;
    public float startTime;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        Debug.Log(System.DateTime.Now.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        float timer = Time.time - startTime;//in seconds
        

        int minutes = (int)timer / 60; //amount of minutes
        int seconds = (int)timer % 60;//remainder seconds from minutes
        //int fraction = (int)(timer * 100) % 100;
        timerText.text = string.Format("{0:00} : {1:00} ", minutes, seconds);
        
    }
}

