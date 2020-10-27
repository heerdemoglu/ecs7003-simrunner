using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public GameObject player;
	private Vector3 offset;

	public float horizontalSpeed = 2.0F;
	public float verticalSpeed = 2.0F;

	// Start is called before the first frame update
	void Start()
	{
		offset = transform.position;
	}

	void LateUpdate()
	{
		transform.position = player.transform.position + offset;
	}

	void Update()
    {
		float h = horizontalSpeed * Input.GetAxis("Mouse X");
		float v = verticalSpeed * Input.GetAxis("Mouse Y");

		transform.Rotate(0, 0, 0); // not being used atm (v;h;0)
	}


}
