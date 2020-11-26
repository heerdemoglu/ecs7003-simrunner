using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Other implementation of player controller, it doesnt use WASD, directly uses Horizontal and Vertical Inputs.
 * It requires clamping setting fixed speed etc. Not used in the final product.
 */
public class PlayerController : MonoBehaviour
{
    // speed
    [Range(1f, 4f)]
    public float acceleration = 2.0f;
    // rotation speed
    [Range(1f, 4f)]
    public float rotationSpeed = 0.3f;
    //gravity
    public float Gravity = Physics.gravity.y;
    // jump speed
    [Range(1f, 4f)]
    public float jumpHeight = 5.0f;
    public GameObject raycastReference;

    // private fields for referencing other objects
    Animator animator;
    CharacterController controller;
    Vector3 moveDirection;
    Vector3 verticalDirection;

    // private variables to track velocity values - sent to animator
    float velocityX = 0.0f;
    float velocityZ = 0.0f;
    float velocityY = 0.0f;
    float currentMaxVelocity = 0.5f;
    float maximumWalkVelocity = 0.5f;
    float maximumRunVelocity = 2.0f;
    float clampingThreshold = 0.05f;

    // states
    bool forwardPressed, 
    rightPressed, 
    leftPressed, 
    runPressed, 
    jumpPressed = false;
    // bool isJumping = true;
    bool isGrounded = false;

    // preformance boost - searching by int is faster than by String
    int VelocityZHash;
    int VelocityXHash;
    int VelocityYHash;
    int isJumpingHash;
    int isJumpAnticipPlayingHash;
    // bool isJumpAnticipPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        moveDirection = Vector3.zero;

        // set speeds
        maximumRunVelocity = acceleration;
        maximumWalkVelocity = maximumRunVelocity/4f;
        currentMaxVelocity = maximumWalkVelocity;

