using UnityEngine;
using System.Collections;

/// <summary>
/// Displays a 2D arrow that rotates around a central target, points towards a second target.
/// 
/// </summary>
public class NavArrow : MonoBehaviour {

	public Transform SelfTarget; // object to orbit
	public Transform Target; // object to point towards
	public Vector2 direction = Vector2.zero;
	public float ArrowRadius = 2.0f; 

	public float Theta; // facing angle of arrow

	public bool ArrowVisible; // manual control for arrow visibility
	public Camera myCam; //

	public float TargetTheta; // Angle the arrow needs to be at to face target

	private Vector3 DrawPos; // calculate where to draw the object.

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

		DrawPos = transform.position + new Vector3(0, ArrowRadius, 0);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Target)
		{
			if (!isOnScreen(Target))
			{
				if (ArrowVisible)
				{
					float f = GetTargetAngleMath(Target);
					float g = GetTargetAngleQuat(Target);

					if (f == g)
					{
						TargetTheta = GetTargetAngleQuat(Target); //
					}
					else
					{
						TargetTheta = GetTargetAngleMath(Target);
					}

					DrawPos = CalculateArrowPos(); // does fancy math to figure out where the arrow should be 
					transform.position = DrawPos;
					rotateArrow();
				}
				else
				{
					// NavArrow isn't visible don't gotta do nuttin'! :)
				}
			}
			else
			{
				// Target is onscreen.
			}
		}
		else
		{
			// No Target assigned.
			// Smart-Target Earth!
			Target = Target_Earth().transform;

		}
	}

	/// <summary>
	/// Calculate where the arrow should be positioned.
	/// </summary>
	/// <returns>The arrow position.</returns>
	Vector3 CalculateArrowPos()
	{
		Vector3 temp_pos = Vector3.zero;

		// Straight up from transform!
		temp_pos = SelfTarget.transform.position + new Vector3(0, ArrowRadius, 0);

		return temp_pos;
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
		//---------------------------------------------------------------------------------
		// 2 - Check if the object is before, in or after the camera bounds
		//---------------------------------------------------------------------------------
		
		// Camera borders
		var dist = (transform.position - Camera.main.transform.position).z;
		float leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).x;
		float rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;
		// float width = Mathf.Abs(rightBorder - leftBorder);

		var topBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).y;
		var bottomBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, dist)).y;
		// float height = Mathf.Abs(topBorder - bottomBorder);
		
		// Determine entry and exit border using direction
		Vector3 exitBorder = Vector3.zero;
		Vector3 entryBorder = Vector3.zero;
		
		if (direction.x < 0)
		{
			exitBorder.x = leftBorder;
			entryBorder.x = rightBorder;
		}
		else if (direction.x > 0)
		{
			exitBorder.x = rightBorder;
			entryBorder.x = leftBorder;
		}
		
		if (direction.y < 0)
		{
			exitBorder.y = bottomBorder;
			entryBorder.y = topBorder;
		}
		else if (direction.y > 0)
		{
			exitBorder.y = topBorder;
			entryBorder.y = bottomBorder;
		}

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
	float GetTargetAngleMath(Transform t)
	{
		float rotation;
		rotation = Mathf.Atan2(t.position.x, t.position.y);

		// rotation = rotation * 180/Math.PI; //convert radians to degrees

		return rotation;
	}

	float GetTargetAngleQuat(Transform t)
	{
		Vector3 relativePos = t.transform.position - SelfTarget.transform.position;
		Quaternion rotation = Quaternion.LookRotation(Vector3.back, relativePos);

		Debug.DrawRay(transform.position, relativePos /2 , Color.grey);
		return rotation.eulerAngles.z; 
	}

	Quaternion LookAt(Transform t)
	{
		Vector3 relativePos = t.transform.position - SelfTarget.transform.position;
		Quaternion rotation = Quaternion.LookRotation(Vector3.back, relativePos);

		Debug.DrawRay(transform.position, relativePos /2 , Color.green);

		return rotation;
	}

	GameObject Target_Earth()
	{
		GameObject temp_target;
		
		temp_target = GameObject.Find("earth_01");
		
		return temp_target;
	}

}
