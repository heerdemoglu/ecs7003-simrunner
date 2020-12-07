using UnityEngine;
using System.Collections;

public class PlayerRotation : MonoBehaviour {

	RaycastHit previousHit;  //Historic previous hit
	RaycastHit hit;  //For Detect Sureface/Base.
	Vector3 surfaceNormal;  //The normal of the surface the ray hit.
	Vector3 forwardRelativeToSurfaceNormal;  //For Look Rotation
	GameObject currentWall;//Reference storing the wall the player is closest to
	GameObject closestWall;//Reference storing the wall the player is closest to

	// Update is called once per frame
	void Update () {
		closestWall = (hit.collider) ? hit.transform.gameObject : null;
		if(GetDistanceFromGround() < 0.3f) 
			currentWall = closestWall;
		else
			currentWall = null;

		if(closestWall != currentWall)
			Debug.Log("wall switched"+closestWall.GetInstanceID());

		// FindClosestObject();
		// if(hit.collider) CharacterFaceRelativeToSurface();

		// // OLD way
		CharacterFaceRelativeToSurface();
	}
	
	//Method For Correct Character Rotation According to Surface.
	private void CharacterFaceRelativeToSurface()
	{
		//For Detect The Base/Surface.
		if (Physics.Raycast(transform.position, -Vector3.up, out hit, 5))
		{
			surfaceNormal = hit.normal; // Assign the normal of the surface to surfaceNormal
			forwardRelativeToSurfaceNormal = Vector3.Cross(transform.right, surfaceNormal);
			Quaternion targetRotation = Quaternion.LookRotation(forwardRelativeToSurfaceNormal, surfaceNormal); //check For target Rotation.
			transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 3f); //Rotate Character accordingly.
		}
	}

	//Emitting a Raycast in 360 degree, we find the closest object (hit with sorthest distance)
	private void FindClosestObject()
	{
		RaycastHit closestHit = new RaycastHit();

		//Check around the character in a 360, 10 times (increase if more accuracy is needed)
        for(int i=0; i<360; i+= 36){
			RaycastHit newHit;
            //Check if anything with the platform layer touches this object
            if (Physics.Raycast(
					transform.position, 
					new Vector3(Mathf.Cos(i), 0, Mathf.Sin(i)), 
					out newHit, 5)
			){
				//keep the hit with the closest distance
				if(!closestHit.collider || newHit.distance < closestHit.distance) 
					closestHit = newHit;
            }
        }
		// previousHit = hit;
		hit = closestHit;
	}

	//Method For Correct Character Rotation According to Surface.
	private void CharacterFaceRelativeToSurface360()
	{
		surfaceNormal = hit.normal; // Assign the normal of the surface to surfaceNormal
		forwardRelativeToSurfaceNormal = Vector3.Cross(transform.right, surfaceNormal);
		Quaternion targetRotation = Quaternion.LookRotation(forwardRelativeToSurfaceNormal, surfaceNormal); //check For target Rotation.
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 3f); //Rotate Character accordingly.
	}

	//Method for supplying the hit distance - returns arbitrarily large float if no hit
	public float GetDistanceFromGround()
	{
		if(hit.collider){
			GameObject wall = hit.transform.gameObject;
			return PointToPlaneDistance(transform.position, wall.transform.position, surfaceNormal);
		}
		else return 1000f;//check for this in PlayerController
	}

	//Point to plane distance calculation
	private float PointToPlaneDistance(Vector3 pointPosition, Vector3 planePosition, Vector3 planeNormal)
	{
		float sb, sn, sd;
		
		sn = -Vector3.Dot(planeNormal, (pointPosition - planePosition));
		sd = Vector3.Dot(planeNormal, planeNormal);
		sb = sn / sd;
		
		Vector3 result = pointPosition + sb * planeNormal;
		return Vector3.Distance(pointPosition, result);
	}
}