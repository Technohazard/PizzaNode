using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Player : MonoBehaviour 
{
	public string Name = "<default>";
	public int PlayerID = 0;
	public int Money = 0; // this is going to need to be a lot bigger
	public int Level = 1; 

	private double _playerHealth = 1;
	public double Health {
		get {
					if (HealthControllerScript != null)
					{
						_playerHealth = Math.Floor(HealthControllerScript.HP);
					}
				
					return _playerHealth;		
				}
				set {
						_playerHealth = value;
						HealthControllerScript.HP = Convert.ToDouble(value);
				}
		}

	private double _playerShield = 1;
	public double Shield {
			get {
					if (ShieldControllerScript != null)
					{
						_playerShield = ShieldControllerScript.HP;
					}

					return _playerShield;		
			}
			set {
					_playerShield = value;
					ShieldControllerScript.HP = value;
			}
	}

	// list of child weapons and their control scripts
	private List<GameObject> WeaponList = new List<GameObject>();
	private List<Weapon> WeaponCtrlList = new List<Weapon>();

	// health and shield scripts
	public HealthController HealthControllerScript = null;
	public ShieldController ShieldControllerScript = null;

	// Player Control Variables
	private float xAxisValue;
	private float yAxisValue;

	private Vector2 player_control = Vector2.zero; // used to process input

	// Player physics values
	public int thrust = 5;
	public Vector3 maxVelocity = new Vector3(10,10,0);

	// Player Animation values.
	public float Anim_RotateSpeed = 1.0f;
	private Vector3 _AnimRotationSpeed = Vector3.zero;

	// Player Control Default Keycodes
	private KeyCode keys_camera = KeyCode.C;
	private KeyCode keys_fire = KeyCode.Z;
	private KeyCode keys_upgrade = KeyCode.U;
	private KeyCode keys_build = KeyCode.B;
	private KeyCode keys_reset = KeyCode.R;

	private Rigidbody2D mRigidBody = null;
	
	// UI Display panels
	public PlayerPanel_UI PlayerPanel;

	// One panel for each weapon slot, should be 2
	public WeaponPanel_UI[] WeaponPanels; 

	public GameControlPanel_UI GameControlPanel;

	// Use this for initialization
	void Start () 
	{
		Name = "Player 1";
		RegisterWeapons();
		RegisterRigidBody2d();

		if (HealthControllerScript == null)
		{
			HealthControllerScript = gameObject.GetComponent<HealthController>();
		}

		if ( ShieldControllerScript == null)
		{
			ShieldControllerScript = gameObject.GetComponent<ShieldController>();
		}

		RegisterUIPanels();
	}

	void RegisterUIPanels()
	{
		// Take ownership of panels as needed.
		if (GameControlPanel != null)
		{
			GameControlPanel.PanelOwner = this;
		}

		// Take ownership of panels as needed.
		if (PlayerPanel != null)
		{
			PlayerPanel.PanelOwner = this;
		}

	}
	void RegisterRigidBody2d ()
	{
		mRigidBody = gameObject.GetComponent<Rigidbody2D> ();
		if (mRigidBody != null) 
		{
			Debug.Log ("Successfully registered Player Rigidbody2D");
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		HandlePlayerInput();
		AnimatePlayerModel();
		updateGUI(); // update player info text // TODO: Don't have to do this every damn frame!
	}

	/// <summary>
	/// Handle animation states, model animation transforms.
	/// </summary>
	private void AnimatePlayerModel ()
	{
		// Rotate at speed
		_AnimRotationSpeed.z = Anim_RotateSpeed;
		transform.Rotate(_AnimRotationSpeed);
	}

	/// <summary>
	/// Get a list of all child weapon objects, add them to appropriate lists.
	/// </summary>
	public void RegisterWeapons()
	{
		Weapon tmpWeaponScript = null;

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

		// Update weapons panel with all weapons
		if (WeaponPanels != null)
		{
			if (WeaponPanels.Length > 0)
			{
				for (int i =0; i < WeaponPanels.Length; i++)
				{
					if (i < WeaponCtrlList.Count)
					{
						WeaponPanels[i].SetCurrentDisplayWeapon(WeaponCtrlList[i]);
					}
				}
			}
		}
	}

	/// <summary>
	/// Handles player input.
	/// </summary>
	private void HandlePlayerInput()
	{
		#region Handle Player Input
		// Sample axes
		xAxisValue = Input.GetAxis("Horizontal");
		yAxisValue = Input.GetAxis("Vertical");
		
		// move the player
		// Linear 1-1 control
		//transform.Translate(xAxisValue * speed * Time.deltaTime, yAxisValue * speed * Time.deltaTime, 0);
		
		// force-based control
		player_control = new Vector2(xAxisValue * thrust * Time.deltaTime, yAxisValue * thrust * Time.deltaTime);
		mRigidBody.AddForce(player_control);

		// Clamp velocity values
		Vector2 updatedVelocity = mRigidBody.velocity;

		updatedVelocity.x = Mathf.Clamp(updatedVelocity.x, -maxVelocity.x, maxVelocity.x);
		updatedVelocity.y = Mathf.Clamp(updatedVelocity.y, -maxVelocity.y, maxVelocity.y);

		if (updatedVelocity != mRigidBody.velocity)
		{
			mRigidBody.velocity.Set(updatedVelocity.x, updatedVelocity.y);
		}

		// Handle Keyboard input

		if (Input.GetKey(keys_camera))
		{
			// Change camera mode with C
			// TODO: UGH make this shit a reference or something. Don't just randomly GO_find on updates! Jesus.
			GameObject.Find("Camera_Controller").SendMessage ("ChangeCameraMode");
		}
		else if (Input.GetKey(keys_fire))
		{
			// i'm firin mah lazer
			FireAllWeapons();
		}
		else if (Input.GetKey (keys_upgrade))
		{
			// open the upgrade menu
			if (GameControlPanel != null)
			{
				// TODO: make this actually click the button
				GameControlPanel.OnUpgradeButtonClick ();
			}
		}

		else if (Input.GetKey(keys_build))
		{
			// open the upgrade menu
			if (GameControlPanel != null)
			{
				// TODO: make this actually click the button
				GameControlPanel.OnBuildButtonClick ();
			}
		}

		else if (Input.GetKey(keys_reset))
		{
			ResetLocation();
		}

		// Fire a bullet if the player hits the fire button
		if (Input.GetButton("Fire1")) 
		{
			FireAllWeapons();
		}
		
		
		#endregion
	}

	/// <summary>
	/// calls .Fire() on each weapon in WeaponCtrlList
	/// </summary>
	private void FireAllWeapons()
	{
		// calls Fire method on each weapon in WeaponCtrlList
		foreach (Weapon wpn in WeaponCtrlList)
		{
			wpn.Fire();
		}
	}

	/// <summary>
	/// Resets player position and velocity to zero.
	/// </summary>
	public void ResetLocation()
	{
		transform.position = Vector3.zero;
		mRigidBody.velocity = Vector2.zero;
	}
	
	/// <summary>
	/// Update all player associated GUI elements.
	/// </summary>
	private void updateGUI()
	{
		if (PlayerPanel == null)
			return;

		PlayerPanel.SetPlayerName(Name);
		PlayerPanel.SetPlayerHealth(HealthControllerScript.HP, HealthControllerScript.HPMax);
		PlayerPanel.SetPlayerShield(ShieldControllerScript.HP, ShieldControllerScript.HPMax);
		PlayerPanel.SetPlayerMoney(Money);
		PlayerPanel.SetPlayerLevel(Level);
	}
}