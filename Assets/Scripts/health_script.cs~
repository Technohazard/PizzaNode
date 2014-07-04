using UnityEngine;
using System.Collections;

public class health_script : MonoBehaviour {

	public float hp = 1.0f;
	public float max_hp = 1.0f;

	public float shield_max = 100.0f; // maximum potential shield energy
	public float shield = 100.0f; // weapon energy available
	public float s_charge_rate = 10.0f; // amount added per charge time
	public float s_charge_time = 1.0f; // number of seconds before charge_rate updates

	private float s_timer = 0.0f; // internal timer for charging shield
	private float dmg_remainder = 0.0f; // if dmg is dealt resulting in shield < 0, deal this remainder to health.

	public bool isEnemy = true;

	// Use this for initialization
	void Start () {
		if (gameObject.tag == "Enemy")
			{
				isEnemy = true;
			}
		else if (gameObject.tag == "Player")
		{
			isEnemy = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		update_shield_charge_timer();
	}

	public void Damage(int damageCount)
	{
		if (shield > 0)
		{ 
			// damage always deals to the shield first, if it's up.
			if (shield - damageCount > 0)
			{ 
				shield -= damageCount;
			}
			else
			{
				// damage dealt is greater than remaining shield
				dmg_remainder = 0.0f;
				dmg_remainder = damageCount - shield;
				shield = 0.0f;

				hp -= dmg_remainder;
			}
		}
		else
		{ 
			hp -= damageCount;
		}
		
		if (hp <= 0)
		{
			// Dead!
			Destroy(gameObject);
		}
	}

	void update_shield_charge_timer()
	{
		s_timer += Time.deltaTime;
		if(s_timer > s_charge_time)
		{
			s_timer = 0.0f; // reset timer
			
			// recharge on update time
			if (shield < shield_max)
			{
				if ((shield + s_charge_rate) <= shield_max)
				{
					shield += s_charge_rate;
				}
				else if ((shield + s_charge_rate) > shield_max)
				{
					shield = shield_max;
				}
			}
		}
	}

	void OnTriggerEnter2D(Collider2D otherCollider)
	{
			// Is this a shot?
			bullet_01 shot = otherCollider.gameObject.GetComponent<bullet_01>();
			if (shot != null)
			{
				// Avoid friendly fire
				if (shot.EnemyShot != isEnemy)
				{
					Damage(shot.damage);
					
					// Destroy the shot
					shot.impact(); // Remember to always target the game object, otherwise you will just remove the script
				}
			}		
	}

}
