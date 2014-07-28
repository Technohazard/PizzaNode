using UnityEngine;
using System.Collections;

/* Camera Controller for PizzaNode
* 6/19/2014 - copied from UNCE project
* 
*/

public class Camera_Controller : MonoBehaviour {

	public Transform Target;
	public Camera myCam;
	public GUIText camera_GUItext;
	
	#region Spring Parameters
	// Spring parameters
	public float Stiffness = 1800.0f;
	public float Damping = 600.0f;
	public float Mass = 50.0f;
	#endregion

	// Camera Modes:
	// Free = control w/ XY axis input
	// Track = Hard Follow selected target
	// Spring = uses a simulated spring to clamp camera to target
	// Mouselook = camera follows mouse projection to 2d world XY coordinates
	#region Camera Parameters
	public enum Camera_Modes {Free, Track, Spring, Mouselook};
	public Camera_Modes cam_mode = Camera_Modes.Spring; // omg spring camera is so good now! - 7/18/14
	public bool ClampZOffset = true; // Clamps camera to initial editor Z offset when true
	public bool cam_rotate = false; // rotate camera to follow mouse pointer in Camera_Modes.MouseLook

	public float mouse_track_speed = 10.0f;
	public float turnSpeed = 1.0f;

	public Vector3 cam_offset = Vector3.zero;	

	public Quaternion LookAtRotation;

	private Vector3 OG_Target_Position = Vector3.zero; // store position relative to original

	#endregion

	private Vector2 mouse_loc; // used to get mouse location for Mouselook

	void Start()
	{	
		OG_Target_Position = Target.position;
		LookAtRotation = Quaternion.LookRotation(OG_Target_Position);
		transform.rotation = LookAtRotation;

		if (cam_offset == Vector3.zero)
		{
			// Defines offset for cam_mode.Track as initial camera position relative to target.
			cam_offset = myCam.transform.position + OG_Target_Position;

		}
		myCam.transform.LookAt(Target);
	}

	void FixedUpdate()
	{
		if (cam_mode == Camera_Modes.Track)
		{
			track_target();
		}
		else if (cam_mode == Camera_Modes.Free)
		{
			cameraMove_Free();
		}
		else if (cam_mode == Camera_Modes.Spring)
		{
		// Camera behaves as if on an invisible spring
		// buggy as fuck
			cameraMove_Spring();	
		}
		else if (cam_mode == Camera_Modes.Mouselook)
		{
		// Camera tracks towards the mouse pointer.
			cameraMove_MouseLook();
		}
		else
		{
			// room for other camera modes
		}

	}

	void Update () 
	{

	}

