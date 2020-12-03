using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Other implementation of player controller, it doesnt use WASD, directly uses Horizontal and Vertical Inputs.
 * It requires clamping setting fixed speed etc. Not used in the final product.
 */
public class PlayerController : MonoBehaviour
{
    // speed
    [Range(2f, 4f)]
    public float acceleration = 4.0f;
    //gravity
    public float Gravity = Physics.gravity.y;

    public GameObject raycastReference;
    public Text distanceText;
    public Text velocityText;
    public Text neVelText;

    // private fields for referencing other objects
    Animator animator;
    CharacterController controller;
    Vector3 horizontalMove;
    Vector3 verticalMove;
    Vector3 slippingMove = Vector3.zero;
    Vector3 combinedMovement = Vector3.zero;

    // private variables to track velocity values - sent to animator
    float velocityX = 0.0f;
    float velocityZ = 0.0f;
    float velocityY = 0.0f;

    float deceleration;
    float maximumWalkVelocity;
    float maximumRunVelocity;
    float currentMaxVelocity;
    float rotationVelocity;
    float jumpHeight;
    float clampingThreshold = 0.05f;

    // states
    bool forwardPressed, 
    backwardPressed,
    rightPressed, 
    leftPressed, 
    runPressed, 
    jumpPressed = false;
    // bool isJumping = true;
    bool isGrounded = false;
    public bool isWallrunning = false;
    public bool isOnSlope = false;

    // preformance boost - searching by int is faster than by String
    int VelocityZHash;
    int VelocityXHash;
    int VelocityYHash;
    int isJumpingHash;
    int isJumpAnticipPlayingHash;
    
    float groundSlopeAngle = 0f;
    float sideFriction = 0.1f;
    float distanceToGround = 0f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        horizontalMove = Vector3.zero;

        // set speeds
        deceleration = acceleration * 2f;
        maximumRunVelocity = acceleration;
        maximumWalkVelocity = maximumRunVelocity/2f;
        currentMaxVelocity = maximumWalkVelocity;
        rotationVelocity = 1f;
        jumpHeight = 5f;

