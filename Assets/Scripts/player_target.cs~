using UnityEngine;
using System.Collections;

public class player_target : MonoBehaviour {

	public Vector3 target_pos = new Vector3(0,0,0);

	public GameObject selected;

	/// <summary>
	/// use movement sliding instead of setting position directly.
	/// </summary>
	public bool move_to_target = false; 

	/// <summary>
	/// the speed, in units per second, we want to move towards the target
	/// </summary>
	public float speed = 10.0f;

	/// <summary>
	/// temp var for retrieving mouse location.
	/// </summary>
	private Vector2 mouse_loc;

	void Start () 
	{
		Camera.main.ScreenToWorldPoint(Input.mousePosition);
		target_pos = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		mouse_loc = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		// Click!
		if (Input.GetMouseButtonDown (0)) 
		{

			RaycastHit2D hit = Physics2D.Raycast((mouse_loc), Vector2.zero);
			
			if (hit.collider != null)
			{
				selected = hit.collider.gameObject;
				Debug.Log ("Target Name: " + selected.name);
				Debug.Log ("Target Position: " + selected.transform.position);
				
				SetTarget(hit.collider.gameObject.transform.position);
			}
		}

		SetTarget(mouse_loc);

		if (target_pos != transform.position)
		{
			if (move_to_target)
			{
				MoveTowardsTarget(); // lerp to target coords
			}
			else
			{
				goToTarget(); // go directly to target coords
			}
		}

	}

	void SetTarget(Vector3 new_pos)
	{
		// sets the pointer's target
		if (new_pos != target_pos)
		{
			target_pos = new_pos;
		}
	}

	/// <summary>
	/// Teleport the reticle directly to target coords.
	/// </summary>
	void goToTarget()
	{
		transform.position = target_pos;
	}

	/// <summary>
	/// seek target coords at speed
	/// good for 'slow' lock on time
	/// </summary>
	void MoveTowardsTarget() 
	{
		Vector3 currentPosition = transform.position;

		//first, check to see if we're close enough to the target
		if(Vector3.Distance(currentPosition, target_pos) > .1f) 
		{ 
			Vector3 directionOfTravel = target_pos - currentPosition;

			//now normalize the direction, since we only want the direction information
			directionOfTravel.Normalize();

			//scale the movement on each axis by the directionOfTravel vector components
			transform.Translate(
				(directionOfTravel.x * speed * Time.deltaTime),
				(directionOfTravel.y * speed * Time.deltaTime),
				(directionOfTravel.z * speed * Time.deltaTime));
		}	
	}
}