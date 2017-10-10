
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// PizzaNode Playercontroller.cs

public class unit_controller : MonoBehaviour {

	public int speed = 5;
	public Vector3 maxVelocity = new Vector3(10,10,0);

	// list of child weapons and their control scripts
	private List<GameObject> WeaponList = new List<GameObject>();
	private List<Weapon> WeaponCtrlList = new List<Weapon>();


	// Use this for initialization
	void Start () 
	{
		RegisterWeapons();
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	/// <summary>
	/// Attempt to .Fire() all child weapons.
	/// </summary>
	void FireAllWeapons()
	{
		// gets a list of all player child weapon scripts and calls Fire method on each
		foreach (Weapon wpn in WeaponCtrlList)
		{
			wpn.Fire();
		}
	}


	/// <summary>
	/// gets a list of all child weapon objects, adds them to appropriate lists.
	/// </summary>
	public void RegisterWeapons()
	{
		Weapon tmpWeaponScript = null;
		
		foreach (Transform t in transform)
		{
			if (t.CompareTag("Weapon"))
			{
				// found a weapon
				WeaponList.Add(t.gameObject);
				tmpWeaponScript = t.gameObject.GetComponent<Weapon>();
				
				if (tmpWeaponScript != null)
				{
					WeaponCtrlList.Add(tmpWeaponScript);
				}
			}
		}
	}

}
