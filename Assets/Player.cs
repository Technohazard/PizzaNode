using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	public string Name = "Player 1";
	public int PlayerID = 1;
	public int money = 0; // this is going to need to be a lot bigger
	
	// list of child weapons and their control scripts
	private List<GameObject> WeaponList = new List<GameObject>();
	private List<weapon_01> WeaponCtrlList = new List<weapon_01>();

	// health and shield scripts
	private health_script refHealthScript = null;
	private shield_script refShieldScript = null;


	private weapon_01[] weapons; // list of player weapon components;

	// Player Control Variables
	private float xAxisValue;
	private float yAxisValue;

	// Player physics values
	public int speed = 5;
	public Vector3 maxVelocity = new Vector3(10,10,0);
	public float ply_rot_speed = 1.0f;

	// Player Control Default Keycodes
	private KeyCode keys_camera = KeyCode.C;
	private KeyCode keys_fire = KeyCode.Z;
	private KeyCode keys_wave = KeyCode.N;
	private KeyCode keys_upgrade = KeyCode.U;


	// Use this for initialization
	void Start () {
		Name = "Player 1";
		RegisterWeapons();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// gets a list of all child weapon objects, adds them to appropriate lists.
	public RegisterWeapons()
	{
		weapon_01 tmpWeaponScript = null;

		foreach (Transform t in transform)
		{
			if (t.CompareTag("Weapon"))
			{
				// found a weapon
				WeaponList.Add(t.gameObject);
				tmpWeaponScript = t.gameObject.GetComponent<weapon_01>();

				if (tmpWeaponScript != null)
				{
					WeaponCtrlList.Add(tmpWeaponScript);
				}
			}
		}
	}

	void HandlePlayerInput()
	{
		#region Handle Player Input
		// Sample axes
		xAxisValue = Input.GetAxis("Horizontal");
		yAxisValue = Input.GetAxis("Vertical");
		
		// move the player
		// Linear 1-1 control
		//transform.Translate(xAxisValue * speed * Time.deltaTime, yAxisValue * speed * Time.deltaTime, 0);
		
		// force-based control
		player_control = new Vector2(xAxisValue * speed * Time.deltaTime, yAxisValue * speed * Time.deltaTime);
		gameObject.rigidbody2D.AddForce(player_control);
		
		// Clamp velocity values
		if (transform.rigidbody2D.velocity.x > maxVelocity.x)
			transform.rigidbody2D.velocity.Set(maxVelocity.x, transform.rigidbody2D.velocity.y);
		else if (transform.rigidbody2D.velocity.x < -maxVelocity.x)
			transform.rigidbody2D.velocity.Set(-maxVelocity.x, transform.rigidbody2D.velocity.y);
		
		if (transform.rigidbody2D.velocity.y > maxVelocity.y)
			transform.rigidbody2D.velocity.Set(transform.rigidbody2D.velocity.x, maxVelocity.y);
		else if (transform.rigidbody2D.velocity.y < -maxVelocity.y)
			transform.rigidbody2D.velocity.Set(transform.rigidbody2D.velocity.x,-maxVelocity.y);
		
		if (Input.GetKey(keys_camera))
		{
			// Change camera mode with C
			GameObject.Find("Camera_Controller").SendMessage ("ChangeCameraMode");
		}
		else if (Input.GetKey(keys_fire))
		{
			// i'm firin mah lazer
			//FireLaser();
		}
		else if (Input.GetKey (keys_wave))
		{
			// advance the wave spawner state.
			GameObject wav_spwn = GameObject.Find("wave_spawner");
			wav_spwn.SendMessage("AdvanceState");
		}
		else if (Input.GetKey (keys_upgrade))
		{
			// open the upgrade menu
		}
		
		
		// Fire a bullet if the player hits the fire button
		if (Input.GetButton("Fire1")) 
		{
			player_fire();
		}
		
		
		#endregion
	}

	/// <summary>
	/// calls .Fire() on each weapon in WeaponCtrlList
	/// </summary>
	void FireAllWeapons()
	{
		// calls Fire method on each weapon in WeaponCtrlList
		foreach (weapon_01 wpn in WeaponCtrlList)
		{
			wpn.Fire();
		}
	}

}
