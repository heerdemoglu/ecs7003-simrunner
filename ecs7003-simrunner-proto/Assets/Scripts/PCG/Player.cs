using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //CollapseController.Instance.OnPlayerCollapsed += OnPlayerCollapsed;
    }

    // Update is called once per frame
    void Update()
    {
    }


    void OnPlayerCollapsed()
    {
        //Debug.Log("[Player] OnPlayerCollapsed");
    }
}