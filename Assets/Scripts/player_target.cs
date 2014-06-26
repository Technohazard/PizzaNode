using UnityEngine;
using System.Collections;

public class player_target : MonoBehaviour {

	public Vector3 target_stored_position = new Vector3(0,0,0);

	// Use this for initialization
	void Start () {
		Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

		if(hit.collider != null)
		{
			Debug.Log ("Target Name: " + hit.collider.gameObject.name);
			Debug.Log ("Target Position: " + hit.collider.gameObject.transform.position);
		}

		MoveTowardsTarget(hit.collider.gameObject.transform.position);
	}

	private void MoveTowardsTarget(Vector3 targetPosition) {
		//the speed, in units per second, we want to move towards the target
		float speed = 1;
		
		if (targetPosition != target_stored_position)
		{
			target_stored_position = targetPosition;
		}
		
		//move towards the center of the world (or where ever you like)
		
		Vector3 currentPosition = this.transform.position;
		//first, check to see if we're close enough to the target
		if(Vector3.Distance(currentPosition, targetPosition) > .1f) { 
			Vector3 directionOfTravel = targetPosition - currentPosition;
			//now normalize the direction, since we only want the direction information
			directionOfTravel.Normalize();
			//scale the movement on each axis by the directionOfTravel vector components
			
			this.transform.Translate(
				(directionOfTravel.x * speed * Time.deltaTime),
				(directionOfTravel.y * speed * Time.deltaTime),
				(directionOfTravel.z * speed * Time.deltaTime),
				Space.World);
		}	
	}
}
