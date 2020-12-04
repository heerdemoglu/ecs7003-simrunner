using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    Transform myTransform;
    Vector3 randomRotation;
    Vector3 randomMovement;

    private void Awake()
    {
        myTransform = transform;
    }

    void Start()
    {
        randomRotation.x = 0;
        randomRotation.y = Random.Range(-5, 5);
        randomRotation.z = Random.Range(-20, 20);
    }

    
    void Update()
    {
        myTransform.Rotate(randomRotation * Time.deltaTime);
    }
}