        // set hashes
        VelocityZHash = Animator.StringToHash("Velocity Z");
        VelocityXHash = Animator.StringToHash("Velocity X");
        VelocityYHash = Animator.StringToHash("Velocity Y");
        isJumpingHash = Animator.StringToHash("isJumping");
        // isJumpAnticipPlayingHash = Animator.StringToHash("isJumpAnticipPlaying");
    }

    // Update is called once per frame
    void Update()
    {
        RegisterUserInputs();
        currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;

        // determine vertical position
        float distanceToGround = GetRaycastDistance();
        isGrounded = distanceToGround < 0.1f;
        
        // jumping
        JumpPlayer();

        // forward movement
        RecalculateVelocityZ();
        MovePlayer();
        
        // update facing direction
        RecalculateVelocityX();
        RotatePlayer();

        // apply movement
        moveDirection = transform.TransformDirection(moveDirection);
        verticalDirection.y = velocityY;
        controller.Move((moveDirection+verticalDirection) * Time.deltaTime);
                
        // assign values to animator parameters
        animator.SetFloat(VelocityXHash, velocityX);
        animator.SetFloat(VelocityZHash, velocityZ);
        animator.SetFloat(VelocityYHash, distanceToGround);
        animator.SetBool(isJumpingHash, !isGrounded);
        // animator.SetBool(isJumpAnticipPlayingHash, false);
    }

    // get input from user and set local variables - true if pressed
    void RegisterUserInputs()
    {
        forwardPressed = Input.GetKey(KeyCode.W);
        rightPressed = Input.GetKey(KeyCode.A);
        leftPressed = Input.GetKey(KeyCode.D);
        runPressed = Input.GetKey(KeyCode.LeftShift);
        jumpPressed = Input.GetKey(KeyCode.Space);
    }

    //set new x position
    void RecalculateVelocityX()
    {
        // left accelerate
        if(leftPressed && velocityX > -currentMaxVelocity )
        {
            velocityX -= Time.deltaTime * acceleration;
        }
        // clamp left to max
        if(leftPressed && runPressed && velocityX < -currentMaxVelocity)
        {
            velocityX = -currentMaxVelocity;
        }
        // right accelerate
        if(rightPressed && velocityX < currentMaxVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
        }
        // clamp left to max
        if(rightPressed && runPressed && velocityX > currentMaxVelocity)
        {
            velocityX = currentMaxVelocity;
        }
        // decelerate
        // note the difference!
        if(!leftPressed && !rightPressed)
        {
            // deceleration from left (turning back from left)
            if(velocityX < 0.0f)
            {
                velocityX += Time.deltaTime * acceleration;
            }
            // deceleration from right (turning back from right)
            if(velocityX > 0.0f)
            {
                velocityX -= Time.deltaTime * acceleration;
            }
            // clamp to min
            if(velocityX != 0.0f && (velocityX > -clampingThreshold && velocityX < clampingThreshold))
            {
                velocityX = 0.0f;
            }
        }
    }

    // // set new forward position
    void RecalculateVelocityZ()
    {
        // if forward key pressed, increase velocity in z direction
        if(forwardPressed && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration;
        }
        // clamp to min
        if(!forwardPressed && velocityZ < 0.0f)
        {
            velocityZ = 0.0f;
        }
        // clamp forward to max
        if(forwardPressed && runPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ = currentMaxVelocity;
        }

        // deceleration from forward
        if(!forwardPressed && velocityZ > 0.0f)
        {
            velocityZ -= Time.deltaTime * acceleration;
        }
        // decelerate
        else if(forwardPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * acceleration;
            // round to max if we are close
            if(velocityZ > currentMaxVelocity && velocityZ < (currentMaxVelocity + clampingThreshold)){
                velocityZ = currentMaxVelocity;
            }
        }
        // round to max if we are close to max
        else if(forwardPressed && velocityZ < currentMaxVelocity 
        && velocityZ > (currentMaxVelocity - clampingThreshold))
        {
            velocityZ = currentMaxVelocity;
        }
    }

    // set vertical position
    void RecalculateYPosition()
    {
        // velocityY = rigidb.position.y;
    }

    // move
    void MovePlayer()
    {
        moveDirection = new Vector3(0, 0, velocityZ * 3f);
    }

    // Rotate player to face look direction
    void RotatePlayer()
    {
        transform.Rotate(0, -velocityX * rotationSpeed, 0);
    }

    // // // Player jumps
    void JumpPlayer()
    {   
        // JUMP make player jump or fall
        if(isGrounded && !jumpPressed)
        {
            //there is always a small force pulling character to the ground
            velocityY = Gravity * Time.deltaTime;
        }
        else if(isGrounded && jumpPressed) {
            velocityY = jumpHeight;
        }
        else
        {
            velocityY += Gravity * Time.deltaTime;
        }
    }

    // // track if the player is Grounded
    bool RaycastToGround()
    {
        // float DistanceToTheGround = GetComponent<Collider>().bounds.extents.y;
        return Physics.Raycast(raycastReference.transform.position, Vector3.down, 0.1f);
    }

    float GetRaycastDistance()
    {
        RaycastHit hit;
        Ray downRay = new Ray(raycastReference.transform.position, -Vector3.up);
        if (Physics.Raycast(downRay, out hit))
        {
            return hit.distance;
        }
        return 10f;
    }


        // // JUMP make player jump
        // // NEW approach
        // if (isGrounded && jumpPressed && !isJumping && !isJumpAnticipPlaying){
        //     isJumpAnticipPlaying = true;
        //     animator.SetBool(isJumpAnticipPlayingHash, true);
        //     // 2. play jumpAnticipation() - trigger it in animator?

        //     //      - have a separate blend tree for jumpAnticipation?
        //     //      - or blend the existing tree with the antip anim            
        //     //  3. have an event in that animation that calls the jumpPlayer function here?
        //     //      - only at this point, this function increases the velocityY
        // }
}
