using UnityEngine;
using System.Collections;

public class health_script : MonoBehaviour {

	public float hp = 10.0f;
	public float hp_max = 10.0f;
	
	public float charge_rate = 10.0f; // amount added per charge time
	public float charge_time = 1.0f; // number of seconds before charge_rate updates

	private float timer = 0.0f; // internal timer for restoring hp
	private float dmg_remainder = 0.0f; // if dmg is dealt resulting in health < 0, deal this remainder to parent.

	public bool isEnemy = true;

	// Use this for initialization
	void Start () 
	{
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
	void Update () 
	{
		recharge();
	}

	public void Damage(int damageCount)
	{
		if (hp > 0)
		{ 
			// damage always deals to the shield first, if it's up.
			if (hp - damageCount > 0)
			{ 
				hp -= damageCount;
			}
			else
			{
				// damage dealt is greater than remaining shield
				dmg_remainder = 0.0f;
				dmg_remainder = damageCount - hp;
				hp = 0.0f;

				gameObject.SendMessageUpwards("Damage", dmg_remainder);
			}
		}
		else
		{ 
			hp -= damageCount;
		}
		
		if (hp <= 0)
		{
			// Dead!
			Die();
		}
	}

	void Die()
	{
		// do 'death' stuff here before destroying. (anims, etc)
		// play death sound, etc.

		// tell parent enemy_wave manager that this unit died!
		if (gameObject != null)
		{
			if (gameObject.transform.parent != null)
			{
				if (transform.root != transform)
				{
					gameObject.transform.parent.SendMessage("removeEnemy", gameObject);
				}
			}
		}

		Destroy(gameObject,0.1f);
	}

	void recharge()
	{
		timer += Time.deltaTime;
		if(timer > charge_time)
		{
			timer = 0.0f; // reset timer
			
			// recharge on update time
			if (hp < hp_max)
			{
				if ((hp + charge_rate) <= hp_max)
				{
					hp += charge_rate;
				}
				else if ((hp + charge_rate) > hp_max)
				{
					hp = hp_max;
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
