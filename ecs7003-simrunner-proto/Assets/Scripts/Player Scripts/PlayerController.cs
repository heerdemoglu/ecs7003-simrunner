using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// enum of all possible movement states
public enum Status { 
    idle, idleTurnright, idleTurnLeft, 
    walking, walkingBackwards, 
    running, wallRunning, 
    jumping, falling, 
    landingOnNewWall, landingOnSameWall,
    dies 
}

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
    bool isMovementLocked = false;

    // preformance boost - searching by int is faster than by String
    int VelocityZHash;
    int VelocityXHash;
    int VelocityYHash;
    int isJumpingHash;
    int isJumpAnticipPlayingHash;
    int isLandingNewWallHash;

    //status
    Status status;

    // Start is called before the first frame update
    void Start()
    {
        //Slider aSlider = accelerationSlider.GetComponent<Slider>();

        status = Status.falling;//initially falling
        
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
        isLandingNewWallHash = Animator.StringToHash("isLandingNewWall");
    }

    // Update is called once per frame
    void Update()
    {   
        // Checks for input from user
        RegisterUserInputs();
        // Checks character distance from closest wallpiece and sets isGrounded
        RecalculateVerticalDistanceInfo();
        // Calculate the current velocity additions
        RecalculateVelocities(leftPressed, rightPressed, isGrounded, jumpPressed, backwardPressed, forwardPressed);
        // Update status
        UpdateStatus();        
        // Set audio sounds
        UpdateStateDependentFeatures(status);
        // Move the character
        UpdateMovement();
        // Assign values to animator parameters
        AssignAnimatorParameters();
    }

    /* ******************* INPUT ****************************************** */

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

    /* ******************* STATUS ****************************************** */

    // Switch between the enum player states
    void UpdateStatus()
    {
        if(!isGrounded && velocityY> 0f)
            status = Status.jumping;
        else if(!isGrounded && velocityY < 0f)
        {
            //landing on new wall
            if(distanceToGround < 1f)
            {
                if(playerRotator.DidSwitchWall()) 
                    status = Status.landingOnNewWall;
                else status = Status.landingOnSameWall;
            }
            else status = Status.falling;
            //landing on same wall
        }
        else if(velocityZ > maximumWalkVelocity)
            status = Status.running;
        else if(velocityZ > 0f)
            status = Status.walking;
        else if(velocityZ < 0f)
            status = Status.walkingBackwards;
        else
            status = Status.idle;
    }

    // updating the status dependent features
    void UpdateStateDependentFeatures(Status status){
        Debug.Log(status);
        switch(status)
        {
            case Status.idle:
                break;
            case Status.idleTurnright:
                break;
            case Status.idleTurnLeft:
                break;
            case Status.walking:
                break;
            case Status.walkingBackwards:
                break;
            case Status.running:
                break;
            case Status.wallRunning:
                break;
            case Status.jumping:
                break;
            case Status.falling:
                break;
            case Status.landingOnNewWall:
                break;
            case Status.landingOnSameWall:
                break;
            case Status.dies:
                break;
            default:
                Debug.Log("this shouldn't happen");
                break;
        }

        audioManager.SetVolume("running", Mathf.Abs(velocityZ)/2f);
        audioManager.SetMute("running", !isGrounded);// Do this in statemanager instead
    }

    /* ******************* POSITION RECALCULATIONS *************************** */
    void RecalculateVerticalDistanceInfo()
    {
        distanceToGround = playerRotator.GetDistanceFromGround();//using the rotator object info
        isGrounded = distanceToGround < 0.3f;
    }

    void RecalculateVelocities(bool leftPressed, bool rightPressed,bool isGrounded, bool jumpPressed,bool backwardPressed, bool forwardPressed)
    {
        //set the speed limit
        currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;

        // Get the desired velocity addition values
        RecalculateVelocityX_deprecated(leftPressed, rightPressed);
        RecalculateVelocityY(isGrounded, jumpPressed);
        RecalculateVelocityZ(backwardPressed, forwardPressed);
    }
    // set new x position
    void RecalculateVelocityX(bool leftPressed, bool rightPressed)
    {
        if(isMovementLocked)
            velocityX = 0f;
        // left accelerate
        else if(leftPressed && !rightPressed)
            velocityX = -rotationSpeed;
        else if(rightPressed && !leftPressed)
            velocityX = rotationSpeed;
        else
            velocityX = 0f;
    }
    // Player jumps/falls/slipping off a slope
    void RecalculateVelocityY(bool isGrounded, bool jumpPressed)
    {   
        // JUMP make player jump or fall
        if(isGrounded && !jumpPressed)
            velocityY = 0f;
        else if(isGrounded && jumpPressed) {
            velocityY = jumpHeight;
        }
        else
            velocityY += Gravity * Time.deltaTime;
    }
    // set new forward position
    void RecalculateVelocityZ(bool backwardPressed, bool forwardPressed)
    {
        if(isMovementLocked){
            velocityZ = 0f;
            return;
        }
        if(isGrounded && status == Status.landingOnNewWall) 
            return;
        // early exit if nothing is pressed and standing still
        if(!backwardPressed && !forwardPressed && velocityZ == 0.0f)
            return;
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
    void RecalculateVelocityX_deprecated(bool leftPressed, bool rightPressed)
    {
        if(isMovementLocked){
            velocityX = 0f;
            return;
        }
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

    //update the lock status of the movement
    public void SetMovementLocked(bool shouldLock)
    {
        isMovementLocked = shouldLock;
    }

    /* ******************* MOVEMENT ****************************************** */

    // physically moving the character
    void UpdateMovement()
    {
        // Reset combined movement vector
        combinedMovement = Vector3.zero; // (0,0,0)
        // horizontal movement in Z direction
        MovePlayer(); // (xDir, 0, zDir)
        //rotate in X
        TankRotatePlayer();
        // jumping and gravity
        JumpAndGravity(); // (xDir, 0, zDir) --> (xDir, YDIR, zDir) ==> (xDir + jumpXProj, jumpYProj , zDir)  ::: Gravity -- (xDir + jumpXProj- gravXProj, jumpYProj - gravYProj , zDir) (IGNORE)
        // apply the combined movement vector
        combinedMovement = transform.TransformDirection(combinedMovement);
        controller.Move(combinedMovement * Time.deltaTime);
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
    // 
    void TankRotatePlayer()
    {
        transform.Rotate(0, -velocityX, 0);
    }
    // Jump and gravity, vertical movement calcualtions
    void JumpAndGravity()
    {
        verticalMove.y = velocityY;
        combinedMovement += verticalMove;
    }


    /* ******************* USER ADJUSTMENTS ********************************** */

    public void adjustAcceleration(float sliderAcceleration)
    {
        acceleration = sliderAcceleration;

    }
    public void adjustRotation(float sliderRotation)
    {
        rotationSpeed = sliderRotation;
    }

    /* ******************* ANIMATION ****************************************** */

    // Assign animation parameters to the animator component
    void AssignAnimatorParameters()
    {
        animator.SetFloat(VelocityXHash, -velocityX);
        animator.SetFloat(VelocityZHash, velocityZ);
        animator.SetFloat(VelocityYHash, distanceToGround);
        animator.SetBool(isJumpingHash, !isGrounded);
        animator.SetBool(isLandingNewWallHash, status == Status.landingOnNewWall);
    }

    // //collision
    // void OnControllerColliderHit(ControllerColliderHit hit)
    // {
    //     Debug.Log("hey");
    // }
}
