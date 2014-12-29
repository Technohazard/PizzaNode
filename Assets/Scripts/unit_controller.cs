
using UnityEngine;
using System.Collections;
// PizzaNode Playercontroller.cs

public class unit_controller : MonoBehaviour {

	public int speed = 5;
	public Vector3 maxVelocity = new Vector3(10,10,0);
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
	


		AnimatePlayerModel ();
		updateGUI(); // update player info text
	}

	/// <summary>
	/// moving to player.cs
	/// </summary>
	void player_fire()
	{
		// gets a list of all player child weapon scripts and calls Fire method on each
		foreach (weapon_01 wpn in weapons)
		{
			wpn.Fire();
		}
	}

	/// <summary>
	/// Handle animation states, model animation transforms.
	/// </summary>
	void AnimatePlayerModel ()
	{
		// Rotate at speed
		euler_rot = new Vector3 (0, 0, ply_rot_speed);
		transform.Rotate (euler_rot);
	}

	void updateGUI()
	{
		player_GUItext.text = RefreshPlayerInfoString();
	}

	/// <summary>
	/// Generates UI-friendly strings for updating the UI with player status.
	/// </summary>
	string RefreshPlayerInfoString()
	{

		string hp_disp_val = "";
		string shield_disp_val = "";

		hp_disp_val = ((gameObject.GetComponent<health_script>().hp / gameObject.GetComponent<health_script>().hp_max)*100).ToString();
		//shield_disp_val = ((gameObject<health_script>.hp / gameObject.GetComponentInChildren<health_script>().hp_max)*100).ToString();

		infoString += "HP:" + hp_disp_val;
		infoString += "\n" + "SP:" + shield_disp_val;

		return infoString;
	}

	void location_reset()
	{
		transform.position = Vector3.zero;
		transform.rigidbody2D.velocity = Vector2.zero;
	}
	
}
