using UnityEngine;
using System.Collections;

public class enemy_pilot : MonoBehaviour {

	public float speed = 1.0f;
	public float turn_speed = 1.0f;

	public GameObject target;

	private weapon_01[] weapons; // list of player weapon components;

	// Use this for initialization
	void Start () {
		target = GameObject.Find("Player");
		weapons = GetComponentsInChildren<weapon_01>();
	}

	// Update is called once per frame
	void Update () {
		lookat(target);
		Thrust();
	}

	void Thrust()
	{
		// add movement in the direction the object is facing.

		// get normalized vector from rotation quaternion
		//transform.Translate(new Vector3(facing.x * speed, facing.y * speed, 0));

	}

	void Fire()
	{
		// gets a list of all player child weapon scripts and calls Fire method on each
		foreach (weapon_01 wpn in weapons)
		{
			wpn.Fire();
		}
	}

	void calc_facing()
	{
		// rotate object to face the direction it's moving.
		Vector3 dir = rigidbody2D.velocity;

		// calculate the angle of current speed.
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

		// rotate the object to face that angle
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

	}

	void lookat(GameObject to_target)
	{
		Vector3 relativePos = to_target.transform.position - transform.position;
		Quaternion rotation = Quaternion.LookRotation(relativePos);
		transform.rotation = rotation;
	}

}
