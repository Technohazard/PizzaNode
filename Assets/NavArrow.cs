using UnityEngine;
using System.Collections;

/// <summary>
/// Displays a 2D arrow that rotates around a central target, points towards a second target.
/// 
/// </summary>
public class NavArrow : MonoBehaviour {

	public Transform SelfTarget; // object to orbit
	public Transform Target; // object to point towards

	public float Theta; // facing angle of arrow

	public bool ArrowVisible; // manual control for arrow visibility
	public Camera myCam; //

	// Use this for initialization
	void Start () {
		if (ArrowVisible)
		{
			showArrow();
		}
		else
		{
			hideArrow(); // arrow always starts hidden
		}

		if (myCam == null)
		{
			myCam = Camera.main;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!isOnScreen(Target))
		{
			if (ArrowVisible)
			{
				Theta = GetTargetAngle(Target);
				rotateArrow();
			}
		}
	}

	void rotateArrow()
	{
		transform.rotation = LookAt(Target);
	}

	void showArrow()
	{
		renderer.enabled = true;
	}

	void hideArrow()
	{
		renderer.enabled = false;
	}


	/// <summary>
	///  check if Target's transform is visible, return true / false
	/// </summary>
	/// <returns><c>true</c>, if onscreen was ised, <c>false</c> otherwise.</returns>
	/// <param name="">.</param>
	bool isOnScreen(Transform t)
	{
		Vector3 currentCamPos;
		Vector3 objPos;
		// Rect r = camera.pixelRect;

		currentCamPos = myCam.WorldToScreenPoint(myCam.transform.position);
		objPos = myCam.WorldToScreenPoint(t.position);

		if (objPos.x < (currentCamPos.x - (myCam.pixelWidth / 2)))
		{
			return false;
		}
		else if (objPos.x > (currentCamPos.x + (myCam.pixelWidth / 2)))
		{
			return false;
		}

		if (objPos.y < (currentCamPos.y - (myCam.pixelHeight / 2)))
		{
			return false;
		}
		else if (objPos.x > (currentCamPos.x + (myCam.pixelHeight / 2)))
		{
			return false;
		}

		return true;
	}

	// use euler angles to calculate rotation angle
	float GetTargetAngle(Transform t)
	{
		float rotation;
		rotation = Mathf.Atan2(t.position.x, t.position.y);
		// rotation = rotation * 180/Math.PI; //convert radians to degrees
		return rotation;
	}

	Quaternion LookAt(Transform t)
	{
		Vector3 relativePos = t.transform.position - SelfTarget.transform.position;
		Quaternion rotation = Quaternion.LookRotation(Vector3.back, relativePos);

		Debug.DrawRay(SelfTarget.transform.position, relativePos /2 , Color.green);

		return rotation;
	}
}
