using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.Profiling;
using UnityEngine;


public class RigidMovementController : MonoBehaviour
{

	[Header("Movement Parameters")]
	public float moveSensi;
	public float jumpSensi;
	public bool isGrounded = true;
	public float RotateSpeed;
	public Animator anim;
	public float bearingAngle; // important for animation and conservation of space

	public bool doubleJump; // ToDo
	public bool isWallRunning;

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
	Vector3 oldMove = new Vector3(0.01f, 0f, 0.01f);

	// For Jammo pre-coded methods: - May not work now.
	public float desiredRotationSpeed = 0.1f;
	public Vector3 desiredMoveDirection;
	public Camera cam;

	// Use this for initialization
	void Start()
	{
		anim = this.GetComponent<Animator>(); // Get the animator
		cam = Camera.main;
		Cursor.lockState = CursorLockMode.Locked;
	}


	void FixedUpdate()
    {

		// Regular WASD direction as we start with y rotation at 0.
		float xAxis = (float)(Input.GetAxis("Horizontal")); // left right //  * Math.Sin(bearingAngle)
		float zAxis = -(float)(Input.GetAxis("Vertical")); // up down // * Math.Cos(bearingAngle)

		/*
		if (transform.eulerAngles.y >= -45 || transform.eulerAngles.y < 45)
        {
			zAxis = (float)(Input.GetAxis("Horizontal")); // left right //  * Math.Sin(bearingAngle)
			xAxis = (float)(Input.GetAxis("Vertical")); // up down // * Math.Cos(bearingAngle)
		}

		if (transform.eulerAngles.y < -45 || transform.eulerAngles.y >= -135)
		{
			zAxis = -(float)(Input.GetAxis("Horizontal")); // left right //  * Math.Sin(bearingAngle)
			zAxis = (float)(Input.GetAxis("Vertical")); // up down // * Math.Cos(bearingAngle)
		}

		if(transform.eulerAngles.y < -135 || transform.eulerAngles.y > 135)
		{
			xAxis = -(float)(Input.GetAxis("Horizontal")); // left right //  * Math.Sin(bearingAngle)
			zAxis = -(float)(Input.GetAxis("Vertical")); // up down // * Math.Cos(bearingAngle)
		}
		*/

		Speed = new Vector2(xAxis, zAxis).sqrMagnitude;

		// Apply the movement wrt object rotation: (X-Z direction)
		Vector3 movement = (new Vector3(xAxis, 0, zAxis)).normalized;

		GetComponent<Rigidbody>().AddForce(movement * moveSensi * Time.fixedDeltaTime, ForceMode.Impulse);

		// Apply the animation: 
		if (transform.GetComponent<Rigidbody>().velocity != Vector3.zero)
		{

			anim.SetFloat("Blend", Speed, StartAnimTime, Time.deltaTime);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(oldMove), Time.deltaTime * RotateSpeed);


		} else
        {
			anim.SetFloat("Blend", Speed, StopAnimTime, Time.deltaTime);
		}

		// Apply Jump:
		if (Input.GetButton("Jump") && isGrounded)
		{
			GetComponent<Rigidbody>().AddForce(new Vector3(0, 1, 0) * jumpSensi, ForceMode.Impulse);
			isGrounded = false;
		}

		if (movement != Vector3.zero)
			oldMove = movement;

	}

	// Rotates player to look at a vector position
	public void LookAt(Vector3 pos)
	{
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), desiredRotationSpeed);
	}

	// Rotates player to camera
	public void RotateToCamera(Transform t)
	{

		var camera = Camera.main;
		var forward = cam.transform.forward;
		var right = cam.transform.right;

		desiredMoveDirection = forward;

		t.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
	}


	private void OnCollisionEnter(Collision collision)
    {
		if (collision.gameObject.tag == "Floor")
			isGrounded = true;
    }
}
