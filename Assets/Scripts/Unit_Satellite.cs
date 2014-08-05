using UnityEngine;
using System.Collections;

// Reference: http://answers.unity3d.com/questions/463704/smooth-orbit-round-object-with-adjustable-orbit-ra.html

public class Unit_Satellite : MonoBehaviour {

	public Transform Target; // target to rotate about.
	public Vector3 rotationAxis = Vector3.forward; // axis to rotate about.

	public float radius = 2.0f;
	public float Speed = 1.0f; // how fast target seeks orbit position
	public float rotationSpeed = 80.0f; // how fast target rotates around central point
	public float TargetRangeTolerance = 1.0f; // range at which a ship is considered 'at' the target location

	public bool showDebugLines = true;
	public bool useStartDistanceAsRadius = true; // converts existing distance into radius - means no lerping at start!
	public bool initialOrbitSnap = true; // Snap to radius value instead of using orbit calculated from editor position.

	private Transform OrbitTarget; // The position on orbit this target is always aiming for.

	// Use this for initialization
	void Start ()
	{
		// Set the initial orbit target to correct location
		OrbitTarget = transform;

		// initial snap to orbit target position?
		if (initialOrbitSnap)
		{
			OrbitTarget.position = 
			transform.position = OrbitTarget.position;
		}

		if (!Target)
		{
			Target = GameObject.Find("player_limit").transform; // Default Target object - the center of the player's sphere of motion.
		}

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (CheckTargetInRange())
		{
			OrbitAround(); // rotate the orbit target to next position
		}
		else
		{
			MoveTowardsOrbitTarget(); 
		}
		//OrbitTarget.position = (transform.position - Target.position).normalized * radius + Target.position;

		// Debug drawing
		if (showDebugLines)
		{
			DrawDebugLines();
			Debug.DrawLine(Target.position, OrbitTarget.position, Color.blue);
		}

	}

	void OrbitAround()
	{
		OrbitTarget.RotateAround(Target.position, rotationAxis, rotationSpeed * Time.deltaTime);
	}

	void MoveTowardsOrbitTarget()
	{
		transform.position = Vector3.MoveTowards(transform.position, OrbitTarget.position, Time.deltaTime * Speed);
	}

	bool CheckTargetInRange()
	{
		// Added condition to check if target was at the proper orbital range
		if (Vector3.Distance(transform.position, OrbitTarget.position) < TargetRangeTolerance)
		{
			return true;
		}

		return false;
	}

	void DrawDebugLines()
	{
		Debug.DrawRay(transform.position, OrbitTarget.position, Color.cyan);
	}
}
