using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;

/// <summary>
/// Main Camera Controller
/// </summary>
public class Camera_Controller : MonoBehaviour {

	public Transform Target;
	public Camera _Camera;

	#region Spring Parameters
	// Spring parameters
	public float Stiffness = 1800.0f;
	public float Damping = 600.0f;
	public float Mass = 50.0f;
	#endregion

	#region SmoothLerp Parameters
	// Smooth Lerp Mode Parameters.
	public AnimationCurve LerpCurve;
	public float LerpTime = 1.0f;
	public float _smoothLerpTimer = 0.0f;
	public float _lerpamt = 0.0f;
	public bool lerping = false;
	#endregion

	// Camera Modes:
	// Free = control w/ XY axis input
	// Track = Hard Follow selected target
	// Spring = uses a simulated spring to clamp camera to target
	// Mouselook = camera follows mouse projection to 2d world XY coordinates
	#region Camera Parameters
	public enum Camera_Modes {
								Unknown,
								Free, 
								Track, 
								Spring, 
								Mouselook,
								SmoothLerp
								};
	public Camera_Modes cameraMode = Camera_Modes.Spring; // omg spring camera is so good now! - 7/18/14
	public bool ClampZOffset = true; // Clamps camera to initial editor Z offset when true
	public bool cam_rotate = false; // rotate camera to follow mouse pointer in Camera_Modes.MouseLook

	public float mouse_track_speed = 10.0f;
	public float turnSpeed = 1.0f;

	/// <summary>
	/// Camera offset from final computed position.
	/// </summary>
	public Vector3 cameraOffset = Vector3.zero;	


	public Quaternion LookAtRotation;

	public bool PreserveTrackModeOffset = true;
	private Vector3 _TrackModeOffset = Vector3.zero;

	private Vector3 OG_Target_Position = Vector3.zero; // store position relative to original
	private Vector3 LastTargetPosition; // the target's position 1 frame ago.
	#endregion

	public CameraPanel_UI CameraPanel;

	private Vector2 mouse_loc; // used to get mouse location for Mouselook

	void Start()
	{	
		// Make sure we have a camera.
		if (_Camera == null)
		{
			_Camera = Camera.main;
		}

		// Store the current offset from target as starting offset.
		OG_Target_Position = Target.position;
		LookAtRotation = Quaternion.LookRotation(OG_Target_Position);
		transform.rotation = LookAtRotation;

		if (cameraOffset == Vector3.zero)
		{
			// Defines offset for cam_mode.Track as initial camera position relative to target.
			cameraOffset = _Camera.transform.position + OG_Target_Position;
		}

		_Camera.transform.LookAt(Target);

		if (CameraPanel != null)
		{
			CameraPanel.ControllerOwner = this;
		}

		// Set the best known camera mode.
		SetCameraMode(Camera_Modes.Track);
		
		UpdateUIPanel();
	}

	void FixedUpdate()
	{
		switch (cameraMode)
		{
			case Camera_Modes.Track:
			{
				SnapToTarget();
			}
			break;

			case Camera_Modes.Free:
			{
				cameraMove_Free();
			}
			break;

			case Camera_Modes.Mouselook:
				{
					// Camera tracks towards the mouse pointer.
					cameraMove_MouseLook();

				}
				break;

			case Camera_Modes.Spring:
			{
				// Camera behaves as if on an invisible spring
				// buggy as fuck
				cameraMove_Spring();	
			}
			break;

			case Camera_Modes.SmoothLerp:
			{
				UpdateLerp ();
			}
			break;
		}
	}

	void Update () 
	{
		// Track last target position in smoothLerp mode and lerp the camera if necessary.
		if (cameraMode == Camera_Modes.SmoothLerp)
		{
			if (lerping == false)
			{
				if (LastTargetPosition != Target.position)
				{
					_smoothLerpTimer = 0.0f;
					lerping = true;
				}

				LastTargetPosition = Target.position;
			}
		}
	}

	void UpdateLerp ()
	{
		if (lerping)
		{
			// NOTE: using unscaledDelta so we can move the camera around while paused.
			if (_smoothLerpTimer < LerpTime) 
			{
				_smoothLerpTimer += Time.unscaledDeltaTime;
				_lerpamt = LerpCurve.Evaluate (Mathf.Lerp (0f, 1f, _smoothLerpTimer / LerpTime));
				Vector3 displacement = (Target.position - _Camera.transform.position);
				displacement = new Vector3(displacement.x, displacement.y, 0); // clamp Z
				_Camera.transform.Translate(displacement * _lerpamt);
			}
			else 
			{
				// already at max.
				SnapToTarget ();
				lerping = false;
			}
		}
		else
		{
			SnapToTarget();
		}
	}

	void cameraMove_MouseLook()
	{
		Vector3 mousePosition = Input.mousePosition;
		cameraOffset = mousePosition;
	}

	void SnapToTarget()
	{
		if (ClampZOffset)
		{
			// Clamping z offset means camera keeps its z-height when snapping.
			Vector3 delta_2d = new Vector3(Target.position.x, Target.position.y, _Camera.transform.position.z);
			_Camera.transform.position = delta_2d;
		}
		else
		{
			// camera just gets set to target position + its offset.
			_Camera.transform.position = Target.position + cameraOffset;
		}

	}

	private void UpdateUIPanel() 
	{
		if (CameraPanel == null)
		{
			return;
		}

		string poostring = String.Format("Camera Mode:\n{0}", cameraMode.ToString());
		CameraPanel.SetText(poostring);
	}

