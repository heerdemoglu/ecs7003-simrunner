using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public GameObject player;
	private Vector3 camOffset;

	public float speed = 2.0f;

	// Start is called before the first frame update
	void Start()
	{
		camOffset = new Vector3(player.transform.position.x-16f, player.transform.position.y+7f, player.transform.position.z+4);

		// Lock the cursor to the middle of the screen.
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update()
	{
		// Use Quarternions to rotate the camera:
		camOffset = Quaternion.AngleAxis(-Input.GetAxis("Mouse Y") * speed, Vector3.right) * camOffset;
		camOffset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * speed, Vector3.up) * camOffset;
		transform.position = player.transform.position + camOffset;
		transform.LookAt(player.transform.position);
	}

}