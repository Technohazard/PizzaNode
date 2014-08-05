
using UnityEngine;
using System.Collections;
// PizzaNode Playercontroller.cs

public class unit_controller : MonoBehaviour {

	public int speed = 5;
	public Vector2 maxVelocity = new Vector3(1000,1000);
	public float ply_rot_speed = 1.0f;

	public GUIText player_GUItext; // attach player GUI info

	private float xAxisValue;
	private float yAxisValue;
	
	private KeyCode keys_camera = KeyCode.C;
	private KeyCode keys_fire = KeyCode.Z;
	private KeyCode keys_wave = KeyCode.N;
	private KeyCode keys_upgrade = KeyCode.U;

	private Vector3 euler_rot; // use to calculate player rotation
	private Vector2 player_control; // Player control input

	private weapon_01[] weapons; // list of player weapon components;

	// Use this for initialization
	void Start () {
	
		weapons = GetComponentsInChildren<weapon_01>();

	}
	
	// Update is called once per frame
	void Update () {
	
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

		#region Animate Player
		// Rotate at speed
		euler_rot = new Vector3 (0,0,ply_rot_speed);
		transform.Rotate(euler_rot);

		#endregion

		gui_update(); // update player info text
	}

	void player_fire()
	{
		// gets a list of all player child weapon scripts and calls Fire method on each
		foreach (weapon_01 wpn in weapons)
		{
			wpn.Fire();
		}
	}

	void gui_update()
	{
		player_GUItext.text = player_info_str();
	}

	string player_info_str()
	{
		string make_string = "";

		string hp_disp_val = "";
		string shield_disp_val = "";

		hp_disp_val = ((gameObject.GetComponent<health_script>().hp / gameObject.GetComponent<health_script>().hp_max)*100).ToString();
		//shield_disp_val = ((gameObject<health_script>.hp / gameObject.GetComponentInChildren<health_script>().hp_max)*100).ToString();

		make_string += "HP:" + hp_disp_val;
		make_string += "\n" + "SP:" + shield_disp_val;

		return make_string;
	}

	void location_reset()
	{
		transform.position = Vector3.zero;
		transform.rigidbody2D.velocity = Vector2.zero;
	}
	
}
