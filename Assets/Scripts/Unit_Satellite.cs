using UnityEngine;
using System.Collections;

// Reference: http://answers.unity3d.com/questions/463704/smooth-orbit-round-object-with-adjustable-orbit-ra.html

public class Unit_Satellite : MonoBehaviour {

		public Transform Target; // target to rotate about.
		Vector3 Axis = Vector3.forward; // axis to rotate about.
		Vector3 target_center; // center of target transform

	public float radius = 2.0f;
	public float radiusSpeed = 1.0f;
	public float rotationSpeed = 80.0f;

	public bool Lock_Radius = true; 


	// Use this for initialization
	void Start ()
	{
		// Set the initial orbit position.
		transform.position = (transform.position - target_center).normalized * radius + target_center;
	}
	
	// Update is called once per frame
	void Update () 
	{
		target_center = Target.position;

		transform.RotateAround (target_center, Axis, rotationSpeed * Time.deltaTime);
		var desiredPosition = (transform.position - target_center).normalized * radius + target_center;
		transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * radiusSpeed);

	}
}
