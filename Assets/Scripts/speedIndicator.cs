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
	private unit_controller myPlayerUnit;
	private GameObject playerLink; // link to the player to reference rigidbody component


	// Use this for initialization
	void Start () 
	{
		playerLink = GameObject.Find ("Player");
		if (!playerLink) 
		{
			Debug.Log(gameObject.name.ToString() + " : Player Object not found in scene!");
		}
		else
		{
			// successfully added player link object
		}

		// connect and initialize LineRenderer Component
		myLineRenderer = GetComponent<LineRenderer>();

		if (!myLineRenderer)
		{
			Debug.Log (gameObject.name.ToString() + " : Linerenderer component not found!");			      
		}
		else
		{
			myLineRenderer.SetVertexCount(lengthOfLineRenderer);
			myLineRenderer.SetPosition(0, playerLink.transform.position);
			myLineRenderer.SetPosition(1, playerLink.transform.position);
			myLineRenderer.SetColors(c1, c2);
			myLineRenderer.SetWidth(startWidth, endWidth);

		}

		// Connect parent velocity limit from <unit_controller> (if any)
		myPlayerUnit = GetComponent<unit_controller>();

		if (!myPlayerUnit) 
		{
			Debug.Log(gameObject.name.ToString() + " : unit_controller not found!");
		}
		else
		{
			VelocityLimit = myPlayerUnit.maxVelocity;	
		}

	}
	
	// Update is called once per frame
	void Update () {
		// get velocity
		// update line renderer end node to match velocity * velocity % indicator
		// 0 = minimum radius
		// 1 = maximum radius

		myLineRenderer.SetPosition(0, playerLink.transform.position);

		copyVelocity = playerLink.rigidbody2D.velocity;

		Vector2 targetVelocityPos = getVelocityHeading (playerLink.rigidbody2D.velocity).normalized;

		targetVelocityPos.x *= Mathf.Lerp(minRadius, maxRadius, (playerLink.rigidbody2D.velocity.x / VelocityLimit.x));
		targetVelocityPos.y *= Mathf.Lerp(minRadius, maxRadius, (playerLink.rigidbody2D.velocity.y / VelocityLimit.y));

		myLineRenderer.SetPosition(1, playerLink.transform.position - (Vector3)targetVelocityPos);
	}

	Vector2 getVelocityHeading(Vector2 t)
	{
		Vector2 relativePos = (t - (Vector2)playerLink.transform.position);
		return relativePos;
	}

}
