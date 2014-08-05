using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class weapon_01 : MonoBehaviour {

	// What type of bullets does this weapon fire?
	public GameObject bullet_type;
	public GameObject Target; // reference to target 

	public float max_charge = 100.0f; // maximum potential weapon energy
	public float charge = 100.0f; // weapon energy available
	public float charge_rate = 10.0f; // amount added per charge time
	public float charge_time = 1.0f; // number of seconds before charge_rate updates
	public float cost_per_bullet = 20.0f; // weapon energy spent everytime a bullet fires
	public float shot_spacing = 0.3f; // seconds to wait before firing another shot.

	public bool charge_ready = false; // if weapon has enough charge to fire, set this to true
	public bool shot_ready = false; // if weapon
	private float timer = 0.0f; // internal timer for charging
	private float shot_timer = 0.0f; // 


	public AudioClip[] snd_shot; // sound of shot firing

	private bool audio_connected = true; // set to true if detect audio component

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
			Target = gameObject.transform.parent.GetComponent<enemy_pilot>().Target;
		}
		else if (transform.parent.tag == "Player")
		{
			Target = GameObject.Find("player_target");
			
			if (Target == null)
			{
				Debug.Log("Couldn't find player target to link to weapon firing script!");
			}
		}
		else
		{
			Debug.Log("Couldn't find player target to link to weapon firing script!");
		}

		if (GetComponent<AudioSource>())
		{
			audio_connected = true;
		}
		else
		{
			Debug.Log (gameObject.name.ToString() + ": No AudioSource connected on this weapon!");
		}

	}
	
	// Update is called once per frame
	void Update () 
	{
		re_target();
		update_charge_timer();
		update_shot_timer();
	}

	void re_target()
	{
		if (transform.parent.tag == "Enemy")
		{
			Target = gameObject.transform.parent.GetComponent<enemy_pilot>().Target;
		}
		else
		{
			Target = GameObject.Find("player_target");
			
			if (Target == null)
			{
				Debug.Log("Couldn't find player target to link to weapon firing script!");
			}
		}
	}

	/// <summary>
	/// Called from enemy_pilot.cs script as a child object.
	/// Checks to see if a bullet can be fired and instantiates prefab.
	/// </summary>
	public void Fire()
	{
		if (charge >= cost_per_bullet)
		{
			if (shot_timer == 0.0f)
			{
				charge -= cost_per_bullet; // ZAP

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
				temp_bullet.GetComponent<move_to_target>().selected = Target;
				
				// after a bullet has been fired, set the spacing timer to disallow further bullets
				shot_timer = shot_spacing;
				
				if (audio_connected)
				{
					shot_sound_play();
				}

			} // end shot_timer == 0.0f
			else
			{
				// weapon not ready to fire next bullet. (cooldown timer still counting down)
				shot_ready = false; 
			}
		}
		else
		{
			// weapon doesn't have enough charge to fire
			charge_ready = false;
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

			// weapon is ready to fire if there's enough charge to shoot 1 bullet
			if (charge >= cost_per_bullet)
			{
				charge_ready = true;
			}
			else
			{
				charge_ready = false;
			}

		

		}
	}

	void update_shot_timer()
	{
		if (shot_timer > 0.0f)
		{
			if ((shot_timer - Time.deltaTime) < 0.0f)
			{
				shot_timer = 0.0f;
				shot_ready = true;
			}
			else
			{
				shot_timer -= Time.deltaTime;
				shot_ready = false;
			}
		}
		else
		{
			shot_ready = true;
			// no need to update, next bullet ready to fire.
		}
	}

	void shot_sound_play()
	{
		if (snd_shot.Length>0)
		{
			audio.clip = snd_shot[0];
			audio.Play();
		
			//AudioSource.PlayClipAtPoint(snd_shot[0], transform.position);
		}


	}

}
