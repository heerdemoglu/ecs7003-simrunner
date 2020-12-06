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
    public GameObject settingPanel;

    void Start()
    {
        GameObject chasingZone = GameObject.FindGameObjectWithTag("Zone");
        zone = chasingZone.GetComponent<Zone>();
        pausePanel.SetActive(false);
        settingPanel.SetActive(false);

        //start background music
        FindObjectOfType<AudioManager>().Play("game background music", true);
    }

    void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
        if (isGameOver)
        {
            gameOverOptions.SetActive(true);
            //AudioSource audioData = GetComponent<AudioSource>();
            //audioData.Play();

        }
        if (Input.GetKeyDown(KeyCode.P) && !isGameOver)//dislay pause menu
        {
            if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                pausePanel.SetActive(false);
                settingPanel.SetActive(false);
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

    public void gameOver()
    {
        isGameOver = true;
        StopTime();
    }
    public void StopTime()
    {
        endTime = Time.time-startTime;
        stopTimer = true;
        timerText.color = Color.red;

    }

    public void openSettings()
    {
        settingPanel.SetActive(true);
    }
    public void closeSettings()
    {
        settingPanel.SetActive(false);
    }
    //public void setAcceleration(float sliderAcceleration)
    //{
    //    GameObject player = GameObject.FindGameObjectWithTag("Player");
    //    player.GetComponent<PlayerController>().acceleration = sliderAcceleration;

    //}
    //public void setRotation(float sliderRotation)
    //{
    //    GameObject player = GameObject.FindGameObjectWithTag("Player");
    //    player.GetComponent<PlayerController>().rotationSpeed = sliderRotation;

    //}


}

