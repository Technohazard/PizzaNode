using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// PizzaNode scrolling script
// assign a scrolling background item to a camera

// better: http://pixelnest.io/tutorials/2d-game-unity/parallax-scrolling/

public class scrolling_script : MonoBehaviour {

	public Vector2 start_speed = new Vector2(2,2);
	public Vector2 start_direction = new Vector2(-1,0);

	public bool isLinkedToCamera = false; // Moves the camera to follow background movement.
	public bool offset_mode = false; // if true, scrolls the texture offset.
	public bool isLooping = true; // Background is infinite

	private Vector2 direction= new Vector2(0,0); // Scrolling Direction
	private Vector2 speed = new Vector2(0,0); // Scrolling Sepeed

	/// <summary>
	/// 2 - List of children with a renderer.
	/// </summary>
	private List<Transform> backgroundPart;

	void Start()
	{
		speed = start_speed;
		direction = start_direction;

		// For infinite background only
		if (isLooping)
		{
			// Get all the children of the layer with a renderer
			backgroundPart = new List<Transform>();
			
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				
				// Add only the visible children
				if (child.renderer != null)
				{
					backgroundPart.Add(child);
				}
			}
			
			// Sort by position.
			// Note: Get the children from left to right.
			// We would need to add a few conditions to handle
			// all the possible scrolling directions.
			backgroundPart = backgroundPart.OrderBy(
				t => t.position.x
				).ToList();
		}

	}

	void Update()
	{
		// Movement
		Vector3 movement = new Vector3(
			speed.x * direction.x,
			speed.y * direction.y,
			0);
		
		movement *= Time.deltaTime;

		// moves the entire object
		if (offset_mode == false)
		{
			transform.Translate(movement);
		}
		else
		{
			// transform the scrolling texture only.
			// NYI
		}

		// moves just the texture offset


		// Move the camera
		if (isLinkedToCamera)
		{
			Camera.main.transform.Translate(movement);
		}

		// 4 - Loop
		if (isLooping)
		{
			// Get the first object.
			// The list is ordered from left (x position) to right.
			Transform firstChild = backgroundPart.FirstOrDefault();
			
			if (firstChild != null)
			{
				// Check if the child is already (partly) before the camera.
				// We test the position first because the IsVisibleFrom
				// method is a bit heavier to execute.
				if (firstChild.position.x < Camera.main.transform.position.x)
				{
					// If the child is already on the left of the camera,
					// we test if it's completely outside and needs to be
					// recycled.
					if (firstChild.renderer.IsVisibleFrom(Camera.main) == false)
					{
						// Get the last child position.
						Transform lastChild = backgroundPart.LastOrDefault();
						Vector3 lastPosition = lastChild.transform.position;
						Vector3 lastSize = (lastChild.renderer.bounds.max - lastChild.renderer.bounds.min);
						
						// Set the position of the recyled one to be AFTER
						// the last child.
						// Note: Only work for horizontal scrolling currently.
						firstChild.position = new Vector3(lastPosition.x + lastSize.x, firstChild.position.y, firstChild.position.z);
						
						// Set the recycled child to the last position
						// of the backgroundPart list.
						backgroundPart.Remove(firstChild);
						backgroundPart.Add(firstChild);
					}
				}
			}
		}

	}

}