using UnityEngine;
using System.Collections;

public class weapon_01 : MonoBehaviour {

	// What type of bullets does this weapon fire?
	public GameObject bullet_type;

	public float max_charge = 100.0f; // maximum potential weapon energy
	public float charge = 100.0f; // weapon energy available
	public float charge_rate = 10.0f; // amount added per charge time
	public float charge_time = 1.0f; // number of seconds before charge_rate updates
	public float cost_per_bullet = 20.0f; // weapon energy spent everytime a bullet fires

	private float timer = 0.0f; // internal timer for charging

	private GameObject target_ref; // reference to player target 
	
	// Use this for initialization
	void Start () 
	{
		// if no bullet type is assigned, use defaults
		if (bullet_type == null)
		{
			bullet_type = (GameObject)Resources.Load("bullet_red_01");
		}

		// Locate the player target and get a reference to it
		target_ref = GameObject.Find("player_target");

		if (target_ref == null)
		{
			Debug.Log("Couldn't find player target to link to weapon firing script!");
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		update_charge_timer();
	}

	public void Fire()
	{
		Debug.Log("pew!");
		if (charge >= cost_per_bullet)
		{
			charge -= cost_per_bullet;

			// instantiate here
			GameObject temp_bullet;
	
			temp_bullet = (GameObject)Instantiate(bullet_type, transform.position, Quaternion.identity);

			// Set the bullet's target to the current playertarget
			temp_bullet.GetComponent<move_to_target>().SetTarget(target_ref.transform.position);

			// temp_bullet.move_to_target.target_stored_position = new Vector3(-10,-10,0);
		}
	}

	void update_charge_timer() 
	{
		timer += Time.deltaTime;
		if(timer > charge_time)
		{
			timer = 0.0f; // reset timer

			// recharge on update time
			if (charge < max_charge)
			{
				if ((charge + charge_rate) <= max_charge)
				{
					charge += charge_rate;
				}
				else if ((charge + charge_rate) > max_charge)
				{
					charge = max_charge;
				}
			}
		}
	}

}
