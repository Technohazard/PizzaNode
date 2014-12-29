using UnityEngine;
using System.Collections;

public class enemy_pilot : MonoBehaviour {
	// Flocking algorithm variables
	public float SeparationRange = 1.0f;
	public float DetectionRange = 2.0f;
	public CircleCollider2D sep_range_indicator;
	public CircleCollider2D det_range_indicator;

	public float speed = 1.0f;
	public float turn_speed = 1.0f;

	public Vector2 map_size = Vector2.zero; // initialized in Start() with screen size.

	public int maxTurnTheta; // maximum turn radius in degrees
	public int currentTheta; // orientation in degrees

	protected Color FlockColor; // Color identifier for flocking purposes

	public bool showRanges = false; // show detection ranges NYI because can't draw circles! (maybe w/ linerenderer?) :(

	public GameObject Target; // default target for this unit. computed in smart_target();
	public float mNavMinDistance = 1.5f; // distance at which the arrow stops nav'ing to point to the center of the ship

	private weapon_01[] weapons; // list of player weapon components;
	private int numweapons; // number of weapons on this ship, used to check for all weapons ready.

	public enum ai_states {stop, face_shoot, teleporter};
	public ai_states unit_state = ai_states.stop;

	// stop: do nothing
	// face_player: rotate to track player position
	// teleport = randomly teleports around the map

	public float TeleportRadius = 10;
	public float TeleportChargeTime = 5;
	public bool TeleportToTarget = false; // if true, teleport to within radius of player location, not just "away from self"
	private float teleportTimer = 0.0f;

	// Use this for initialization
	void Start () {
		Target = smart_target(); // target = GameObject.Find("Player");

		// create weapon list for firing.
		getChildWeapons();

		map_size = new Vector2(Screen.width, Screen.height); // 800x600

		// Set up range indicators
		sep_range_indicator.radius = SeparationRange; 
		sep_range_indicator.center = transform.position;

		det_range_indicator.radius = DetectionRange;
		det_range_indicator.center = transform.position;
	}

	// Update is called once per frame
	void Update () 
	{
		// Handles AI states for this object 
		behave();
	}


	/// <summary>
	/// Handles AI state behavior for this unit
	/// </summary>
	void behave()
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
				UpdateTeleport();
				lookat(Target); // renderer faces target, poly collider faces the wrong direction! QUATERNIONNNNS.
				checkWeapons(); // iterate over all child weapons and try to fire them if appropriate.
				break;
			}
			case ai_states.face_shoot :
			{
				lookat(Target); // renderer faces target, poly collider faces the wrong direction! QUATERNIONNNNS.
				checkWeapons(); // iterate over all child weapons and try to fire them if appropriate.
				break;
			}
		} // end switch(unit_state)
		
		
	}

	/// <summary>
	/// handles the teleport timer, teleports the player when appropriate
	/// </summary>
	void UpdateTeleport()
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
	/// Does a GameObject.Find for the Player and returns it.
	/// </summary>
	GameObject smart_target()
	{
		GameObject temp_target;

		temp_target = GameObject.Find("Player");

		return temp_target;
	}

	void Thrust()
	{
		// add movement in the direction the object is facing.

		// get normalized vector from rotation quaternion
		//transform.Translate(new Vector3(facing.x * speed, facing.y * speed, 0));

	}

	void checkWeapons()
	{
		int tempweapons = 0;

		// test all weapons to see if they have minimal firing energy and are ready to shoot.
		foreach (weapon_01 wpn in weapons)
		{
			if ((wpn.shot_ready == true)&&(wpn.charge_ready == true))
			{
				tempweapons += 1;
			}
		}

		if (tempweapons == numweapons)
		{
			Fire_all();
		}
		else if (tempweapons == 0)
		{
			// ouch, none of the weapons are ready!
		}
		else
		{
			// some intermediate number of weapons are ready. 
			// Fire individual categories of weapons only? 'light', 'heavy' could be cool.
		}

	}

	void getChildWeapons()
	{
		// get a list of children weapon scripts for firing.
		weapons = GetComponentsInChildren<weapon_01>();
		numweapons = weapons.Length;
	}


	void Fire_all()
	{
		// gets a list of all player child weapon scripts and calls Fire method on each
		foreach (weapon_01 wpn in weapons)
		{
			wpn.Fire();
		}
	}

	void calc_facing()
	{
		// rotate object to face the direction it's moving.
		Vector3 dir = rigidbody2D.velocity;

		// calculate the angle of current speed.
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

		// rotate the object to face that angle
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

	}

	void lookat(GameObject to_target)
	{
		Vector3 relativePos = to_target.transform.position - transform.position;
		Quaternion rotation = Quaternion.LookRotation(Vector3.back, relativePos);
		transform.rotation = rotation;
	}

	void Randomize()
	{
		Vector2 new_loc = new Vector2(Random.value * map_size.x, Random.value * map_size.y);
		transform.position = new_loc;

		Vector3 new_rot = Random.rotation.eulerAngles;
		currentTheta = (int)new_rot.x;

	}

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