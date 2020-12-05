using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PC_v2 : MonoBehaviour
{
    [Header("Speed & Gravity Settings:")]
    [Range(2f, 4f)] public float acceleration = 2.5f;
    [SerializeField] private float gravity = Physics.gravity.y;

    [Header("Character States:")]
    [SerializeField] private bool isWallrunning = false;
    [SerializeField] private bool isOnSlope = false;

    [Header("Setup: Game Objects and Animators:")]
    public GameObject raycastReference;
    public GameObject myMesh;
    public Text distanceText;
    public Text velocityText;
    public Text neVelText;

    // Vectors:
    Vector3 horizontalMove = Vector3.zero;
    Vector3 verticalMove = Vector3.zero;
    Vector3 slippingMove = Vector3.zero;
    Vector3 combinedMovement = Vector3.zero;

    // Private Variables for tracking Velocities:
    float velocityX, velocityY, velocityZ = 0.0f;
    float deceleration;
    float maximumWalkVelocity;
    float maximumRunVelocity;
    float currentMaxVelocity;
    float rotationVelocity;
    float jumpHeight;
    float clampingThreshold = 0.05f;

    // Private fields for referencing other objects:
    Animator animator;
    CharacterController controller;
    // WallRunning wallRunningObject;

    // All user inputs:
    private bool forwardPressed,
    backwardPressed,
    rightPressed,
    leftPressed,
    runPressed,
    jumpPressed = false;

    // All character states:
    private bool isGrounded,
    wallRunRight,
    hasCollided,
    wallRunLeft = false;

    // Performance boost - searching by int is faster than by String:
    private int VelocityZHash, VelocityXHash, VelocityYHash;
    int isJumpingHash, isJumpAnticipPlayingHash;

    // Other Variables:
    private float groundSlopeAngle = 0f;
    private float sideFriction = 0.1f;
    private float distanceToGround = 0f;
    private float distance;
    private int platLayer;
    private RaycastHit rayHit;

    void Start()
    {
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
        maximumWalkVelocity = maximumRunVelocity / 2f;
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
        // Get user inputs and set the maximum velocity according to the button combination pressed
        RegisterUserInputs();
        currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity; RegisterUserInputs();
        currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;

        // Reset combined movement vector - Each update is independent from the past
        combinedMovement = Vector3.zero;
        /*
        // If not touching any surface - IE on air: 
        if(!hasCollided)
        {
            // Check if there is a surface in proximity:
            if (!CheckProximity())
            {
                // determine vertical position
                RaycastHit rayCastHit = RaycastDownwards();
                isGrounded = distanceToGround < 0.15f;
                // horizontal movements
                MovePlayer();
                // jumping and gravity
                JumpAndGravity();
                // apply the combined movement vector
                controller.Move(combinedMovement * Time.deltaTime);
            }
        }*/
    }

    // Gets input from user and set local variables - true if pressed
    void RegisterUserInputs()
    {
        forwardPressed = Input.GetKey(KeyCode.W);
        backwardPressed = Input.GetKey(KeyCode.S);
        rightPressed = Input.GetKey(KeyCode.A);
        leftPressed = Input.GetKey(KeyCode.D);
        runPressed = Input.GetKey(KeyCode.LeftShift);
        jumpPressed = Input.GetKey(KeyCode.Space);
    }

    // ###################################################################################
    // Calculating X, Y, Z movements:
    // set new x position
    void RecalculateVelocityX()
    {
        // left accelerate
        if (leftPressed && velocityX > -rotationVelocity)
        {
            velocityX -= Time.deltaTime * acceleration;
        }
        // clamp left to max
        if (leftPressed && velocityX < -rotationVelocity)
        {
            // velocityX = -currentMaxVelocity/2f;
            velocityX += Time.deltaTime * deceleration;
        }
        // right accelerate
        if (rightPressed && velocityX < rotationVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
        }
        // clamp left to max
        if (rightPressed && velocityX > rotationVelocity)
        {
            // velocityX = currentMaxVelocity/2f;
            velocityX -= Time.deltaTime * deceleration;
        }
        // decelerate
        // note the difference!
        if (!leftPressed && !rightPressed)
        {
            // deceleration from left (turning back from left)
            if (velocityX < 0.0f)
            {
                velocityX += Time.deltaTime * deceleration;
            }
            // deceleration from right (turning back from right)
            if (velocityX > 0.0f)
            {
                velocityX -= Time.deltaTime * deceleration;
            }
            // clamp to min
            if (velocityX != 0.0f && (velocityX > -clampingThreshold && velocityX < clampingThreshold))
            {
                velocityX = 0.0f;
            }
        }
    }

    // Player jumps/falls/slipping off a slope
    void RecalculateVelocityY()
    {
        // JUMP make player jump or fall
        if (isGrounded && !jumpPressed)
        {
            //there is always a small force pulling character to the ground
            // velocityY = Gravity * Time.deltaTime;
            velocityY = 0f;
        }
        else if (isGrounded && jumpPressed)
        {
            velocityY = jumpHeight;
            // velocityY = Mathf.Lerp(0, jumpHeight, Time.deltaTime);
        }
        else
        {
            velocityY += gravity * Time.deltaTime;
        }
    }

    // set new forward position
    void RecalculateVelocityZ()
    {
        // early exit if nothing is pressed and standing still
        if (!backwardPressed && !forwardPressed && velocityZ == 0.0f)
            return;

        // // add this if you want to prevent inAir acceleration/deceleration
        // if(!isGrounded)
        //     return;

        // if forward key pressed, increase velocity in z direction
        if (forwardPressed && velocityZ < currentMaxVelocity)
            velocityZ += Time.deltaTime * acceleration;

        // deceleration from forward
        if (!forwardPressed && velocityZ > 0.0f)
            velocityZ -= Time.deltaTime * deceleration;

        // decelerate from a higher speed than what's allowed
        if (forwardPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * acceleration;
            // round to max (from above) if we are close to threshold
            if (velocityZ < (currentMaxVelocity + clampingThreshold))
                velocityZ = currentMaxVelocity;

        }

        // clamp forward to max (when running)
        if (forwardPressed && runPressed && velocityZ > currentMaxVelocity)
            velocityZ = currentMaxVelocity;

        // round to max if we are close to max
        if (forwardPressed && velocityZ < currentMaxVelocity
        && velocityZ > (currentMaxVelocity - clampingThreshold))
            velocityZ = currentMaxVelocity;

        // walk backwards
        if (backwardPressed && velocityZ <= 0.0f)
        {
            if (velocityZ > -maximumWalkVelocity)
            {
                velocityZ -= Time.deltaTime * acceleration;
            }
            if (velocityZ <= -maximumWalkVelocity)
            {
                velocityZ = -maximumWalkVelocity;
            }
        }
        // decelerate from walk backwards
        if (!backwardPressed && velocityZ < 0.0f)
        {
            velocityZ += Time.deltaTime * deceleration;
        }

    }

    // Combine movement using velocities calculated: (X,Z only)
    void MovePlayer()
    {
        RecalculateVelocityZ();
        horizontalMove = new Vector3(0, 0, velocityZ * 3f);

        RecalculateVelocityX();
        transform.Rotate(0, -velocityX, 0);

        horizontalMove = transform.TransformDirection(horizontalMove);
        combinedMovement += horizontalMove;
    }

    // Combine movement using velocities calculated: (Y only)
    void JumpAndGravity()
    {
        RecalculateVelocityY();
        verticalMove.y = velocityY;
        combinedMovement += verticalMove;
    }

    // Check the proximity to see if you are close to any object.
    bool CheckProximity()
    {
        RaycastHit hit;

        // Bottom of controller. Slightly above ground so it doesn't bump into slanted platforms. 
        // (Adjust to your needs)

        Vector3 p1 = transform.position + Vector3.up * 0.25f;
        
        //Top of controller
        Vector3 p2 = p1 + Vector3.up * controller.height;

        //Check around the character in a 360, 10 times (increase if more accuracy is needed)
        for (int i = 0; i < 360; i += 36)
        {
            //Check if anything with the platform layer touches this object
            if (Physics.CapsuleCast(p1, p2, 0, new Vector3(Mathf.Cos(i), 0, Mathf.Sin(i)), out hit, distance, 1 << platLayer))
            {
                //If the object is touched by a platform, move the object away from it
                controller.Move(hit.normal * (distance - hit.distance));
                hasCollided = true;
                Debug.Log("wallrunning activated");
                rayHit = hit;
                return true;
            }
        }
        rayHit = new RaycastHit();
        return false;
    }
    // ###################################################################################



    // ###################################################################################
    // Wallrun Mechanics :

    // ###################################################################################









}

