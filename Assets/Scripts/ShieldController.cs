using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles behavior of child Shields. Extends HealthController.
/// </summary>
public class ShieldController : HealthController {
	public enum Shield_Types 
	{
		DEFAULT = 0,
		PERFECT = 1,
	}
	/* Definitions of Shield_Types
	 * DEFAULT: Received unmoderated damage.
	 * PERFECT: Reduced all incoming damage to zero.
	*/

	public Shield_Types mShieldType = Shield_Types.DEFAULT;

	private sphere_swirler refSphereSwirler = null;
	// Use this for initialization
	void Start () 
	{
		InitializeShield();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void InitializeShield()
	{
		// for now, load defaults.
		mShieldType = Shield_Types.DEFAULT;

		// register sphere sweirler
		RegisterSphereSwirler();
	}

	private void RegisterSphereSwirler()
	{
		if (refSphereSwirler == null)
		{
			Debug.Log (this + ": Finding sphere_swirler...");
			refSphereSwirler = transform.GetComponentInChildren<sphere_swirler>();
			RegisterSphereSwirler();
		}
		else
		{
			// 
			Debug.Log(this + ": sphere_swirler component is registered!");
			return;
		}

	}
}
