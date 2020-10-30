using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;//create a public game object
    private Vector3 offset;

    void Start() //called before first frame update
    {
        offset = transform.position;//inistialise the variable offset with the initial position of the camera
        //we can directly acces transform through the variable transform
    }

    //Any update thate happen after movement, used for procedural animations and gathering last known states
    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
