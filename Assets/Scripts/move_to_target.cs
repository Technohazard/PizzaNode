using UnityEngine;
using System.Collections;

public class move_to_target : MonoBehaviour {

	public Vector3 target_stored_position = new Vector3(0,0,0);
	public Vector3 directionOfTravel = Vector3.zero; // always starts inert

	public enum movement_type {dumb, target, seeker};
	// dumb = pick a direction at start and move in that direction until collide / death
	// target = check to see if bullet has reached target - explode when there!
	// seeker = continually update target position and seek it

	public movement_type bullet_movement = movement_type.dumb; // defaults to dumb

	public float speed = 10.0f;
	public GameObject selected;

	// Use this for initialization
	void Start () 
	{
		if (selected != null)
		{
			// set target to selected target's position
			target_stored_position = selected.transform.position;

			// Target selected
			switch (bullet_movement)
			{
				case movement_type.dumb:
					getDirectionToTarget();
					break;

				case movement_type.target:
					getDirectionToTarget();
					break;

				case movement_type.seeker:
					getDirectionToTarget();
					break;
			}
		}
		else
		{
			// No Target selected
			switch (bullet_movement)
			{
			case movement_type.dumb:
				// no target selected
				// bullet *should* continue to travel in whatever direction is set in Unity editor
				break;

			case movement_type.target:
				Debug.Log ("Bullet missing Target (targeted)");
				directionOfTravel = Vector3.zero;
				break;

			case movement_type.seeker:
				Debug.Log ("Bullet missing Target (seeker)");
				directionOfTravel = Vector3.zero;
				break;
			}
		}
	} // end Start()

	// Update is called once per frame
	void Update () 
	{
		switch (bullet_movement)
		{
			case movement_type.dumb:
				MoveDirection();
				break;

			case movement_type.target:
				MoveTowardsTarget();
				break;

			case movement_type.seeker:
				MoveSeekTarget();
				break;
		}
	}

	void MoveDirection()
	{
	// Update movement based only on direction
		//scale the movement on each axis by the directionOfTravel vector components
		transform.Translate(
			(directionOfTravel.x * speed * Time.deltaTime),
			(directionOfTravel.y * speed * Time.deltaTime),
			(directionOfTravel.z * speed * Time.deltaTime));

	}

	void MoveTowardsTarget() 
	{	
		//check to see if we're close enough to the target
		if(Vector3.Distance(transform.position, target_stored_position) > .1f) 
		{ 
			// calculate direction based on target location
			directionOfTravel = target_stored_position - transform.position;
			
			//now normalize the direction, since we only want the direction information
			directionOfTravel.Normalize();
			
			//scale the movement on each axis by the directionOfTravel vector components
			transform.Translate(
				(directionOfTravel.x * speed * Time.deltaTime),
				(directionOfTravel.y * speed * Time.deltaTime),
				(directionOfTravel.z * speed * Time.deltaTime));
			
			// Space.World
		}
		else
		{
			// arrived at target!
			ArriveAtTarget();
		}

	}

	void MoveSeekTarget()
	{
		// continually update target location 
		target_stored_position = selected.transform.position;
		
		getDirectionToTarget();

		MoveTowardsTarget();

	}

	public void SetTarget(Vector3 targetPosition)
	{
		// sets the pointer's target
		if (targetPosition != target_stored_position)
		{
			// assign new target 
			target_stored_position = targetPosition;

			// recalculate direction for new target
			getDirectionToTarget();
		}
	}

	void ArriveAtTarget()
	{
		// ex: if (animation exists) { switch to play animation; destroy after animation length;}

		if (gameObject.tag == "bullet")
		{
			gameObject.GetComponent<bullet_01>().impact();
		}
		else if (gameObject.tag == "sauce")
		{
			// SPLAT!
			gameObject.GetComponent<SauceBlob>().Splat();
		}
		else
		{
			//nothing... for now!
		}
	}

	void getDirectionToTarget()
	{
		// calculate direction based on target location
		directionOfTravel = target_stored_position - transform.position;
		
		//now normalize the direction, since we only want the direction information
		directionOfTravel.Normalize();
	}
}
