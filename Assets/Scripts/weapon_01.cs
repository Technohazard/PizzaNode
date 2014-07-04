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

	public bool ready; // if weapon has enough charge to fire, set this to true

	private float timer = 0.0f; // internal timer for charging

	public GameObject target_ref; // reference to target 
	
	// Use this for initialization
	void Start () 
	{
		// if no bullet type is assigned, use defaults
		if (bullet_type == null)
		{
			bullet_type = (GameObject)Resources.Load("bullet_red_01");
		}

		// Locate the player target and get a reference to it
		if (transform.parent.tag == "Enemy")
		{
			target_ref = gameObject.transform.parent.GetComponent<enemy_pilot>().target;
		}
		else if (transform.parent.tag == "Player")
		{
			target_ref = GameObject.Find("player_target");
			
			if (target_ref == null)
			{
				Debug.Log("Couldn't find player target to link to weapon firing script!");
			}
		}
		else
		{
			Debug.Log("Couldn't find player target to link to weapon firing script!");
		}

	}
	
	// Update is called once per frame
	void Update () 
	{
		re_target();
		update_charge_timer();
	}

	void re_target()
	{
		if (transform.parent.tag == "Enemy")
		{
			target_ref = gameObject.transform.parent.GetComponent<enemy_pilot>().target;
		}
		else
		{
			target_ref = GameObject.Find("player_target");
			
			if (target_ref == null)
			{
				Debug.Log("Couldn't find player target to link to weapon firing script!");
			}
		}
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

			// check to see whether fired by enemy or player
			if (transform.parent.tag == "Enemy")
			{
				temp_bullet.transform.parent = GameObject.Find("Enemy_Bullet_Group").transform;
			}
			else
			{
				temp_bullet.transform.parent = GameObject.Find("Player_Bullet_Group").transform;
			}

			// Set the bullet's target to the current playertarget
			temp_bullet.GetComponent<move_to_target>().selected = target_ref;
		}
		else
		{
			// weapon doesn't have enough charge to fire
			ready = false;
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
		
					// weapon is ready to fire if there's enough charge to shoot 1 bullet
					if (charge > cost_per_bullet)
					{
						ready = true;
					}
				}
				else if ((charge + charge_rate) > max_charge)
				{
					charge = max_charge;
				}
			}
		}
	}

}
