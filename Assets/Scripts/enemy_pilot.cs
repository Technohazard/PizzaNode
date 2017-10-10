using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class enemy_pilot : MonoBehaviour {
	// Flocking algorithm variables
	public float SeparationRange = 1.0f;
	public float DetectionRange = 2.0f;
	public CircleCollider2D sep_range_indicator;
	public CircleCollider2D det_range_indicator;

	// References to components.
	private Rigidbody2D _Rigidbody = null;

	public float speed = 1.0f;
	public float turn_speed = 1.0f;

	public Vector2 map_size = Vector2.zero; // initialized in Start() with screen size.

	public int maxTurnTheta; // maximum turn radius in degrees
	public int currentTheta; // orientation in degrees

	protected Color FlockColor; // Color identifier for flocking purposes

	public bool showRanges = false; // show detection ranges NYI because can't draw circles! (maybe w/ linerenderer?) :(

	public GameObject Target; // default target for this unit. computed in smart_target();
	public float mNavMinDistance = 1.5f; // distance at which the arrow stops nav'ing to point to the center of the ship

	private List<GameObject> WeaponList; 	// list of player weapon objects;
	private List<Weapon> WeaponCtrlList; 	// list of player weapon components;

	private int numweapons; 		// number of weapons on this ship, used to check for all weapons ready.

	public enum ai_states {
		stop, 
		face_shoot, 
		teleporter,
		flock
		};

	public ai_states unit_state = ai_states.stop;

	// stop = do nothing
	// face_player = rotate to track player position
	// teleport = randomly teleports around the map
	// flock = Obey flocking rules.

	public float TeleportRadius = 10;
	public float TeleportChargeTime = 5;
	public bool TeleportToTarget = false; // if true, teleport to within radius of player location, not just "away from self"
	private float teleportTimer = 0.0f;

	// Use this for initialization
	void Start () 
	{
		Target = SmartTarget("Player"); // Target the player by default
		RegisterWeapons();

		map_size = new Vector2(Screen.width, Screen.height); // 800x600

		SetupAI ();


		// Register Components
		_Rigidbody = GetComponent<Rigidbody2D>();

	}

	// Update is called once per frame
	void Update () 
	{
		// Handles AI states for this object 
		AIBehavior();
	}

	// Initialize AI range indicators and states
	void SetupAI ()
	{
		if (sep_range_indicator != null)
		{
			sep_range_indicator.radius = SeparationRange;
			sep_range_indicator.offset = transform.position;
		}

		if (det_range_indicator != null)
		{
			det_range_indicator.radius = DetectionRange;
			det_range_indicator.offset = transform.position;
		}
	}

	/// <summary>
	/// Handles AI state behavior for this unit
	/// </summary>
	void AIBehavior()
	{
		//decides what this unit should be doing
		switch (unit_state)
		{
			case ai_states.stop  :
			{
				// do nothing
				break;
			}
			case ai_states.teleporter :
			{
				UpdateTeleportTimer();
				LookAt(Target); // renderer faces target, poly collider faces the wrong direction! QUATERNIONNNNS.
				CheckWeapons(); // iterate over all child weapons and try to fire them if appropriate.
				break;
			}
			case ai_states.face_shoot :
			{
				LookAt(Target); // renderer faces target, poly collider faces the wrong direction! QUATERNIONNNNS.
				CheckWeapons(); // iterate over all child weapons and try to fire them if appropriate.
				break;
			}
			case ai_states.flock:
			{
				// nothing, yet.
			}
			break;
		} // end switch(unit_state)
		
		
	}

	/// <summary>
	/// handles the teleport timer, teleports the player when appropriate
	/// </summary>
	void UpdateTeleportTimer()
	{
		teleportTimer += Time.deltaTime;
		if (teleportTimer >= TeleportChargeTime)
		{
			teleportTimer = 0.0f;
			if (TeleportToTarget)
			{
				transform.position = Target.transform.position + (Vector3)Randomize_circle(TeleportRadius);
			}
			else
			{
				transform.position += (Vector3)Randomize_circle(TeleportRadius);
			}
		}
	}

	/// <summary>
	/// Does a GameObject.Find for a target named object and returns it.
	/// </summary>
	GameObject SmartTarget(string TagToTarget = "Player")
	{
		GameObject temp_target;

		temp_target = GameObject.Find(TagToTarget);

		return temp_target;
	}

	void Thrust()
	{
		// add movement in the direction the object is facing.

		// get normalized vector from rotation quaternion
		//transform.Translate(new Vector3(facing.x * speed, facing.y * speed, 0));

	}

	/// <summary>
	/// Iterate over all weapons. If they all have minimal firing energy and are ready to shoot, fire them.
	/// </summary>
	void CheckWeapons()
	{
		int WeaponsReady = 0;

		// test all weapons 
		if (WeaponCtrlList != null)
		{
			RegisterWeapons();
		}

		if (WeaponCtrlList == null)
		{
			Debug.Log ("CheckWeapons(): Fail - WeaponCtrlList == null");
			return;
		}

		foreach (Weapon wpn in WeaponCtrlList)
		{
			if ((wpn.ShotReady == true) && (wpn.ChargeReady == true))
			{
				WeaponsReady += 1;
			}
		}

		if (WeaponsReady == WeaponCtrlList.Count)
		{
			FireAllWeapons();
		}
		else if (WeaponsReady == 0)
		{
			// ouch, none of the weapons are ready!
		}
		else
		{
			// some intermediate number of weapons are ready.
			// Fire individual categories of weapons only? 'light', 'heavy' could be cool.
		}

	}

	/// <summary>
	/// Get a list of all child weapon objects, add them to appropriate lists.
	/// </summary>
	public void RegisterWeapons()
	{
		Weapon tmpWeaponScript = null;
	
		if (WeaponList == null)
		{
			WeaponList = new List<GameObject>();
		}

		if (WeaponCtrlList == null)
		{
			WeaponCtrlList = new List<Weapon>();
		}

		foreach (Transform t in transform)
		{
			if (t.CompareTag("Weapon"))
			{
				tmpWeaponScript = t.gameObject.GetComponent<Weapon>();
				
				if (tmpWeaponScript != null)
				{
					Debug.Log (this + ": found weapon " + tmpWeaponScript.DisplayName);
					
					// Add this weapon to gameObject and Script lists.
					if (!WeaponList.Contains(t.gameObject))
					{
						WeaponList.Add(t.gameObject);
					}
					
					if (!WeaponCtrlList.Contains(tmpWeaponScript))
					{
						WeaponCtrlList.Add(tmpWeaponScript);
					}
				}
			}
		}
		
		// show a count of the # of weapons found.
		if (WeaponCtrlList.Count > 0)
		{
			Debug.Log(this + ": " + WeaponCtrlList.Count.ToString() + " weapons registered!");
		}
	}
	/// <summary>
	/// Calls Fire method on each weapon in WeaponList
	/// </summary>
	private void FireAllWeapons()
	{
		Debug.Log ("FireAllWeapons(): Called."); 
		foreach (Weapon wpn in WeaponCtrlList)
		{
			wpn.Fire();
		}
	}

	/// <summary>
	/// Rotate object to face the direction of its velocity.
	/// </summary>
	private void calc_facing()
	{
		// Rotate object to face the direction it's moving.
		Vector3 dir = _Rigidbody.velocity;

		// Calculate the angle of current speed.
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

		// Rotate the object to face that angle
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

	}

	/// <summary>
	/// Instantly Rotate to face target GameObject
	/// </summary>
	/// <param name="to_target">To_target.</param>
	void LookAt(GameObject to_target)
	{
		if (to_target != null)
		{
			Vector3 relativePos = to_target.transform.position - transform.position;
			Quaternion rotation = Quaternion.LookRotation(Vector3.back, relativePos);
			transform.rotation = rotation;
		}
		else
		{
			Debug.Log ("Enemy_Pilot: Can't LookAt a null object!");
		}
	}


	/// <summary>
	/// Randomize Position and Rotation of this transform.
	/// </summary>
	void Randomize()
	{
		Vector2 new_loc = new Vector2(Random.value * map_size.x, Random.value * map_size.y);
		transform.position = new_loc;

		Vector3 new_rot = Random.rotation.eulerAngles;
		currentTheta = (int)new_rot.x;

	}

	/// <summary>
	/// Randomize Position within circle, and Rotation
	/// </summary>
	/// <param name="spawn_radius">Spawn_radius.</param>
	Vector2 Randomize_circle(float spawn_radius)
	{
		Vector2 new_loc = Random.insideUnitCircle * spawn_radius;

		Vector3 new_rot = Random.rotation.eulerAngles;	
		currentTheta = (int)new_rot.x;

		return new_loc;
	}

	/// <summary>
	/// Move_heading the specified new_heading.
	/// Causes the bird to attempt to face a new direction.
	/// Based on maxTurnTheta, the bird may not be able to complete the 
	/// </summary>
	/// <param name="new_heading">The direction in degrees that the bird should turn toward..</param>
	void move_heading(int new_heading)
	{
		// determine if it is better to turn left or right for the new heading
		int left = (new_heading - currentTheta + 360) % 360;
		int right = (currentTheta - new_heading + 360) % 360;

		// after deciding which way to turn, find out if we can turn that much
		int thetaChange = 0;
		if (left < right) {
			// if left > than the max turn, then we can't fully adopt the new heading
			thetaChange = Mathf.Min(maxTurnTheta, left);
		}
		else {
			// right turns are negative degrees
			thetaChange = -Mathf.Min(maxTurnTheta, right);
		}
		
		// Make the turn
		currentTheta = (currentTheta + thetaChange + 360) % 360;

		Vector2 new_loc = new Vector2(transform.position.x, transform.position.y);

		// Now move currentSpeed pixels in the direction the bird now faces.
		// Note: Because values are truncated, a speed of 1 will result in no
		// movement unless the bird is moving exactly vertically or horizontally.
		new_loc.x += (int)(speed * Mathf.Cos(currentTheta * Mathf.PI / 180)) + map_size.x;
		new_loc.x %= map_size.x;
		new_loc.y -= (int)(speed * Mathf.Sin(currentTheta * Mathf.PI / 180)) - map_size.y;
		new_loc.y %= map_size.y;
	
		transform.position = new_loc;
	}

	/**
     * Get the distance in pixels between this bird and another
     *
     * @param  otherBird The other bird to measure the distance between
     * @return The distance to the other bird
     */
	public float getDistance(GameObject otherBird) {
		float dX = otherBird.transform.position.x - transform.position.x;
		float dY = otherBird.transform.position.y - transform.position.y;
		
		return (int)Mathf.Sqrt( Mathf.Pow( dX, 2 ) + Mathf.Pow( dY, 2 ));
	}

	/**
     * Get the distance in pixels between this bird and a point
     *
     * @param p The point to measure the distance between
     * @return The distance between this bird and the point
     */
	public int getDistance(Vector3 p) {
		float dX = p.x - transform.position.x;
		float dY = p.y - transform.position.y;
		
		return (int)Mathf.Sqrt( Mathf.Pow( dX, 2 ) + Mathf.Pow( dY, 2 ));
	}

	/**
     * Get the current direction that the bird is facing
     *
     * @return The Maximum Theta for this bird
     */
	public int getMaxTurnTheta() {
		return maxTurnTheta;
	}

	/**
     * Set the maximum turn capability of the bird for each movement.
     *
     * @param  theta The new maximum turning theta in degrees
     */
	public void setMaxTurnTheta(int theta)
	{
		maxTurnTheta = theta;
	}
	
	/**
     * Get the current direction of this bird
     *
     * @return  The direction that this bird is facing
     */
	public int getTheta() {
		return currentTheta;
	}
	
	/**
     * Get the current location of this bird
     *
     * @return  The location of this bird
     */
	public Vector2 getLocation() {
		return transform.position;
	}

	/**
     * Set the current speed of the bird
     *
     * @param  speed The new speed for the bird
     */
	public void setSpeed( float set_speed ) {
		speed = set_speed;
	}
	
	/**
     * Get the color of this bird
     *
     * @return  The color of this bird
     */
	public Color getColor() {
		return FlockColor;
	}

	// returns the navigational distance to the center of the ship.
	public float GetNavMinDistance()
	{
		return mNavMinDistance;
	}

}