using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// PizzaNode scrolling script
// assign all child scripts as scrolling background items to a camera

// References:
// better: http://pixelnest.io/tutorials/2d-game-unity/parallax-scrolling/
// multidirectional: https://gist.github.com/Valryon/7547513

public class scrolling_script : MonoBehaviour {

	public bool isLinkedToCamera = false; // Moves the camera to follow background movement.
	public Camera camera_link; // link to the camera to follow

	public bool isLinkedToPlayer = true; // scrolls bkgrnd according to player speed
	private GameObject link_player; // link to the player to follow

	public bool isLooping = true; // Background is infinite

	public Vector2 direction= new Vector2(-1,0); // Scrolling Direction
	public Vector2 speed = new Vector2(2.0f, 2.0f); // Scrolling Speed

	private Vector2 repeatableSize;

	/// <summary>
	/// 2 - List of children with a renderer.
	/// </summary>
	private List<Transform> backgroundPart;

	void Start()
	{
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

			if (backgroundPart.Count == 0)
			{
				Debug.LogError("No background objects to scroll!");
			}

			// Sort by position.
			// Note: Get the children from left to right.
			// We would need to add a few conditions to handle
			// all the possible scrolling directions.
			backgroundPart = backgroundPart.OrderBy(t => t.position.x * (-1 * direction.x)).ThenBy(t => t.position.y * (-1 * direction.y)).ToList();
				
			// Get the size of the repeatable parts
			Transform first = backgroundPart.First();
			Transform last = backgroundPart.Last();

			repeatableSize = new Vector2(
				Mathf.Abs(last.position.x - first.position.x),
				Mathf.Abs(last.position.y - first.position.y)
				);
		}

		if (isLinkedToPlayer == true)
		{
			link_player = GameObject.Find("Player");
		}


	}

	void Update()
	{
		Vector3 movement;
		
			// Movement
		if (isLinkedToPlayer == true)
		{
			speed.x = link_player.rigidbody2D.velocity.x;
			speed.y = link_player.rigidbody2D.velocity.y;

			direction = link_player.rigidbody2D.velocity;
			direction.Normalize();

			movement = new Vector3(
				link_player.rigidbody2D.velocity.x,
				link_player.rigidbody2D.velocity.y,
				0);
		}
		else
		{
			movement = new Vector3(
			speed.x * direction.x,
			speed.y * direction.y,
			0);
		}

		movement *= Time.deltaTime;

		// moves the entire object
		transform.Translate(movement);

		// Move the camera
		if (isLinkedToCamera)
		{
			Camera.main.transform.Translate(movement);
			// camera_link.transform.Translate(movement); // this is for use with a linked camera
			// transform.Translate(movement); // this works! 
		}

		// 4 - Loop
		if (isLooping)
		{
			//---------------------------------------------------------------------------------
			// 2 - Check if the object is before, in or after the camera bounds
			//---------------------------------------------------------------------------------
			
			// Camera borders
			var dist = (transform.position - camera_link.transform.position).z;

			float leftBorder = camera_link.ViewportToWorldPoint(new Vector3(0, 0, dist)).x;
			float rightBorder = camera_link.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;
			// float width = Mathf.Abs(rightBorder - leftBorder);

			var topBorder = camera_link.ViewportToWorldPoint(new Vector3(0, 0, dist)).y;
			var bottomBorder = camera_link.ViewportToWorldPoint(new Vector3(0, 1, dist)).y;
			// float height = Mathf.Abs(topBorder - bottomBorder);
			
			// Determine entry and exit border using direction
			Vector3 exitBorder = Vector3.zero;
			Vector3 entryBorder = Vector3.zero;
			
			if (direction.x < 0)
			{
				exitBorder.x = leftBorder;
				entryBorder.x = rightBorder;
			}
			else if (direction.x > 0)
			{
				exitBorder.x = rightBorder;
				entryBorder.x = leftBorder;
			}
			
			if (direction.y < 0)
			{
				exitBorder.y = bottomBorder;
				entryBorder.y = topBorder;
			}
			else if (direction.y > 0)
			{
				exitBorder.y = topBorder;
				entryBorder.y = bottomBorder;
			}

			// Get the first object.
			// The list is ordered from left (x position) to right.
			Transform firstChild = backgroundPart.FirstOrDefault();
			
			if (firstChild != null)
			{
				bool checkVisible = false;

				// Check if the child is already (partly) before the camera.
				// Test the position first because the IsVisibleFrom method is heavy

				// check border depending on direction
				if (direction.x != 0)
				{
					if ((direction.x < 0 && (firstChild.position.x < exitBorder.x))
					    || (direction.x > 0 && (firstChild.position.x > exitBorder.x)))
					{
						checkVisible = true;
					}
				}
				if (direction.y != 0)
				{
					if ((direction.y < 0 && (firstChild.position.y < exitBorder.y))
					    || (direction.y > 0 && (firstChild.position.y > exitBorder.y)))
					{
						checkVisible = true;
					}
				}

				if (checkVisible == true)
				{
					//---------------------------------------------------------------------------------
					// 3 - The object was in the camera bounds but isn't anymore.
					// -- We need to recycle it
					// -- That means he was the first, he's now the last
					// -- And we physically moves him to the further position possible
					//---------------------------------------------------------------------------------

					// If the child is already on the left of the camera,
					// we test if it's completely outside and needs to be
					// recycled.
					if (firstChild.renderer.IsVisibleFrom(camera_link) == false)
					{
						// Set position in the end
						firstChild.position = new Vector3(
							firstChild.position.x + ((repeatableSize.x + firstChild.renderer.bounds.size.x) * -1 * direction.x),
							firstChild.position.y + ((repeatableSize.y + firstChild.renderer.bounds.size.y) * -1 * direction.y),
							firstChild.position.z
							);
						
						// The first part become the last one
						backgroundPart.Remove(firstChild);
						backgroundPart.Add(firstChild);
					}
				} // end checkVisible
			} // end firstchild != null
		} // end isLooping
	} // end Update
} // end Class
