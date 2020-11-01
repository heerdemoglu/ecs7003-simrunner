using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject bulletDirection;
    public float speed;
    private Vector3 direction;
    void Start()
    {
        //bulletDirection = GameObject.FindGameObjectWithTag("Bot");
        //direction = bulletDirection.transform.forward;
        transform.LookAt(GameObject.FindGameObjectWithTag("Player").transform);
        direction = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        //bullet move in direction of intial forward position
        transform.position = transform.position + (direction*speed * Time.deltaTime);
    }
}