	/// <summary>
	/// Spring-Type camera movement
	/// Called from FixedUpdate()
	/// </summary>
	void cameraMove_Spring()		
	{
		Vector3 cameraVelocity = Vector3.zero;
		Vector3 stretch = _Camera.transform.position - Target.position;
		Vector3 force = -Stiffness * stretch - Damping * cameraVelocity;
		Vector3 acceleration = force / Mass;

		Vector3 calc_pos; // final position
		calc_pos = _Camera.transform.position;

		cameraVelocity += acceleration * Time.deltaTime;
		calc_pos += cameraVelocity * Time.deltaTime;
		if (ClampZOffset)
		{
			calc_pos.z = cameraOffset.z; // 
		}

		_Camera.transform.position = calc_pos;
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
		_Camera.transform.Translate(new Vector3(xAxisValue, zAxisValue,0.0f));
	}

	/// <summary>
	/// Cycles the camera mode.
	/// </summary>
	public void ChangeCameraMode()
	{
		switch (cameraMode)
		{
			case Camera_Modes.Track:
			{ SetCameraMode(Camera_Modes.Free); }
			break;

			case Camera_Modes.Free:
			{ SetCameraMode(Camera_Modes.Spring); }
			break;

			case Camera_Modes.Spring:
			{ SetCameraMode(Camera_Modes.Mouselook); }
			break;

			case Camera_Modes.Mouselook:
			{ SetCameraMode(Camera_Modes.SmoothLerp); }
			break;

			default:
			case Camera_Modes.SmoothLerp:
			{ SetCameraMode(Camera_Modes.Track); }
			break;
		}
	}

	void SetCameraMode(Camera_Modes mode)
	{
		// When switching out of Track mode, preserve offset.
		if ((cameraMode == Camera_Modes.Track) && (cameraMode != mode))
		{
			if (PreserveTrackModeOffset)
			{
				_TrackModeOffset = cameraOffset;
			}
		}

		// Select new camera mode.
		switch (mode)
		{
			case (Camera_Modes.Track):
			{	
				TargetPlayer();

				if (PreserveTrackModeOffset)
				{
					cameraOffset = _TrackModeOffset; // snap to target.
				}
				else
				{
					cameraOffset = Vector3.zero; // snap to target.
				}

				SnapToTarget();
				cameraMode = mode;
			}
			break;

			case (Camera_Modes.Free):
			{	
				cameraMode = mode;
			}
			break;

			case (Camera_Modes.Spring):
			{	
				TargetPlayer();
				cameraMode = mode;
			}
			break;

			case (Camera_Modes.Mouselook):
			{	
				cameraMode = mode;
			}
			break;

			case Camera_Modes.SmoothLerp:
			{
				_smoothLerpTimer = 0.0f;
				TargetPlayer();
				lerping = true;
				cameraMode = mode;
			}
			break;
		}

		UpdateUIPanel(); 	
	}

	/// <summary>
	/// Change the primary Camera tracking target.
	/// </summary>
	void SetTarget(Transform target_new)
	{
		Target = target_new;
	}

	/// <summary>
	/// Find the player's transform and set camera target to it.
	/// </summary>
	void TargetPlayer()
	{
		Transform player_transform = GameObject.Find("Player_Unit").transform;
		SetTarget(player_transform);
	}

		/// <summary>
		///  check if Target's transform is visible, return true / false
		/// </summary>
		/// <returns><c>true</c>, if onscreen was ised, <c>false</c> otherwise.</returns>
		/// <param name="">.</param>
		public bool isOnScreen(Transform tform)
		{
			Vector3 currentCamPos;
			Vector3 objPos;
			// Rect r = camera.pixelRect;
			//---------------------------------------------------------------------------------
			// 2 - Check if the object is before, in or after the camera bounds
			//---------------------------------------------------------------------------------

			// Camera borders
			float zdist = (transform.position - _Camera.transform.position).z;

			float leftBorder = _Camera.ViewportToWorldPoint(new Vector3(0, 0, zdist)).x;
			float rightBorder = _Camera.ViewportToWorldPoint(new Vector3(1, 0, zdist)).x;
			// float width = Mathf.Abs(rightBorder - leftBorder);

			var topBorder = _Camera.ViewportToWorldPoint(new Vector3(0, 0, zdist)).y;
			var bottomBorder = _Camera.ViewportToWorldPoint(new Vector3(0, 1, zdist)).y;
			// float height = Mathf.Abs(topBorder - bottomBorder);

			// Determine entry and exit border using direction
//			Vector3 exitBorder = Vector3.zero;
//			Vector3 entryBorder = Vector3.zero;

//			if (direction.x < 0)
//			{
//					exitBorder.x = leftBorder;
//					entryBorder.x = rightBorder;
//			}
//			else if (direction.x > 0)
//			{
//					exitBorder.x = rightBorder;
//					entryBorder.x = leftBorder;
//			}
//
//			if (direction.y < 0)
//			{
//					exitBorder.y = bottomBorder;
//					entryBorder.y = topBorder;
//			}
//			else if (direction.y > 0)
//			{
//					exitBorder.y = topBorder;
//					entryBorder.y = bottomBorder;
//			}

			currentCamPos = _Camera.WorldToScreenPoint(_Camera.transform.position);
			objPos = _Camera.WorldToScreenPoint(tform.position);

			if (objPos.x < (currentCamPos.x - (_Camera.pixelWidth / 2)))
			{
				return false;
			}
			else if (objPos.x > (currentCamPos.x + (_Camera.pixelWidth / 2)))
			{
				return false;
			}

			if (objPos.y < (currentCamPos.y - (_Camera.pixelHeight / 2)))
			{
				return false;
			}
			else if (objPos.y > (currentCamPos.y + (_Camera.pixelHeight / 2)))
			{
				return false;
			}

			return true;
		}
}