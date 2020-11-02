using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine.UI;
using UnityEngine;


/**
 * Other implementation of player controller, it doesnt use WASD, directly uses Horizontal and Vertical Inputs.
 * It requires clamping setting fixed speed etc. Not used in the final product.
 */
public class PlayerController : MonoBehaviour
{
    public float speed;//create a public variable of type float
    //private int count;
    //private int numPickUps = 3;
    //public Text scoreText;
    //public Text winText;
    //public Text playerPosition;

    void Start()
    {
        //count = 0;
        //winText.text = " ";
        //SetCountText();

    }

    void FixedUpdate()//method for operation involves physics
    {
        float horAxis = Input.GetAxis("Horizontal");//takes input from arrow keys and captures them using GetAxis()
        float verAxis = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horAxis, 0.0f, verAxis);//forms a vector in the direction the ball should move
        GetComponent<Rigidbody>().AddForce(movement * speed * Time.fixedDeltaTime);//calculates force to be applied to the player
        //SetplayerPositionText();
    }

    //called when other game object’s collider, configured as a trigger, collides with the collider of the game object that has this script as a component

    //void OnTriggerEnter(Collider other)
    //{
    //    //if player collides with a game object with the tag PickUp,it sets the game object to inactive
    //    if (other.gameObject.tag == "PickUp")
    //    {
    //        other.gameObject.SetActive(false);
    //        count++;
    //        SetCountText();
    //    }
    //}
    //private void SetplayerPositionText()
    //{
    //    playerPosition.text = "Position: x= " + transform.position.x.ToString("0.00") + " z= " + transform.position.z.ToString("0.00");

    //}

    //private void SetCountText()
    //{
    //    scoreText.text = "Score: " + count.ToString();
    //    if (count >= numPickUps)
    //    {
    //        winText.text = "You Win";
    //    }

    //}

}
