using UnityEngine;
using System.Collections;

/// <summary>
/// Displays a 2D arrow that rotates around a central target, points towards a second target.
/// 
/// </summary>
public class NavArrow : MonoBehaviour {

	public Transform SelfTarget; // object to orbit
	public GameObject Target; // object to point towards
	public Vector2 direction = Vector2.zero;
	public float ArrowRadius = 2.0f; 

	private SpriteRenderer myRend = null;
	private Transform lastTransform = null;

	public enum NAVARROW_TYPES
	{
		NAV_EARTH,
		NAV_ENEMY,
		NAV_BEACON
	}
	// NAV_EARTH : points to earth
	// NAV_ENEMY : points to enemy group
	// NAV_BEACON : points to friendly beacon

	public NAVARROW_TYPES ArrowType = NAVARROW_TYPES.NAV_EARTH; // point to earth by default
	public float Theta; // facing angle of arrow

	public bool ArrowVisible; // manual control for arrow visibility
	public Camera myCam; //

	public float TargetTheta; // Angle the arrow needs to be at to face target
	private Quaternion TargetThetaRotation; // used to calculate nav arrow rotation relative to SelfTarget

	private Vector3 DrawPos; // calculate where to draw the object.

	// Use this for initialization
	void Start () 
	{

		myRend = gameObject.GetComponent<SpriteRenderer>();
		if (SelfTarget == null)
		{
			SelfTarget = GameObject.FindGameObjectWithTag("Player").transform;
		}

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
		if (ArrowVisible)
		{
			if (Target != null)
			{			
				// rotate angle to face target transform
				TargetThetaRotation = GetTargetAngleQuat(Target.transform); 				
				transform.rotation = TargetThetaRotation;
				transform.Rotate(new Vector3(0,0,-45));

				lastTransform = transform;

				if (isOnScreen(Target.transform))
				{
					// Target is onscreen.
					float tmpDistNav = 1.0f; // default dist to stop floating nav arrow

					switch(ArrowType)
					{
						case NAVARROW_TYPES.NAV_ENEMY:
						{							
							if (Target.CompareTag("Enemy_Wave"))
							{
								enemy_wave tmpWave = Target.GetComponent<enemy_wave>();	
								if (tmpWave != null)
								{
								tmpDistNav = tmpWave.GetNavMinDistance();	
								}
															
								myRend.color = Color.red;
							}
							else if (Target.CompareTag("Enemy"))
							{
								enemy_pilot tmpPilot = Target.GetComponent<enemy_pilot>();
								if (tmpPilot != null)
								{
								tmpDistNav = tmpPilot.GetNavMinDistance();	
								}

								myRend.color = Color.yellow;
							}

							break;
						}
						case NAVARROW_TYPES.NAV_EARTH:
						{
							myRend.color = Color.blue;

							break;
						}
						case NAVARROW_TYPES.NAV_BEACON:
						{
							myRend.color = Color.green;

							break;
						}
						default:
						{
							// whatever
							myRend.color = Color.gray;
							break;
						}
					}

					// only position the target if distance is sufficiently far away
					if (Vector2.Distance(transform.position, Target.transform.position) > ArrowRadius)
					{
						if (Vector2.Distance(transform.position, Target.transform.position) > tmpDistNav)
						{
							Vector3 relativePos = (Target.transform.position - SelfTarget.transform.position);
							transform.position = SelfTarget.transform.position + (relativePos.normalized * ArrowRadius);
						}
					}
					else
					{
						// arrow rotates, but doesn't move
						transform.localPosition = lastTransform.localPosition; // should save position, etc.
					}
				} // end isOnScreen(Target)
				else
				{
					// target is offscreen, always need to position arrow
					Vector3 relativePos = (Target.transform.position - SelfTarget.transform.position);
					transform.position = SelfTarget.position + (relativePos.normalized * ArrowRadius);
				}
			} // end Target != null
			else
			{
				// No Target assigned.

					GameObject go = GameObject.FindGameObjectWithTag("Earth");
				if (go != null)
				{
					// Smart-Target Earth!
					SetTarget(go);
				}
				else
				{
					/// couldn't find earth, no target
					SetTarget(null);
				}
			}
		} // end ArrowVisible
		else
		{
			// NavArrow isn't visible don't gotta do nothin'! :)
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
		transform.eulerAngles = new Vector3(0, 0, Theta);

		/*if (transform.rotation < Theta) 
		{
			transform.RotateAround(Target.position, Vector3.back, Theta);
		}
*/

		//transform.rotation = LookAt(Target);
	}

	public void showArrow()
	{
		ArrowVisible = true;
		renderer.enabled = true;
	}

	public void hideArrow()
	{
		ArrowVisible = false;
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
		rotation = Mathf.Atan2(transform.position.x - t.position.x, transform.position.y - t.position.y);

		// rotation = rotation * 180/Math.PI; //convert radians to degrees

		return rotation;
	}

	Quaternion GetTargetAngleQuat(Transform t)
	{
		Vector3 relativePos = (t.transform.position - SelfTarget.transform.position);
		// Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.back);
		Quaternion rotation = Quaternion.LookRotation(Vector3.back, relativePos);

		Debug.DrawRay(SelfTarget.transform.position, relativePos /2 , Color.grey);
		return rotation; 
	}

	Quaternion LookAt(Transform t)
	{
		Vector3 relativePos = t.transform.position - SelfTarget.transform.position;
		Quaternion rotation = Quaternion.LookRotation(Vector3.back, relativePos);

		Debug.DrawRay(transform.localPosition, relativePos /2 , Color.green);

		return rotation;
	}

	public void SetTarget(GameObject toTarget)
	{	
		Target = toTarget;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.gray;
		Gizmos.DrawWireSphere(transform.parent.transform.position, ArrowRadius);
	}
}
