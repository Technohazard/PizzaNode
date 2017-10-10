using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// PizzaNode scrolling script
// assign all child scripts as scrolling background items to a camera

// References:
// better: http://pixelnest.io/tutorials/2d-game-unity/parallax-scrolling/
// multidirectional: https://gist.github.com/Valryon/7547513

public class BackgroundScroller : MonoBehaviour {

	public bool FollowCamera = false; // Moves background to follow camera.
	public Camera camera; // link to the camera to follow

	/// <summary>
	/// Use Target as reference?
	/// </summary>
	public bool FollowingTarget = true; 
	
	// Scroll background in relation to a target.
	public GameObject Target; 
	private Rigidbody2D targetRigidBody2D;

	/// <summary>
	/// Background  infinite
	/// </summary>
	public bool isLooping = true;
	
	public Vector2 Velocity;
		public float Scale; 
	
		/// <summary>
		/// Scale factors for additional child bgs.
		/// should be one for each child otherwise it auto-calcs values with programmer fudge factor
		/// </summary>
		public float[] BGScaleArray;

	// Calculation variables, etc.
	private Vector2 repeatableSize;
	
	/// <summary>
	/// List of child background objects the scroller controls.
	/// </summary>
	private List<GameObject> BackgroundList = new List<GameObject> ();
	private Dictionary<GameObject, float> BGScaleDict = new Dictionary<GameObject, float>();

	void Start()
	{
		if (isLooping) 
		{
			RegisterBackgroundChildren ();
		}

		if (FollowingTarget == true)
		{
			if (Target == null) 
			{
				Debug.Log ("<color=yellow>Warning:</color> no Target selected to follow.");
				RegisterPlayer ();
			}

						if (Target != null)
						{
							targetRigidBody2D = Target.GetComponent<Rigidbody2D> ();
						}
		}
	}

	/// <summary>
	/// Get all the appropriate children as backgrounds.
	/// </summary>
	private void RegisterBackgroundChildren ()
	{
				// TODO make this recursive?
				foreach(Transform child in transform) 
				{

				// add only active children
				if (child.gameObject.activeSelf) 
				{
					BackgroundList.Add (child.gameObject);
				}
			}

			if (BackgroundList.Count == 0) 
			{
					Debug.Log ("<color = yellow>WARN:</color> No background child objects found!");
						return;
			}

			// Populate dictionary with computed float scale values based on total # of bgs.
			int i = 0;
			foreach(GameObject obj in BackgroundList)
			{
					if ((i > BGScaleArray.Length) || (i < 0))
					{
							continue;
					}

					BGScaleDict.Add (obj, i * Scale);
					i++;
			}
	}

	public void SortBackgroundList()
	{
		// Sort by position.
		// Note: Get the children from left to right.
		// We would need to add a few conditions to handle
		// all the possible scrolling directions.
		BackgroundList = BackgroundList.OrderBy (t => t.transform.position.x * (-1 * Velocity.x))
										.ThenBy (t => t.transform.position.y * (-1 * Velocity.y))
										.ToList ();

		// Get the size of the repeatable parts
		GameObject first = BackgroundList.First ();
		GameObject last = BackgroundList.Last ();

		repeatableSize = new Vector2 (
						Mathf.Abs (last.transform.position.x - first.transform.position.x),
						Mathf.Abs (last.transform.position.y - first.transform.position.y));
	}

