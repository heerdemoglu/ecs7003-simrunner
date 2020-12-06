using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Other implementation of player controller
 */
public class PlayerController : MonoBehaviour
{
    // speed
    [Range(1f, 4f)]
    public float acceleration ;

    // rotation speed
    [Range(2f, 4f)]
    public float rotationSpeed;

    //settings sliders
    public Slider accelerationSlider;
    public float intitialAcceleration = 2f;
    public Slider rotationSlider;
    public float intitialRotation = 2f;

    public float maxSlider = 4f;
    public float minSlider = 1f;


    // fields referencing other objects
    Animator animator;
    CharacterController controller;
    public AudioManager audioManager;
    public PlayerRotation playerRotator;

    // Vectors
    Vector3 horizontalMove = Vector3.zero;
    Vector3 verticalMove = Vector3.zero;
    Vector3 slippingMove = Vector3.zero;
    Vector3 combinedMovement = Vector3.zero;

    // private variables to track velocity values - sent to animator
    float velocityX = 0.0f;
    float velocityZ = 0.0f;
    float velocityY = 0.0f;

    public float Gravity = Physics.gravity.y;
    float deceleration;
    float maximumWalkVelocity;
    float maximumRunVelocity;
    float currentMaxVelocity;
    float jumpHeight = 5f;
    float clampingThreshold = 0.1f;
    float distanceToGround = 0f;

    // states
    bool forwardPressed, 
    backwardPressed,
    rightPressed, 
    leftPressed, 
    runPressed, 
    jumpPressed,
    isGrounded = false;

    // preformance boost - searching by int is faster than by String
    int VelocityZHash;
    int VelocityXHash;
    int VelocityYHash;
    int isJumpingHash;
    int isJumpAnticipPlayingHash;

    // Start is called before the first frame update
    void Start()
    {
        //Slider aSlider = accelerationSlider.GetComponent<Slider>();
        
        //start breathing sound and running sound
        audioManager = FindObjectOfType<AudioManager>();
        audioManager.Play("breathing", true);
        audioManager.Play("running", true);
        
        //SLIDERs
        accelerationSlider.maxValue = maxSlider;
        accelerationSlider.minValue = minSlider;
        accelerationSlider.value = intitialAcceleration;

        rotationSpeed = intitialRotation;
        rotationSlider.maxValue = maxSlider;
        rotationSlider.minValue = minSlider;
        rotationSlider.value = intitialRotation;

        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        playerRotator = GetComponent<PlayerRotation>();

        // set speeds
        deceleration = acceleration * 2f;
        maximumRunVelocity = acceleration;
        maximumWalkVelocity = maximumRunVelocity/2f;
        currentMaxVelocity = maximumWalkVelocity;

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
        distanceToGround = playerRotator.GetDistanceFromGround();//using the rotator object info
        isGrounded = distanceToGround < 0.15f;

        // Get the desired velocity addition values
        RecalculateVelocityX(leftPressed, rightPressed);
        RecalculateVelocityY(isGrounded, jumpPressed);
        RecalculateVelocityZ(backwardPressed, forwardPressed);

        Debug.Log(velocityZ);

        // Reset combined movement vector
        combinedMovement = Vector3.zero; // (0,0,0)


        // Set audio sounds
        audioManager.SetVolume("running", Mathf.Abs(velocityZ)/2f);
        audioManager.SetMute("running", !isGrounded);// Do this in statemanager instead


        // horizontal movement in Z direction
        MovePlayer(); // (xDir, 0, zDir)
        //rotate in X
        TankRotatePlayer();
        // jumping and gravity
        JumpAndGravity(); // (xDir, 0, zDir) --> (xDir, YDIR, zDir) ==> (xDir + jumpXProj, jumpYProj , zDir)  ::: Gravity -- (xDir + jumpXProj- gravXProj, jumpYProj - gravYProj , zDir) (IGNORE)
        // apply the combined movement vector
        combinedMovement = transform.TransformDirection(combinedMovement);
        controller.Move(combinedMovement * Time.deltaTime);

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
    void RecalculateVelocityX_deprecated(bool leftPressed, bool rightPressed)
    {
        // left accelerate
        if(leftPressed && velocityX > -rotationSpeed)
        {
            velocityX -= Time.deltaTime * acceleration;
        }
        // clamp left to max
        if(leftPressed && velocityX < -rotationSpeed)
        {
            // velocityX = -currentMaxVelocity/2f;
            velocityX += Time.deltaTime * deceleration;
        }
        // right accelerate
        if(rightPressed && velocityX < rotationSpeed)
        {
            velocityX += Time.deltaTime * acceleration;
        }
        // clamp left to max
        if(rightPressed && velocityX > rotationSpeed)
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

    void RecalculateVelocityX(bool leftPressed, bool rightPressed)
    {
        // left accelerate
        if(leftPressed && !rightPressed)
        {
            velocityX = -rotationSpeed;
        }
        else if(rightPressed && !leftPressed)
        {
            velocityX = rotationSpeed;
        }
        else {
            velocityX = 0f;
        }
    }
    // Player jumps/falls/slipping off a slope
    void RecalculateVelocityY(bool isGrounded, bool jumpPressed)
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
    // set new forward position
    void RecalculateVelocityZ(bool backwardPressed, bool forwardPressed)
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
        
        // clamp if no button pressed
        if(!backwardPressed && !forwardPressed && Mathf.Abs(velocityZ) < clampingThreshold)
        {
            velocityZ = 0f;
        }
    }



    // horizontal movement calculations
    void MovePlayer()
    {
        horizontalMove = new Vector3(0, 0, velocityZ * 3f);
        
        // Local to world space translation is required to move in correct direction:
        // This adds taken movement capability:
        //horizontalMove = transform.TransformDirection(horizontalMove);
        combinedMovement += horizontalMove;
    }
    void TankRotatePlayer()
    {
        transform.Rotate(0, -velocityX, 0);
    }
    // Jump and gravity, vertical movement calcualtions
    void JumpAndGravity()
    {
        verticalMove.y = velocityY;
        //verticalMove = transform.TransformDirection(verticalMove);

        combinedMovement += verticalMove;
    }




    public void adjustAcceleration(float sliderAcceleration)
    {
        acceleration = sliderAcceleration;

    }
    public void adjustRotation(float sliderRotation)
    {
        rotationSpeed = sliderRotation;
    }


    // Add
    void AssignAnimatorParameters()
    {
        animator.SetFloat(VelocityXHash, -velocityX);
        animator.SetFloat(VelocityZHash, velocityZ);
        animator.SetFloat(VelocityYHash, distanceToGround);
        animator.SetBool(isJumpingHash, !isGrounded);
    }
}