        // set hashes
        VelocityZHash = Animator.StringToHash("Velocity Z");
        VelocityXHash = Animator.StringToHash("Velocity X");
        VelocityYHash = Animator.StringToHash("Velocity Y");
        isJumpingHash = Animator.StringToHash("isJumping");
    }

    // Update is called once per frame
    void Update()
    {   
        RegisterUserInputs();
        currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;
        

        // Reset combined movement vector
        combinedMovement = Vector3.zero;
        // determine vertical position
        RaycastHit rayCastHit = RaycastDownwards();
        isGrounded = distanceToGround < 0.15f;
        
        // horizontal movements
        MovePlayer();

        // jumping and gravity
        JumpAndGravity();

        isOnSlope = groundSlopeAngle < controller.slopeLimit && groundSlopeAngle >= 1f;
        isWallrunning = groundSlopeAngle >= controller.slopeLimit;
        if(isOnSlope) 
            OnSlopeMovements(rayCastHit.normal);
        else if (isWallrunning)
        {
            // wall run script?
            slippingMove = Vector3.zero;
            //
        }
        else 
            slippingMove = Vector3.zero;


        // apply the combined movement vector
        controller.Move(combinedMovement * Time.deltaTime);



        // displaying text on UI
        DisplayUI();
        // assign values to animator parameters
        AssignAnimatorParameters();
    }

    // get input from user and set local variables - true if pressed
    void RegisterUserInputs()
    {
        forwardPressed = Input.GetKey(KeyCode.W);
        backwardPressed = Input.GetKey(KeyCode.S);
        rightPressed = Input.GetKey(KeyCode.A);
        leftPressed = Input.GetKey(KeyCode.D);
        runPressed = Input.GetKey(KeyCode.LeftShift);
        jumpPressed = Input.GetKey(KeyCode.Space);
    }

    // set new x position
    void RecalculateVelocityX()
    {
        // left accelerate
        if(leftPressed && velocityX > -rotationVelocity)
        {
            velocityX -= Time.deltaTime * acceleration;
        }
        // clamp left to max
        if(leftPressed && velocityX < -rotationVelocity)
        {
            // velocityX = -currentMaxVelocity/2f;
            velocityX += Time.deltaTime * deceleration;
        }
        // right accelerate
        if(rightPressed && velocityX < rotationVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
        }
        // clamp left to max
        if(rightPressed && velocityX > rotationVelocity)
        {
            // velocityX = currentMaxVelocity/2f;
            velocityX -= Time.deltaTime * deceleration;
        }
        // decelerate
        // note the difference!
        if(!leftPressed && !rightPressed)
        {
            // deceleration from left (turning back from left)
            if(velocityX < 0.0f)
            {
                velocityX += Time.deltaTime * deceleration;
            }
            // deceleration from right (turning back from right)
            if(velocityX > 0.0f)
            {
                velocityX -= Time.deltaTime * deceleration;
            }
            // clamp to min
            if(velocityX != 0.0f && (velocityX > -clampingThreshold && velocityX < clampingThreshold))
            {
                velocityX = 0.0f;
            }
        }
    }

    // set new forward position
    void RecalculateVelocityZ()
    {
        // early exit if nothing is pressed and standing still
        if(!backwardPressed && !forwardPressed && velocityZ == 0.0f)
            return;

        // // add this if you want to prevent inAir acceleration/deceleration
        // if(!isGrounded)
        //     return;

        // if forward key pressed, increase velocity in z direction
        if(forwardPressed && velocityZ < currentMaxVelocity)
            velocityZ += Time.deltaTime * acceleration;

        // deceleration from forward
        if(!forwardPressed && velocityZ > 0.0f)
            velocityZ -= Time.deltaTime * deceleration;

        // decelerate from a higher speed than what's allowed
        if(forwardPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * acceleration;
            // round to max (from above) if we are close to threshold
            if(velocityZ < (currentMaxVelocity + clampingThreshold))
                velocityZ = currentMaxVelocity;

        }
        
        // clamp forward to max (when running)
        if(forwardPressed && runPressed && velocityZ > currentMaxVelocity)
            velocityZ = currentMaxVelocity;

        // round to max if we are close to max
        if(forwardPressed && velocityZ < currentMaxVelocity 
        && velocityZ > (currentMaxVelocity - clampingThreshold))
            velocityZ = currentMaxVelocity;

        // walk backwards
        if(backwardPressed && velocityZ <= 0.0f)
        {
            if(velocityZ > -maximumWalkVelocity)
            {
                velocityZ -= Time.deltaTime * acceleration;
            }
            if(velocityZ <= -maximumWalkVelocity) 
            {
                velocityZ = -maximumWalkVelocity;
            }
        }
        // decelerate from walk backwards
        if(!backwardPressed && velocityZ < 0.0f)
        {
            velocityZ += Time.deltaTime * deceleration;
        }

    }

    // Player jumps/falls/slipping off a slope
    void RecalculateVelocityY()
    {   
        // JUMP make player jump or fall
        if(isGrounded && !jumpPressed)
        {
            //there is always a small force pulling character to the ground
            // velocityY = Gravity * Time.deltaTime;
            velocityY = 0f;
        }
        else if(isGrounded && jumpPressed) {
            velocityY = jumpHeight;
            // velocityY = Mathf.Lerp(0, jumpHeight, Time.deltaTime);
        }
        else
        {
            velocityY += Gravity * Time.deltaTime;
        }
    }

    // horizontal movement calculations
    void MovePlayer()
    {
        RecalculateVelocityZ();
        horizontalMove = new Vector3(0, 0, velocityZ * 3f);

        RecalculateVelocityX();
        transform.Rotate(0, -velocityX, 0);
        
        horizontalMove = transform.TransformDirection(horizontalMove);
        combinedMovement += horizontalMove;
    }

    // Jump and gravity, vertical movement calcualtions
    void JumpAndGravity()
    {
        RecalculateVelocityY();
        verticalMove.y = velocityY;
        combinedMovement += verticalMove;
    }

    //
    void OnSlopeMovements(Vector3 normal)
    {
        // check if there is a slipping downwards force
        if(distanceToGround < 0.3f)
        {
            slippingMove.x += (1f - normal.y) * normal.x * (1f - sideFriction);
            slippingMove.z += (1f - normal.y) * normal.z * (1f - sideFriction);
            combinedMovement += slippingMove * Time.deltaTime * 30f;
        }
        // clearing the slipping movement
        else if (groundSlopeAngle < 10f || groundSlopeAngle > -10f)
        {
            slippingMove = Vector3.zero;
        }
    }

    //
    RaycastHit RaycastDownwards()
    {
        RaycastHit rayCastHit;
        Vector3 p1 = transform.position + controller.center;

        if(Physics.SphereCast(
            p1 + new Vector3(0,0.1f,0), 
            controller.height/2 , 
            Vector3.down, 
            out rayCastHit, 
            10)
        ){
            groundSlopeAngle = Vector3.Angle(rayCastHit.normal, Vector3.up);
            distanceToGround = rayCastHit.distance;
        }

        return rayCastHit;
    }

    //
    void DisplayUI()
    {
        distanceText.text = distanceToGround.ToString();;
        velocityText.text = velocityY.ToString();
        neVelText.text = groundSlopeAngle.ToString();
    }

    // 
    void AssignAnimatorParameters()
    {
        animator.SetFloat(VelocityXHash, -velocityX);
        animator.SetFloat(VelocityZHash, velocityZ);
        animator.SetFloat(VelocityYHash, distanceToGround);
        animator.SetBool(isJumpingHash, !isGrounded);
    }
}
