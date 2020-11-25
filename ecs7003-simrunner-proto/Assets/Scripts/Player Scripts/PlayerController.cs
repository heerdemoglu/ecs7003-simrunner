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
    public float rotationSpeed = 1.0f;
    //gravity
    public float Gravity = Physics.gravity.y;
    // jump speed
    [Range(1f, 4f)]
    public float jumpForce = 10.0f;
    public bool isJumping = true;
    public bool isGrounded = false;

    // private fields for referencing other objects
    Animator animator;
    // Vector3 currentEulerAngles;
    // Quaternion currentRotation;

    // private variables to track velocity values - sent to animator
    float velocityX = 0.0f;
    float velocityZ = 0.0f;
    float velocityY = 0.0f;

    // preformance boost - searching by int is faster than by String
    int VelocityZHash;
    int VelocityXHash;
    int VelocityYHash;
    int isJumpingHash;
    int isJumpAnticipPlayingHash;

    float currentMaxVelocity = 0.5f;
    float maximumWalkVelocity = 0.5f;
    float maximumRunVelocity = 2.0f;
    // float maxJumpVelocity = 0.5f;
    float clampingThreshold = 0.05f;
    bool forwardPressed, rightPressed, leftPressed, runPressed, jumpPressed = false;
    // bool isJumpAnticipPlaying = false;

    private CharacterController controller;
    private Vector3 moveDirection;
    private Vector3 verticalDirection;
    public GameObject raycastReference;

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

        // forward movement
        RecalculateVelocityZ();
        MovePlayer();
        
        // update facing direction
        RecalculateVelocityX();
        RotatePlayer();
        
        // jumping
        JumpPlayer();

        // apply movement
        moveDirection = transform.TransformDirection(moveDirection);
        verticalDirection.y = velocityY;
        controller.Move((moveDirection+verticalDirection) * Time.deltaTime);
                
        // assign values to animator parameters
        animator.SetFloat(VelocityXHash, velocityX);
        animator.SetFloat(VelocityZHash, velocityZ);
        animator.SetFloat(VelocityYHash, GetRaycastDistance());
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
        moveDirection = new Vector3(0, 0, velocityZ * 4f);
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
        if(controller.isGrounded) 
        {
            //there is always a small force pulling character to the ground
            // velocityY = Gravity * Time.deltaTime;
            velocityY = 0f;

            if(jumpPressed) {
                velocityY = 5f;
                animator.SetBool(isJumpingHash, true);
            }
        }
        else
        {
            velocityY += Gravity * Time.deltaTime;

            if(velocityY < 0f){
                // falling
            }
            else if(velocityY >0f){
                animator.SetBool(isJumpingHash, true);
            }
        }

        animator.SetBool(isJumpingHash, true);
        // animator.SetBool(isJumpAnticipPlayingHash, false);
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
        return -1f;
    }


    // ARCHIVE
    // Rotate player to face look direction
    // void RotatePlayer()
    // {
    //     float rotAmount = rotationSpeed * velocityX * Time.deltaTime;
    //     currentEulerAngles -= new Vector3(0, rotAmount, 0);
    //     currentRotation.eulerAngles = currentEulerAngles;
    //     transform.rotation = currentRotation;
    // }
        // //Player moves
    // void MovePlayer()
    // {
    //      rigidb.MovePosition(
    //             transform.position 
    //             + transform.forward * velocityZ 
    //             * acceleration * 1.5f * Time.deltaTime
    //         );
    // }


            // MOVE and ROTATE
        // RecalculateXPosition();
        // if(velocityX != 0.0f) 
        //     RotatePlayer();
        // RecalculateZPosition();
        
        // apply movements
        // Vector3 move = new Vector3(velocityX, velocityY, velocityZ);
        // if(velocityZ != 0.0f)
        //     controller.Move(move * Time.deltaTime * acceleration);

        // if (move != Vector3.zero)
        //     transform.forward = move;

        // // set gravity
        // _velocity.y += Gravity * Time.deltaTime;
        // controller.Move(_velocity * Time.deltaTime);



        // RegisterUserInputs();
        // currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;
        
        // isGrounded = RaycastToGround();

        // // MOVE & ROTATE calculate current velocities
        // RecalculateXPosition();
        // if(velocityX != 0.0f) RotatePlayer();
        // RecalculateZPosition();
        // if(velocityZ != 0.0f) MovePlayer();

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
