using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameController : MonoBehaviour
{
    public Text timerText;
    public Text gameOverText;

    private float startTime;
    private bool stopTimer = true;
    private float endTime;

    bool isGameOver=false;

    public GameObject gameOverOptions;
    public GameObject pausePanel;
    // Start is called before the first frame update
    void Start()
    {

        //startTime = Time.time;
        StartCoroutine("startTimer");
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
        if (stopTimer)
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

    IEnumerator startTimer() {
        yield return new WaitUntil(() => GUI.color.a == 0);
        stopTimer = false;
        startTime = Time.time;
    }
    public void StopTime()
    {
        endTime = Time.time-startTime;
        stopTimer = true;
        timerText.color = Color.red;
        if(Time.timeScale !=0)
            isGameOver = true;

    }
}

