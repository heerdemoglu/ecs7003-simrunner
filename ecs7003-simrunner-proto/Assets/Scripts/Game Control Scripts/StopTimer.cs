using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class StopTimer : MonoBehaviour
{
    public Text gameOver;
    private GameController gameController;
    private void Start()
    {
        GameObject gameC = GameObject.FindGameObjectWithTag("GameController");
        gameController = gameC.GetComponent<GameController>();//gets the script component
    }
    /* void Update()
    {
        if (gameOver.text == "YOU DIED")
        {
            gameController.StopTime();
            return;
        }
    }*/
    void OnTriggerEnter(Collider other)
    {
        gameController.StopTime();
    }
}
