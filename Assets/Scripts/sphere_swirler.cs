using UnityEngine;
using System.Collections;

public class sphere_swirler : MonoBehaviour {
	public float rotation_speed = 1.0f;
	public float cutoffValue_min = 0.0f;
	public float cutoffValue_max = 1.0f;
	public float cutoffDelta = 0.025f;

	private float cutOffValue = 0.0f;
	private bool cutoffdir = true; // true = up, false = down;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(rotation_speed, rotation_speed, rotation_speed));
		transform.position = transform.parent.position;

		GetComponent<Renderer>().material.SetFloat("_Cutoff", cutOffValue);

		if (cutoffdir == true)
		{
			if (cutOffValue < cutoffValue_max)
			{
				cutOffValue += cutoffDelta;
			}
			else
			{
				cutOffValue = cutoffValue_max;
				cutoffdir = false;
			}
		}
		else if (cutoffdir == false)
		{
			if (cutOffValue > cutoffValue_min)
			{
				cutOffValue -= cutoffDelta;
			}
			else
			{
				cutOffValue = cutoffValue_min;
				cutoffdir = true;
			}

		}


	}
}
