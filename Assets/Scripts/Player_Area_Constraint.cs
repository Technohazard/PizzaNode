using UnityEngine;
using System.Collections;

public class Player_Area_Constraint : MonoBehaviour {

	private CircleCollider2D mConstraint = null;
	public bool isActive = true;
	public GameObject mPlayerObj = null;

	// Use this for initialization
	void Start () 
	{
		RegisterConstraint ();
		RegisterPlayer ();
	}
	
	// Update is called once per frame
	void Update () {

		if (isActive)
		{
				CheckConstraint();
		}
	}

	void RegisterPlayer ()
	{
		if (mPlayerObj == null) {
			mPlayerObj = GameObject.Find ("Player_Unit");
		}
		if (mPlayerObj == null) {
			Debug.Log (this + ": Couldn't find PlayerObj to constrain!");
			isActive = false;
		}
	}

	void RegisterConstraint ()
	{
		if (mConstraint == null) {
			mConstraint = gameObject.GetComponent<CircleCollider2D> ();
		}
		if (mConstraint == null) {
			Debug.Log (this + ": AreaConstraint not active - no CircleCollider2D found.");
			isActive = false;
		}
	}

	private void CheckConstraint()
	{
		// is player in constraint?
		if ((mPlayerObj != null) && (mConstraint != null))
		{
			// if player is outside constraint
		}

	}

	void OnTriggerExit2D (Collider2D other) 
	{
		if (other.CompareTag("Player"))
		{
			Vector3 tmpVec = Vector3.zero;
			Vector3 tmpVelVec = Vector3.zero;

			tmpVec = transform.position - other.transform.position;
			tmpVec.Normalize();
			transform.position = tmpVec * mConstraint.radius;

			GetComponent<Rigidbody>().velocity.Set(tmpVelVec.x, tmpVelVec.y, tmpVelVec.z); 
		}
	}
}
