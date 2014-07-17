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
	public Camera_Modes cam_mode = Camera_Modes.Track;
	public bool ClampZOffset = true; // Clamps camera to initial editor Z offset when true

	public float mouse_track_speed = 10.0f;

	public Vector3 cam_offset = Vector3.zero;	

	public Quaternion LookAtRotation;
	
	private Vector3 desiredPosition = Vector3.zero;	
	private Vector3 cameraVelocity = Vector3.zero;
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
		desiredPosition = cam_offset;
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
		// Control the camera w/ XY input = Free Look
			float xAxisValue = Input.GetAxis("Horizontal");
			float zAxisValue = Input.GetAxis("Vertical");
			myCam.transform.Translate(new Vector3(xAxisValue, zAxisValue,0.0f));
		}
		else if (cam_mode == Camera_Modes.Spring)
		{
		// Camera behaves as if on an invisible spring
		// buggy as fuck
			FollowUpdate();	
		}
		else if (cam_mode == Camera_Modes.Mouselook)
		{
		// Camera attempts to follow the mouse pointer.
			Vector3 player_loc;
			player_loc = Camera.main.WorldToScreenPoint(Target.position); // the thing we're following

			Vector3 mouse_loc;
			mouse_loc = Input.mousePosition; // location of the mouse pointer on screen

			Vector3 delta = player_loc + mouse_loc;
			float dist = Vector3.Distance(player_loc, mouse_loc); // get the distance before we wipe it out
			delta.Normalize(); // normalized heading

			myCam.transform.position = Target.position + (delta * mouse_track_speed);

		}
		else
		{
			// room for other camera modes
		}

	}

	void Update () 
	{

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
		Vector2 menu_size = new Vector2(100, 80); // camera mode box
		Vector2 menu_pos = new Vector2((Screen.width - menu_size.x),0);

		Vector2 button_size = new Vector2(80,20); // camera mode button
		Vector2 button_pos = menu_pos + (button_size/2);

		// top right
		string poostring = "Camera Mode: " + cam_mode.ToString();
		GUI.Box(new Rect(menu_pos.x, menu_pos.y, menu_size.x, menu_size.y), poostring);
		//update camera mode display text (font)
		camera_GUItext.text = poostring;

		GUI.Label (new Rect (button_pos.x, button_pos.y - (button_size.y/2), button_size.x, button_size.y), poostring);

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
	void FollowUpdate()		
	{
		Vector3 stretch = myCam.transform.position - Target.position;
		Vector3 force = -Stiffness * stretch - Damping * cameraVelocity;
		Vector3 acceleration = force / Mass;
		
		cameraVelocity += acceleration * Time.deltaTime;
		myCam.transform.position += cameraVelocity * Time.deltaTime;
		
		/* Matrix4x4 CamMat = new Matrix4x4();
	
	CamMat.SetRow(0, new Vector4(-Target.forward.x, -Target.forward.y, -Target.forward.z));		
	CamMat.SetRow(1, new Vector4(Target.up.x, Target.up.y, Target.up.z));
	Vector3 modRight = Vector3.Cross(CamMat.GetRow(1), CamMat.GetRow(0));	
	CamMat.SetRow(2, new Vector4(modRight.x, modRight.y, modRight.z));
	
	desiredPosition = Target.position + cam_offset; // + TransformNormal(cam_offset, CamMat);	
	//Vector3 lookat = Target.position + TransformNormal(LookAtOffset, CamMat);
	
	// myCam.transform.LookAt(lookat, Target.up);
	myCam.transform.LookAt(Target.position);
	
	myCam.projectionMatrix = Matrix4x4.Perspective(myCam.fieldOfView, myCam.aspect, myCam.nearClipPlane, myCam.farClipPlane);*/
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
			cam_mode = Camera_Modes.Spring;
		}
		else if (cam_mode == Camera_Modes.Spring)
		{
			cam_mode = Camera_Modes.Mouselook;
		}
		else if (cam_mode == Camera_Modes.Mouselook)
		{
			cam_mode = Camera_Modes.Track;
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