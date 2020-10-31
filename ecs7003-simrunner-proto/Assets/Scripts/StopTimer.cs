using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class StopTimer : MonoBehaviour
{
    private GameController gameController;
    private void Start()
    {
        GameObject gameC = GameObject.FindGameObjectWithTag("GameController");
        gameController = gameC.GetComponent<GameController>();
    }
    void OnTriggerEnter(Collider other)
    {
        gameController.StopTime();
    }
}