	void Update()
	{
		// Update the velocity from target's velocity, if appropriate
		if (FollowingTarget == true)
		{
				if (Target != null)
				{
					Velocity = targetRigidBody2D.velocity.normalized * Scale;
				}
		}

		// Translate this object with base scale.	
		ScaledMovement (Velocity, Scale, gameObject);
		
		int i = 0;
		float tmpScale; 
		// Translate all children by their base scaling.
		foreach(GameObject obj in BackgroundList)
		{
			if (BGScaleDict.TryGetValue(obj, out tmpScale))
			{
				ScaledMovement (Velocity, tmpScale, obj);				
			}
			else
			{
				// couldn't retrieve a scale from the dict for this obj.
				// calculate a janky one from scale and depth.
				ScaledMovement (Velocity, Scale * (2^i), obj);	
			}

			i++;
		}

		// Camera follows this object?
		if (FollowCamera)
		{
			camera.transform.position = transform.position;
		}

		// 4 - Loop
		if (isLooping)
		{
			//---------------------------------------------------------------------------------
			// 2 - Check if the object is before, in or after the camera bounds
			//---------------------------------------------------------------------------------
			
			// Camera borders
			var dist = (transform.position - camera.transform.position).z;

			float leftBorder = camera.ViewportToWorldPoint(new Vector3(0, 0, dist)).x;
			float rightBorder = camera.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;
			// float width = Mathf.Abs(rightBorder - leftBorder);

			var topBorder = camera.ViewportToWorldPoint(new Vector3(0, 0, dist)).y;
			var bottomBorder = camera.ViewportToWorldPoint(new Vector3(0, 1, dist)).y;
			// float height = Mathf.Abs(topBorder - bottomBorder);
			
			// Determine entry and exit border using direction
			Vector3 exitBorder = Vector3.zero;
			Vector3 entryBorder = Vector3.zero;
			
			if (Velocity.x < 0)
			{
				exitBorder.x = leftBorder;
				entryBorder.x = rightBorder;
			}
			else if (Velocity.x > 0)
			{
				exitBorder.x = rightBorder;
				entryBorder.x = leftBorder;
			}
			
			if (Velocity.y < 0)
			{
				exitBorder.y = bottomBorder;
				entryBorder.y = topBorder;
			}
			else if (Velocity.y > 0)
			{
				exitBorder.y = topBorder;
				entryBorder.y = bottomBorder;
			}

			// Get the first object.
			// The list is ordered from left (x position) to right.
			GameObject firstChild = BackgroundList.FirstOrDefault();
			Renderer firstChildRenderer = firstChild.GetComponent<Renderer> ();

			if (firstChild != null)
			{
				bool checkVisible = false;

				// Check if the child is already (partly) before the camera.
				// Test the position first because the IsVisibleFrom method is heavy

				// check border depending on direction
				if (Velocity.x != 0)
				{
					if ((Velocity.x < 0 && (firstChild.transform.position.x < exitBorder.x))
				    || (Velocity.x > 0 && (firstChild.transform.position.x > exitBorder.x)))
					{
						checkVisible = true;
					}
				}
				if (Velocity.y != 0)
				{
					if ((Velocity.y < 0 && (firstChild.transform.position.y < exitBorder.y))
					|| (Velocity.y > 0 && (firstChild.transform.position.y > exitBorder.y)))
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
					if (firstChildRenderer.IsVisibleFrom(camera) == false)
					{
						// Set position in the end
						firstChild.transform.position = new Vector3(
						firstChild.transform.position.x + ((repeatableSize.x + firstChildRenderer.bounds.size.x) * -1 * Velocity.x),
						firstChild.transform.position.y + ((repeatableSize.y + firstChildRenderer.bounds.size.y) * -1 * Velocity.y),
						firstChild.transform.position.z
							);
						
						// The first part become the last one
						BackgroundList.Remove(firstChild);
						BackgroundList.Add(firstChild);
					}
				} // end checkVisible
			} // end firstchild != null
		} // end isLooping
	} // end Update

	/// <summary>
	/// Translate obj by baseVelocity * baseScale and deltaTime
	/// </summary>
	/// <param name="baseVelocity">Base velocity.</param>
	/// <param name="baseScale">Base scale.</param>
	/// <param name="obj">Object.</param>
	private void ScaledMovement(Vector2 baseVelocity, float baseScale, GameObject obj)
	{
		Vector2 movement;

		movement = baseVelocity * baseScale * Time.deltaTime;

		obj.transform.Translate(movement);
	}
	
	/// <summary>
	/// Set Target to Player GameObject by any means necessary.
	/// </summary>
	private void RegisterPlayer()
	{
			if (Target == null) 
			{
						Player ply = GameObject.FindObjectOfType<Player> ();
						if (ply != null)
						{
								Target = ply.gameObject;
						}
			}

			if (Target == null)
			{
					Target = FindPlayerByName ("Player_Unit");
			}

			if (Target == null)
			{
					Debug.Log ("No Player found!");
			}
	}

	/// <summary>
	/// Attempt to find and return the Player's GameObject by string name.
	/// </summary>
	/// <returns>The player by name.</returns>
	/// <param name="playerName">Player name.</param>
	public GameObject FindPlayerByName(string playerName = "Player_Unit")
	{
		GameObject retObj = GameObject.Find (playerName);

				if (retObj == null)
		{
				Debug.Log(string.Format("<color=orange><null><color>: Couldn't find player by name '{0}'", playerName));
		}

		return retObj;
	}
} // end Class
