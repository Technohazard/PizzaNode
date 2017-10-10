using UnityEngine;
using System.Collections;

public class speedIndicator : MonoBehaviour {

	public int lengthOfLineRenderer = 2; // number of nodes to render between origin and target
	public Color c1 = Color.yellow;
	public Color c2 = Color.red;
	public Vector2 VelocityLimit = new Vector2(10,10);

	public float minRadius = 1.0f;
	public float maxRadius = 2.0f;

	public float startWidth = 0.5f;
	public float endWidth = 0.1f;
	public Vector2 copyVelocity;

	private LineRenderer myLineRenderer;
	private GameObject Target = null;
	private Rigidbody2D TargetRigidBody2D = null;
	private unit_controller mTargetUnitControl = null;


	// Use this for initialization
	void Start () 
	{
		Target = GameObject.Find("Player_Unit");

		if (!Target) 
		{
			Debug.Log(gameObject.name.ToString() + " : Player not found in scene!");
		}
		else
		{
			// successfully added player link object
		}

		RegisterRigidBody();

		RegisterLineRenderer ();


		// Connect parent velocity limit from <unit_controller> (if any)
		if (mTargetUnitControl == null) 
		{
			RegisterUnitControl();
		}

		if (mTargetUnitControl != null)
		{
			VelocityLimit = mTargetUnitControl.maxVelocity;	
		}

	}


	/// <summary>
	/// connect and initialize LineRenderer Component
	/// </summary>
	void RegisterLineRenderer ()
	{
		myLineRenderer = GetComponent<LineRenderer> ();
		if (!myLineRenderer) {
			Debug.Log (gameObject.name.ToString () + " : Linerenderer component not found!");
		}
		else {
			myLineRenderer.positionCount = lengthOfLineRenderer;
			myLineRenderer.SetPosition (0, transform.position);
			myLineRenderer.SetPosition (1, Target.transform.position);
			myLineRenderer.startColor = c1;
			myLineRenderer.startColor = c2;
			myLineRenderer.startWidth = startWidth;
			myLineRenderer.endWidth = endWidth;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ((Target != null)&& (TargetRigidBody2D != null))
		{
			// update line renderer end node to match velocity * velocity % indicator
			myLineRenderer.SetPosition(0, Target.transform.position);

			Vector2 targetVelocityPos = getVelocityHeading (TargetRigidBody2D.velocity).normalized;

			targetVelocityPos.x *= Mathf.Lerp(minRadius, maxRadius, (TargetRigidBody2D.velocity.x / VelocityLimit.x));
			targetVelocityPos.y *= Mathf.Lerp(minRadius, maxRadius, (TargetRigidBody2D.velocity.y / VelocityLimit.y));

			myLineRenderer.SetPosition(1, Target.transform.position - (Vector3)targetVelocityPos);
		}
	}

	Vector2 getVelocityHeading(Vector2 t)
	{
		Vector2 relativePos = (t - (Vector2)Target.transform.position);
		return relativePos;
	}

	public void RegisterRigidBody()
	{
		if (Target != null)
		{
			TargetRigidBody2D = Target.GetComponent<Rigidbody2D>();
		}
	}

	public void RegisterUnitControl()
	{
		if (Target != null)
		{
			mTargetUnitControl = Target.GetComponent<unit_controller>();
		}
	}
}
