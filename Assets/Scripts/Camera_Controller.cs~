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
	
	#region Camera Parameters
	// Camera Modes:
	// Free = control w/ axis input
	// Track = Hard Follow selected target
	// Spring = uses a simulated spring to clamp camera to target
	private enum Camera_Modes {Free, Track, Spring};
	private Camera_Modes cam_mode = Camera_Modes.Free;
	
	private Vector3 cam_offset = new Vector3(0,0,-10);	
	
	// public Vector3 DesiredOffset = new Vector3(0.0f, 3.5f, -10.0f);	
	public Vector3 LookAtOffset = new Vector3(0.0f, 3.1f, 0.0f);
	
	private Vector3 desiredPosition = Vector3.zero;	
	private Vector3 cameraVelocity = Vector3.zero;
	#endregion
	
	void Start()
	{
		cam_mode = Camera_Modes.Track;
		SetCamera_GUI();
		
		// Defines offset for cam_mode.Track as initial camera position relative to target.
		cam_offset = Target.position + myCam.transform.position;
		desiredPosition = cam_offset;
		myCam.transform.LookAt(Target);
		
	}
	
	#region Follow Update
	/// <summary>
	/// Spring-Type camera movement
	/// Called from FixedUpdate()
	/// </summary>
	void FollowUpdate()		
	{		
		Vector3 stretch = myCam.transform.position - desiredPosition;
		Vector3 force = -Stiffness * stretch - Damping * cameraVelocity;
		Vector3 acceleration = force / Mass;
		
		cameraVelocity += acceleration * Time.deltaTime;
		myCam.transform.position += cameraVelocity * Time.deltaTime;
		
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
	}
	#endregion
	
	void LateUpdate () {
		if (cam_mode == Camera_Modes.Track)
		{
			// Sets the transform position to the location of the target, plus offset.
			myCam.transform.position = Target.position + cam_offset;
		}
		else if (cam_mode == Camera_Modes.Spring)
		{
			FollowUpdate();	
		}
		else 
		{
			// if (cam_mode == Camera_Modes.Free);
			float xAxisValue = Input.GetAxis("Horizontal");
			float zAxisValue = Input.GetAxis("Vertical");
			myCam.transform.Translate(new Vector3(xAxisValue, zAxisValue,0.0f));
		}
	}
	
	#region Transform Normal
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
	#endregion
	
	#region Change Camera Mode
	/// <summary>
	/// Changes the camera mode.
	/// </summary>
	void ChangeCameraMode()
	{
		if (cam_mode == Camera_Modes.Free)
		{
			cam_mode = Camera_Modes.Track;
		}
		else if (cam_mode == Camera_Modes.Track)
		{
			cam_mode = Camera_Modes.Spring;
		}
		else if (cam_mode == Camera_Modes.Spring)
		{
			cam_mode = Camera_Modes.Free;
		}
		myCam.transform.LookAt(Target);
		SetCamera_GUI();
	}
	#endregion
	
	void SetCamera_GUI()
	{
		string poostring = "Camera Mode: " + cam_mode.ToString();
		camera_GUItext.text = poostring;
	}
	
	void SetCamera_Target(Transform target_new)
	{
		Target = target_new;
	}
}