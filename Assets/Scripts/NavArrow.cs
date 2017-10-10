using UnityEngine;
using System.Collections;

/// <summary>
/// Displays a 2D arrow that rotates around a central target, points towards a second target.
/// 
/// </summary>
public class NavArrow : MonoBehaviour {

	/// <summary>
	/// Object to orbit
	/// </summary>
	public Transform Owner; 

	/// <summary>
	/// object to point towards
	/// </summary>
	public GameObject Target;
	
	/// <summary>
	/// Cache of direction calculated from owner to target
	/// </summary>
	public Vector3 direction = Vector3.zero;

	/// <summary>
	/// Distance the arrow maintains from Owner (radius)
	/// </summary>
	public float ArrowRadius = 2.0f; 

	/// <summary>
	/// The actual sprite for the arrow.
	/// </summary>
	public SpriteRenderer _SpriteRenderer = null;

	private Transform lastTransform = null;

	public enum NAVARROW_TYPES
	{
		NAV_EARTH,
		NAV_ENEMY,
		NAV_BEACON
	}
	// NAV_EARTH : points to earth
	// NAV_ENEMY : points to enemy group or enemy.
	// NAV_BEACON : points to friendly beacon

	public NAVARROW_TYPES ArrowType = NAVARROW_TYPES.NAV_EARTH; // point to earth by default
	
	private bool _visible = true;
	public bool ArrowVisible
		{
				get
				{
						return _visible;
				}
				set
				{
						_visible = value;
						if (value)
						{
								Show();
						}
						else{
								Hide();
						}
				}
		}

	public Camera_Controller _Camera_Controller;

	// Use this for initialization
	void Start () 
	{
				if (_SpriteRenderer == null) {
						_SpriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
				}

		if (_Camera_Controller == null)
		{
				_Camera_Controller = GameObject.FindObjectOfType<Camera_Controller> ();
		}

		if (Owner == null)
		{
				Owner = GameObject.FindGameObjectWithTag("Player").transform;
		}

		if (Owner != null) 
		{
			transform.position = Owner.transform.position + new Vector3 (0, ArrowRadius, 0);
		}

		ArrowVisible = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!ArrowVisible) 
		{
			return;
		}

		if (Target == null) {			
				return;
		}

				// Get vector diff to target from Owner.
				direction = (Target.transform.position - Owner.transform.position);
				direction.Normalize ();

				// Point at the target.
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation (transform.position, Target.transform.position - transform.position),Time.deltaTime);

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
															
								_SpriteRenderer.color = Color.red;
							}
							else if (Target.CompareTag("Enemy"))
							{
								enemy_pilot tmpPilot = Target.GetComponent<enemy_pilot>();
								if (tmpPilot != null)
								{
								tmpDistNav = tmpPilot.GetNavMinDistance();	
								}

							_SpriteRenderer.color = Color.red;
							}

							break;
						}
						case NAVARROW_TYPES.NAV_EARTH:
						{
							//_SpriteRenderer.color = Color.blue;

							break;
						}
						case NAVARROW_TYPES.NAV_BEACON:
						{
							//_SpriteRenderer.color = Color.green;

							break;
						}
						default:
						{
							// whatever
							//_SpriteRenderer.color = Color.gray;
							break;
						}
					}

					float distance = Vector2.Distance (transform.position, Target.transform.position);
					// only position the target if distance is sufficiently far away
					if (distance > ArrowRadius)
					{
						if (distance > tmpDistNav)
						{
							transform.position = Owner.transform.position + (direction.normalized * ArrowRadius);
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
					transform.position = Owner.position + (direction.normalized * ArrowRadius);
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
		temp_pos = Owner.transform.position + new Vector3(0, ArrowRadius, 0);

		return temp_pos;
	}

	public void Show()
	{
		_SpriteRenderer.enabled = true;
	}

	public void Hide()
	{
		_SpriteRenderer.enabled = false;
	}
				
	// use euler angles to calculate rotation angle
	float GetTargetAngleMath(Transform t)
	{
		float rotation;
		rotation = Mathf.Atan2(transform.position.x - t.position.x, transform.position.y - t.position.y);

		// rotation = rotation * 180/Math.PI; //convert radians to degrees

		return rotation;
	}

		Quaternion GetTargetAngleQuat(Transform target)
	{
		Vector3 relativePos = (target.transform.position - Owner.transform.position);
				Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.back);

		Debug.DrawRay(Owner.transform.position, relativePos /2 , Color.grey);
		return rotation; 
	}

		Quaternion LookAt(Transform target)
	{
		Vector3 relativePos = target.transform.position - Owner.transform.position;
				Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.back);

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

	/// <summary>
	/// Uses the Camera_Controller to determine if transform is on screen
	/// </summary>
	/// <param name="tform">Tform.</param>
		public bool isOnScreen(Transform tform)
	{
		if (_Camera_Controller == null)
		{
			// can't use camera controller so everything is on screen.
			return true;
		}

		return _Camera_Controller.isOnScreen (tform);
	}
}
