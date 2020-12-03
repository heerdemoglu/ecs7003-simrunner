using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameController : MonoBehaviour
{
    public Text timerText;
    public Text gameOverText;

    //timer
    private float startTime;
    private bool stopTimer = false;
    private float endTime;

    //game booleans
    private bool isGameOver = false;
    private bool gameBegun = false;

    private Zone zone;
    //displays
    public GameObject gameOverOptions;
    public GameObject pausePanel;

    void Start()
    {
        GameObject chasingZone = GameObject.FindGameObjectWithTag("Zone");
        zone = chasingZone.GetComponent<Zone>();
    }
    //IEnumerator fadeIn()
    //{
    //    GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
    //    Fade fade = cam.GetComponent<Fade>();
    //    //float fadeTime = fade.FadeIn();
    //    yield return new WaitForSeconds(fade.fadeTime);
    //    stopTimer = false;
    //    startTime = Time.time;

    //}

    // Update is called once per frame

    void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
        if (isGameOver)
       {
            gameOverOptions.SetActive(true);
       }
        if (Input.GetKeyDown(KeyCode.P) && !isGameOver)//dislay pause menu
        {
            if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                pausePanel.SetActive(false);
            }
            else
            {
                Time.timeScale = 0;
                pausePanel.SetActive(true);
            }
        }
        if (stopTimer || !gameBegun)
        {
            return;
        }
        float timer = Time.time - startTime;//in seconds
        int minutes = (int)timer / 60; //amount of minutes
        int seconds = (int)timer % 60;//remainder seconds from minutes
        int fraction = (int)(timer * 100) % 100;
        timerText.text = string.Format("{0:00} : {1:00} : {2:00}", minutes, seconds, fraction);
        
    }
    
    public void leaveGame()
    {
        StopTime();
        Time.timeScale = 1;
    }

    public void startGame()
    {
        gameBegun = true;
        startTime = Time.time;
        zone.beginChase();

    }

    //IEnumerator startTimer() {
    //    yield return new WaitUntil(() => GUI.color.a == 0);
    //    stopTimer = false;
    //    startTime = Time.time;
    //}
    public void StopTime()
    {
        endTime = Time.time-startTime;
        stopTimer = true;
        timerText.color = Color.red;
        if(Time.timeScale != 0)
            isGameOver = true;

    }
}

