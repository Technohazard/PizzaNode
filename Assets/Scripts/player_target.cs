using UnityEngine;
using System.Collections;

public class player_target : MonoBehaviour {

	public Vector3 target_stored_position = new Vector3(0,0,0);

	public GameObject selected;

	//the speed, in units per second, we want to move towards the target
	public float speed = 10.0f;

	private Vector2 mouse_loc;

	void Start () {
		Camera.main.ScreenToWorldPoint(Input.mousePosition);
		target_stored_position = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		mouse_loc = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		if (Input.GetMouseButtonDown (0)) {

			RaycastHit2D hit = Physics2D.Raycast((mouse_loc), Vector2.zero);
			
			if(hit.collider != null)
			{
				selected = hit.collider.gameObject;
				Debug.Log ("Target Name: " + selected.name);
				Debug.Log ("Target Position: " + selected.transform.position);
				
				SetTarget(hit.collider.gameObject.transform.position);
			}
		}

		SetTarget(mouse_loc);
		MoveTowardsTarget();
	}

	void SetTarget(Vector3 targetPosition)
	{
		// sets the pointer's target
		if (targetPosition != target_stored_position)
		{
			target_stored_position = targetPosition;
		}
	}

	void MoveTowardsTarget() 
	{
		Vector3 currentPosition = transform.position;

		//first, check to see if we're close enough to the target
		if(Vector3.Distance(currentPosition, target_stored_position) > .1f) 
		{ 
			Vector3 directionOfTravel = target_stored_position - currentPosition;

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