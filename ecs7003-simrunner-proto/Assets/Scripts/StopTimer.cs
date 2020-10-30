using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class StopTimer : MonoBehaviour
{
    private TimerController timeController;
    private void Start()
    {
        GameObject gameC = GameObject.FindGameObjectWithTag("TimerController");
        timeController = gameC.GetComponent<TimerController>();
    }
    void OnTriggerEnter(Collider other)
    {
        timeController.StopTime();
        
    }
}
