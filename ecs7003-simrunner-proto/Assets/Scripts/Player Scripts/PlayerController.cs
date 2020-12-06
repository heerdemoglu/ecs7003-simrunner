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
    [Range(1f, 4f)]
    public float rotationSpeed;

    //settings sliders
    public Slider accelerationSlider;
    public float intitialAcceleration = 2f;
    public Slider rotationSlider;
    public float intitialRotation = 0.3f;

    public float maxSlider = 4f;
    public float minSlider = 1f;

    //gravity
    public float Gravity = Physics.gravity.y;

    public bool isWallrunning = false;
    public bool isOnSlope = false;
    
    public GameObject raycastReference;
    public Text distanceText;
    public Text velocityText;
    public Text neVelText;

    // private fields for referencing other objects
    Animator animator;
    CharacterController controller;
    // WallRunning wallRunningObject;

    // Vectors
    Vector3 horizontalMove = Vector3.zero;
    Vector3 verticalMove = Vector3.zero;
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
    jumpPressed,
    isGrounded,
    wallRunRight,
    hasCollided,
    wallRunLeft = false;

    // preformance boost - searching by int is faster than by String
    int VelocityZHash;
    int VelocityXHash;
    int VelocityYHash;
    int isJumpingHash;
    int isJumpAnticipPlayingHash;
    
    float groundSlopeAngle = 0f;
    float sideFriction = 0.1f;
    float distanceToGround = 0f;


    float distance;
    int platLayer;
    RaycastHit rayHit;
    public GameObject myMesh;
    public AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        //Slider aSlider = accelerationSlider.GetComponent<Slider>();
        
        //start breathing sound and running sound
        audioManager = FindObjectOfType<AudioManager>();
        audioManager.Play("breathing", true);
        audioManager.Play("running", true);

        
        accelerationSlider.maxValue = maxSlider;
        accelerationSlider.minValue = minSlider;
        accelerationSlider.value = intitialAcceleration;

        //rotationSpeed = intitialRotation;
        rotationSlider.maxValue = maxSlider;
        rotationSlider.minValue = minSlider;
        rotationSlider.value = intitialRotation;

        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        distance = controller.radius + 0.2f;
        //First add a Layer name to all platforms (I used MovingPlatform)
        //Now this script won't run on regular objects, only platforms.
        platLayer = LayerMask.NameToLayer("Ground");
        
        // wallRunningObject = (WallRunning)gameObject.GetComponent(typeof(WallRunning));

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
        combinedMovement = Vector3.zero; // (0,0,0)

        RecalculateVelocityX(leftPressed, rightPressed);
        RecalculateVelocityY(isGrounded, jumpPressed);
        RecalculateVelocityZ(backwardPressed, forwardPressed);

        Debug.Log(velocityZ);

        if(velocityZ > 0f) {
            audioManager.SetVolume("running", velocityZ/2f);
        }
        audioManager.SetMute("running", !isGrounded);

        // not wallrunning
        // if(!hasCollided)
        // {
        //     if(!TestThisShit())
        //     {
                Debug.Log("Not wallrunning");
                // determine vertical position
                RaycastHit rayCastHit = RaycastDownwards();
                isGrounded = distanceToGround < 0.15f;
                // horizontal movement - x and z
                MovePlayer(); // (xDir, 0, zDir)
                TankRotatePlayer();
                // jumping and gravity
                JumpAndGravity(); // (xDir, 0, zDir) --> (xDir, YDIR, zDir) ==> (xDir + jumpXProj, jumpYProj , zDir)  ::: Gravity -- (xDir + jumpXProj- gravXProj, jumpYProj - gravYProj , zDir) (IGNORE)
                // apply the combined movement vector
                combinedMovement = transform.TransformDirection(combinedMovement);
                controller.Move(combinedMovement * Time.deltaTime);
        //     }
        // }
        // // wallrunning
        // else
        // {
        //     Debug.Log("Wallrunning");
        //     GameObject wallThatWasHit = rayHit.transform.gameObject;
        //     Vector3 wallSurfaceVector = wallThatWasHit.transform.forward;
        //     // Debug.Log(wallThatWasHit.transform.forward);
        //     // Debug.DrawRay(rayHit.transform.position, wallThatWasHit.transform.forward, Color.green);
        //     controller.Move(wallSurfaceVector * velocityZ * 2f * Time.deltaTime);
        //     TankRotatePlayer();

        //     // velocityY = 0f;
        //     // distanceToGround = 0f;
        //     transform.rotation = Quaternion.LookRotation(wallSurfaceVector);
        //     myMesh.transform.rotation = Quaternion.FromToRotation(Vector3.up, rayHit.normal);
        //     transform.position 
        //         = new Vector3(transform.position.x-0.45f, transform.position.y, transform.position.z);
        //     // Debug.Log(myMesh.transform.rotation);
        //     // float smooth = 5.0f;
        //     // Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(wallSurfaceVector),  Time.deltaTime * smooth);
        //     // myMesh.transform.rotation.z = 90f;

        //     if(jumpPressed)
        //     {
        //         hasCollided = false;
        //     }
        // }

        // displaying text on UI
        DisplayUI();
        // assign values to animator parameters
        AssignAnimatorParameters();
    }

    // get the hit here if any 
    bool TestThisShit(){
        RaycastHit hit;
 
        //Bottom of controller. Slightly above ground so it doesn't bump into slanted platforms. (Adjust to your needs)
        Vector3 p1 = transform.position + Vector3.up * 0.1f;
        //Top of controller
        Vector3 p2 = p1 + Vector3.up * controller.height;
 
        //Check around the character in a 360, 10 times (increase if more accuracy is needed)
        for(int i=0; i<360; i+= 36){
            //Check if anything with the platform layer touches this object
            if (Physics.CapsuleCast(p1, p2, 0, new Vector3(Mathf.Cos(i), 0, Mathf.Sin(i)), out hit, distance, 1<<platLayer)){
                //If the object is touched by a platform, move the object away from it
                controller.Move(hit.normal*(distance-hit.distance));
                hasCollided = true;
                Debug.Log("wallrunning activated");
                rayHit = hit;
                return true;
            }
        }
        rayHit = new RaycastHit();
        return false;
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
    void RecalculateVelocityX(bool leftPressed, bool rightPressed)
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
