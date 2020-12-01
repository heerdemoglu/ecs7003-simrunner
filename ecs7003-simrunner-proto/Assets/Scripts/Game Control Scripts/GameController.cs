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
    private bool stopTimer = false;
    private float endTime;
    bool isGameOver=false;
    public GameObject playButton;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
       if (isGameOver)
       {
            gameOverText.text = "GAME OVER !!";
            playButton.SetActive(true);
        }
       if (Input.GetKeyDown(KeyCode.P) && !isGameOver)
        {
            if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 0;
            }

            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
    public void StopTime()
    {
        endTime = Time.time-startTime;
        stopTimer = true;
        timerText.color = Color.red;
       // Debug.Log(endTime);

    }
}