	void cameraMove_MouseLook()
	{
		// from: http://forum.unity3d.com/threads/rotate-sprite-to-face-direction.218557/
	
		// HANDLES MOVEMENT
		Vector3 currentPosition; // stores the current position of the Camera
		Vector3 mousePosition;  // stores mouse position for quick access

		// stores the Camera's current position 
		currentPosition = myCam.transform.position;

		// stores the position of the mouse
		mousePosition = Input.mousePosition;

		// changes the mouse position variable to match the mouses position in relation to the camera
		mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

		// actually transforms the target position using the lerp function, which slowly moves the sprite between
		// the first vector position and the second vector position at the rate of moveSpeed       
		myCam.transform.position = Vector2.Lerp(myCam.transform.position, mousePosition, mouse_track_speed);
		
		if (cam_rotate == true)
		{
			// HANDLES ROTATION
			// calculate the angle we need to look toward
			Vector3 moveToward;
			Vector3 moveDirection; 

			// gets the mouse position relative to the camera and stores it in movetoward
			moveToward = Camera.main.ScreenToWorldPoint( Input.mousePosition );

			// moveDirection (variable announced at top of class) becomes moveToward - current position
			moveDirection = moveToward - currentPosition;

			// make z part of the vector 0 as we dont need it
			moveDirection.z = 0;

			// normalize the vector so its in units of 1
			moveDirection.Normalize();

			// if we have moved and need to rotate
			if (moveDirection != Vector3.zero)
			{
				// calculates the angle we should turn towards, - 90 makes the sprite rotate
				float targetAngle;
				targetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90;

				// actually rotates the sprite using Slerp (from its previous rotation, to the new one at the designated speed.
				myCam.transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, 0, targetAngle), turnSpeed * Time.deltaTime);
			}
		}

		/* 
		Vector3 moveToward;
		
		// gets the mouse position relative to the camera and stores it in movetoward
		moveToward = Camera.main.ScreenToWorldPoint( Input.mousePosition );
		
		player_loc = Camera.main.WorldToScreenPoint(Target.position); // player location in-world on screen.
		
		Vector3 mouse_loc;
		mouse_loc = Input.mousePosition; // location of the mouse pointer on screen
		
		float dist = Vector3.Distance(player_loc, mouse_loc); // get the distance between player/mouse before we wipe it out
		
		Vector3 delta = mouse_loc + player_loc;
		Debug.DrawLine(player_loc, delta, Color.green); // Delta Line
		
		delta.Normalize(); // normalized heading
		
		myCam.transform.position = Target.position + (delta * mouse_track_speed);
		*/
	}

	void track_target()
	{
		// Sets the transform position to the location of the target, plus XY offset.
		if (ClampZOffset)
		{
			myCam.transform.position = Target.position + cam_offset;
		}
		else
		{
			Vector3 delta_2d = new Vector3(Target.position.x, Target.position.y, 0);
			// z is free to swing!
			myCam.transform.position += delta_2d;
		}
	}

	void OnGUI() 
	{
		Vector2 menu_size = new Vector2(200, 40); // camera mode box
		Vector2 menu_pos = new Vector2((Screen.width - menu_size.x),0);

		Vector2 button_size = new Vector2(80, 20); // camera mode button
		Vector2 button_pos = new Vector2(menu_pos.x + (menu_size.x / 2) - (button_size.x / 2), 20f);

		// top right
		string poostring = "Camera Mode: " + cam_mode.ToString();
		GUI.Box(new Rect(menu_pos.x, menu_pos.y, menu_size.x, menu_size.y), poostring);
		//update camera mode display text (font)
		camera_GUItext.text = poostring;

		//GUI.Label (new Rect (button_pos.x, button_pos.y - (button_size.y/2), button_size.x, button_size.y), poostring);

		// Make the first button.
		if(GUI.Button(new Rect(button_pos.x, button_pos.y, button_size.x, button_size.y), "Next Mode")) 
		{
			ChangeCameraMode();
		}
	}

	/// <summary>
	/// Spring-Type camera movement
	/// Called from FixedUpdate()
	/// </summary>
	void cameraMove_Spring()		
	{
		Vector3 cameraVelocity = Vector3.zero;
		Vector3 stretch = myCam.transform.position - Target.position;
		Vector3 force = -Stiffness * stretch - Damping * cameraVelocity;
		Vector3 acceleration = force / Mass;

		Vector3 calc_pos; // final position
		calc_pos = myCam.transform.position;

		cameraVelocity += acceleration * Time.deltaTime;
		calc_pos += cameraVelocity * Time.deltaTime;
		if (ClampZOffset)
		{
			calc_pos.z = cam_offset.z; // 
		}

		myCam.transform.position = calc_pos;

		/* 
		Matrix4x4 CamMat = new Matrix4x4();
		
		CamMat.SetRow(0, new Vector4(-Target.forward.x, -Target.forward.y, -Target.forward.z));		
		CamMat.SetRow(1, new Vector4(Target.up.x, Target.up.y, Target.up.z));
		Vector3 modRight = Vector3.Cross(CamMat.GetRow(1), CamMat.GetRow(0));	
		CamMat.SetRow(2, new Vector4(modRight.x, modRight.y, modRight.z));
		
		desiredPosition = Target.position + cam_offset; // + TransformNormal(cam_offset, CamMat);	
		//Vector3 lookat = Target.position + TransformNormal(LookAtOffset, CamMat);
		
		// myCam.transform.LookAt(lookat, Target.up);
		myCam.transform.LookAt(Target.position);
		
		myCam.projectionMatrix = Matrix4x4.Perspective(myCam.fieldOfView, myCam.aspect, myCam.nearClipPlane, myCam.farClipPlane);
		*/
	}
	
	Vector3 TransformNormal(Vector3 normal, Matrix4x4 matrix)
	{
		Vector3 transformNormal = new Vector3();	
		Vector3 axisX = new Vector3(matrix.m00, matrix.m01, matrix.m02);
		Vector3 axisY = new Vector3(matrix.m10, matrix.m11, matrix.m12);
		Vector3 axisZ = new Vector3(matrix.m20, matrix.m21, matrix.m22);
		
		transformNormal.x = Vector3.Dot(normal, axisX);
		transformNormal.y = Vector3.Dot(normal, axisY);
		transformNormal.z = Vector3.Dot(normal, axisZ);
		
		return transformNormal;
	}

	void cameraMove_Free()
	{
		// Control the camera w/ XY input = Free Look
		float xAxisValue = Input.GetAxis("Horizontal");
		float zAxisValue = Input.GetAxis("Vertical");
		myCam.transform.Translate(new Vector3(xAxisValue, zAxisValue,0.0f));
	}

	/// <summary>
	/// Cycles the camera mode.
	/// </summary>
	void ChangeCameraMode()
	{
		if (cam_mode == Camera_Modes.Track)
		{
			SetCameraMode(Camera_Modes.Free);
		}
		else if (cam_mode == Camera_Modes.Free)
		{
			SetCameraMode(Camera_Modes.Spring);
		}
		else if (cam_mode == Camera_Modes.Spring)
		{
			SetCameraMode(Camera_Modes.Mouselook);
		}
		else if (cam_mode == Camera_Modes.Mouselook)
		{
			SetCameraMode(Camera_Modes.Track);
		}
	}

	void SetCameraMode(Camera_Modes to_set)
	{
		if (to_set == Camera_Modes.Track)
		{	
			target_player();
			track_target();
			cam_mode = to_set;
		}
		if (to_set == Camera_Modes.Free)
		{	
			cam_mode = to_set;
		}
		if (to_set == Camera_Modes.Spring)
		{	
			target_player();
			cam_mode = to_set;
		}
		if (to_set == Camera_Modes.Mouselook)
		{	
			cam_mode = to_set;
		}

	}

	/// <summary>
	/// Change the primary Camera tracking target.
	/// </summary>
	void SetCamera_Target(Transform target_new)
	{
		Target = target_new;
	}

	/// <summary>
	/// Find the player's transform and set camera target to it.
	/// </summary>
	void target_player()
	{
		Transform player_transform = GameObject.Find("Player").transform;
		SetCamera_Target(player_transform);
	}
}