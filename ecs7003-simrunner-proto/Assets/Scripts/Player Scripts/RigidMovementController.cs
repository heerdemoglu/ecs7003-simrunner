using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO.MemoryMappedFiles;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;

public class RigidMovementController : MonoBehaviour
{
	// Some of the code is inspired by CharacterController used by Jammo-Character Asset Scripts:
	// This one uses Rigidbody instead of Character controller to utilize Physics of Unity Engine.
	[Header("Movement Parameters")]
	public float moveSensi; // sensitivity for movement
	public float jumpSensi; // sensitivity for jump
	public bool isGrounded = true; // single jump requires this to reset jump otherwise pressing space makes you fly
	public float RotateSpeed; // set animation of rotation
	public Animator anim; // animator used to utilize animations (used and modified assets from Jammo Player)
	public float bearingAngle; // important for animation and conservation of space

	public bool doubleJump; // ToDo
	public bool isWallRunning; 
	public Vector3 left, right, up, down, jump;
	Vector3 finPosition;

	//private bool isGameOver;
	public Text GameOver;

	// Inspired from JammoPlayer asset. Used along with animator to animate the object.
	[Header("Animation Smoothing")]
	[Range(0, 1f)]
	public float HorizontalAnimSmoothTime = 0.2f;
	[Range(0, 1f)]
	public float VerticalAnimTime = 0.2f;
	[Range(0, 1f)]
	public float StartAnimTime = 0.3f;
	[Range(0, 1f)]
	public float StopAnimTime = 0.15f;
	public float Speed;

	// For Jammo pre-coded methods: - May not work now.
	public float desiredRotationSpeed = 0.1f;
	public Vector3 desiredMoveDirection;
	public Camera cam;

	// used for input
	float xAxis;
	float yAxis; // not used right now
	float zAxis;

	Rigidbody rigidb;

	// performance optimizations
	int blendTreeHash;
	int isJumpingHash;


	// Use this for initialization
	void Start()
	{
		// Parameters that will be used throughout the project.
		anim = this.GetComponent<Animator>(); // Get the animator
		rigidb = this.GetComponent<Rigidbody>(); // Get the rigidbody

		// setting up hashes used
		blendTreeHash = Animator.StringToHash("Blend");
		isJumpingHash = Animator.StringToHash("isJumping");

		// initialise isJumping at start
		anim.SetBool(isJumpingHash, false);

		// get camera reference and lock cursor
		cam = Camera.main;
		Cursor.lockState = CursorLockMode.Locked;
	}


	void FixedUpdate()
    {
		// Get inputs, move player do animations and jumping every fixed time.
		RegisterInputs();
		MovePlayer();
		ApplyMoveAnimation();
		JumpPlayer();

		// track/map isGrounded to !isJumping on every frame
		if (isGrounded) anim.SetBool(isJumpingHash, false);

	}

	// Rotates player to look at a vector position
	public void LookAt(Vector3 pos)
	{
		if(pos == Vector3.zero) return;
		transform.rotation = Quaternion.Slerp(
			transform.rotation, 
			Quaternion.LookRotation(pos), 
			desiredRotationSpeed
		);
	}

	// Rotates player to camera
	// Used originally by JammoCharacter Asset; left here as it may come in handy later.
	public void RotateToCamera(Transform t)
	{

		var camera = Camera.main;
		var forward = cam.transform.forward;
		var right = cam.transform.right;

		desiredMoveDirection = forward;

		t.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
	}

	// set ui text if died
    private void OnTriggerEnter(Collider other)
    {
		if (other.gameObject.tag == "Death")
        {
			//isGameOver = true;
			GameOver.text = "YOU DIED";
		}

	}

	// reset grounded if touching floor or died
    private void OnCollisionEnter(Collision collision)
    {
		if (collision.gameObject.tag == "Floor" 
			|| collision.transform.CompareTag("Tiles") 
			|| collision.transform.CompareTag("Speed Up Wall") 
			|| collision.transform.CompareTag("Slow Down Wall")
			|| collision.transform.CompareTag("Wall Break") 
			|| collision.transform.CompareTag("Double Jump Wall"))
		{ 
			isGrounded = true; 
			if(collision.transform.CompareTag("Wall Break"))
            {
				collision.gameObject.SetActive(false);
            }
		}

		if (collision.gameObject.tag == "Win")
		{
			//isGameOver = true;
			isGrounded = true;
			GameOver.text = "YOU WIN!";
		}
	}

	// Register movement input
	public void RegisterInputs()
	{
		// Regular WASD direction as we start with y rotation at 0.
		xAxis = (float)(Input.GetAxis("Horizontal")); // left right //  * Math.Sin(bearingAngle)
	 	zAxis = (float)(Input.GetAxis("Vertical")); // up down // * Math.Cos(bearingAngle)

		left = Input.GetKey(KeyCode.A) ? Vector3.left : Vector3.zero;
		up = Input.GetKey(KeyCode.W) ? Vector3.forward : Vector3.zero;
		down = Input.GetKey(KeyCode.S) ? Vector3.back : Vector3.zero;
		right = Input.GetKey(KeyCode.D) ? Vector3.right : Vector3.zero;

		jump = Input.GetButton("Jump") ? new Vector3(0, 1, 0) : Vector3.zero;
	}

	// move
	public void MovePlayer()
	{
		// create move force after input is registered
		Vector3 MoveForce = (left + right + up + down) * moveSensi;
		
		// if there was any movement, store the end position
		if (MoveForce != Vector3.zero)
		{
			// store final position
			finPosition = MoveForce;
			// apply force to rigidbody
			rigidb.AddForce(MoveForce);
		}		
	}

	// animate movement
	public void ApplyMoveAnimation()
	{
		// set the speed based on axis'
		Speed = new Vector2(xAxis, zAxis).sqrMagnitude;

		// Apply the animation: 
		// ?
		if (rigidb.velocity != Vector3.zero)
		{
			anim.SetFloat(blendTreeHash, Speed, StartAnimTime, Time.deltaTime);
			//transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(MoveForce), Time.deltaTime * RotateSpeed);
			LookAt(finPosition);
		} else
        {
			anim.SetFloat(blendTreeHash, Speed, StopAnimTime, Time.deltaTime);
		}
	}

	// make jump
	public void JumpPlayer()
	{
		if (jump != Vector3.zero && isGrounded)
		{
			rigidb.AddForce(jump * jumpSensi, ForceMode.Impulse);
			anim.SetBool(isJumpingHash, true);
			isGrounded = false;
		}
	}
}
