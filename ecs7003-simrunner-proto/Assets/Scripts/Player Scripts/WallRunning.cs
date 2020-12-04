using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to describe the wall running behavior of the player.
 * 
 * */
public class WallRunning : MonoBehaviour
{
    
    // Used to stick player to the walls 
    public float upForce;
    public float rightForce;

    // For use in animations; not perfected yet, hence commented out for the prototype.
    // The animation does not support rotation and as player rotates the movement actions may not work as intended.
    // Using model meshes cause unwanted rigidbody interactions.
    // public Transform body;
    Vector3 rot;
    CharacterController controller;

    // Used by wall checking - Determine which wall I connect at.
    private float distanceFromLeft;
    private float distanceFromRight;

    private float raycastLimit = 10f;

    // Use SerializeField to show these values on the Unity Inspector
    [SerializeField] private bool onLeftWall;
    [SerializeField] private bool onRightWall;
    [SerializeField] private bool isWallRunning;

    // Used to start WallRunning instance
    private void Awake()
    {
        // rot = GetComponent<Rigidbody>().transform.eulerAngles;
        controller = GetComponent<CharacterController>();
    }

    private void checkWall()
    {
        // Object has rays to its left and right. Uses these to determine which wall it connects to.
        // Maybe increasing the ray number can increase the sensitivity of the model connecting the walls.
        Ray rayLeft = new Ray(transform.position, -transform.right);
        Ray rayRight = new Ray(transform.position, transform.right);
        
        RaycastHit leftW;
        RaycastHit rightW;

        // If ray hits from the left
        if (Physics.Raycast(rayLeft, out leftW, raycastLimit))
        {
            // check distance set the corresponding boolean wall parameter
            distanceFromLeft = Vector3.Distance(transform.position, leftW.point);
            if (distanceFromLeft < 1f)
            {
                RunOnLeftWall();        
            }
            else
            {
                EndRunOnLeftWall();
            }
        }

        // Else same for the right. If connects the wall from both sides; breaks tie with the left.
        else if (Physics.Raycast(rayRight, out rightW, raycastLimit))
        {
            distanceFromRight = Vector3.Distance(transform.position, rightW.point);
            if (distanceFromRight < 1f)
            {
                RunOnRightWall();
            }
            else
            {
                EndRunOnRightWall();
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        // only check the boolean parameters as the rest will be handled by colliders only.
        checkWall();
    }

    private void RunOnLeftWall()
    {
        onLeftWall = true;
        onRightWall = false;
    }

    private void EndRunOnLeftWall()
    {
        onLeftWall = false;
    }
    
    private void RunOnRightWall()
    {
        onLeftWall = false;
        onRightWall = true;
    }

    private void EndRunOnRightWall()
    {
        onRightWall = false;
    }  

    public bool isOnLeftWall(){
        return this.onLeftWall;
    }  
  
    public bool isOnRightWall(){
        return this.onRightWall;
    }  
    
    // private void OnCollisionEnter(Collision collision)
    // {

    //     // If connected with a tile or a special tile set wall running to true.
    //     if (collision.transform.CompareTag("Tiles") 
    //         || collision.transform.CompareTag("Speed Up Wall") 
    //         || collision.transform.CompareTag("Slow Down Wall")
    //         || collision.transform.CompareTag("Wall Break") 
    //         || collision.transform.CompareTag("Double Jump Wall"))
    //     {
    //         isWallRunning = true;
    //     }
    // }

    // private void OnCollisionStay(Collision collision)
    // {
    //     // could also do with isWallRunning = true; this is too verbose.
    //     if(collision.transform.CompareTag("Tiles") 
    //         || collision.transform.CompareTag("Speed Up Wall") 
    //         || collision.transform.CompareTag("Slow Down Wall")
    //         || collision.transform.CompareTag("Wall Break") 
    //         || collision.transform.CompareTag("Double Jump Wall"))
    //     {

    //         // ROTATIONS ARE DISABLED FOR NOW, at specific scenarios they are broken, otherwise working.
    //         //transform.rotation = Quaternion.FromToRotation(GetComponent<Rigidbody>().transform.eulerAngles, collision.contacts[0].normal);
            
    //         // According to the wall; apply some force that moves you forward and sticks you to the surface (gravity still effects you)
    //         if (onLeftWall) {
    //             RunOnLeftWall();
    //         }

    //         if (onRightWall)
    //         {
    //             RunOnRightWall();
    //         }
    //     }
    // }

    // private void OnCollisionExit(Collision collision)
    // {
    //     // if you exit wall return back to original running condition
    //     if (collision.transform.CompareTag("Tiles") 
    //         && collision.transform.CompareTag("Speed Up Wall") 
    //         && collision.transform.CompareTag("Slow Down Wall")
    //         && collision.transform.CompareTag("Wall Break") 
    //         && collision.transform.CompareTag("Double Jump Wall"))
    //     { 
    //         isWallRunning = false;
    //         // Rotation animation disabled again due to the reason told above.
    //         //transform.rotation = Quaternion.FromToRotation(GetComponent<Rigidbody>().transform.eulerAngles, rot);
    //     } 
    // }

    // private void RunOnLeftWall()
    // {
    //     GetComponent<Rigidbody>().AddForce(Vector3.up * upForce, ForceMode.Impulse);
    //     GetComponent<Rigidbody>().AddForce(transform.right * rightForce, ForceMode.Impulse);
    // }
    
    // private void RunOnRightWall()
    // {
    //     GetComponent<Rigidbody>().AddForce(Vector3.up * upForce, ForceMode.Impulse);
    //     GetComponent<Rigidbody>().AddForce(-transform.right * rightForce, ForceMode.Impulse);
    // }
}
