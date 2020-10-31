using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Security.Cryptography;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public GameObject player;
	private Vector3 offset;
    
	private float sensitivity = 0.01f;

	private float yaw = 0;
	private float pitch = 0;

	float rotX = 0.0f;
	float rotY = 0.0f;


	void Start()
	{

		// Lock the cursor to the middle of the screen.
		Cursor.lockState = CursorLockMode.Locked;

		// Set camera to follow the player (distance btwn camera and player)
		// Set camera to follow the player (distance btwn camera and player)
		offset = transform.position - player.transform.position; // (back(+) ,down(-), right(-))
		rotX = transform.eulerAngles.x;
		rotY = transform.eulerAngles.y;
	}


	void Update()
	{

		float xAxis = Input.GetAxis("Mouse X"); //0,11, best clamp angles
		float yAxis = Input.GetAxis("Mouse Y"); // 262, 277, best clamp angles

		yaw += sensitivity * xAxis;
		pitch += sensitivity * yAxis;

		rotY += yaw;
		rotX -= pitch;

		rotX = moduloAngleClamp(rotX, 20, 30);
		rotY = moduloAngleClamp(rotY, 250, 290);

		transform.rotation = Quaternion.Euler(rotX, rotY, 0);

		// Use Quarternions to rotate the camera:
		transform.position = player.transform.position + offset;
		//transform.LookAt(player.transform.position);

	}


	public static float moduloAngleClamp(float angle, float min, float max)
	{
		if (angle < -360f)
			angle += 360f;
		if (angle > 360f)
			angle -= 360f;
		return Mathf.Clamp(angle, min, max);
	}
}